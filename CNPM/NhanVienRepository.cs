using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace CNPM
{
    internal class NhanVienRepository
    {
        //kiểm tra đăng nhập
        public static ThongTinNhanVien CheckLogin(string username, string password)
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    NV.MaNhanVien, NV.HoTen, NV.CCCD, NV.Gmail, NV.SoDienThoai, NV.DiaChi,
                    TK.VaiTro, TK.TrangThai
                FROM TAIKHOAN TK
                JOIN NHANVIEN NV ON TK.MaLienKet = NV.MaNhanVien
                WHERE TK.TenDangNhap = @user
                  AND TK.MatKhau = @pass
                  AND TK.VaiTro IN ('NhanVien','QuanLy')
                  AND TK.TrangThai = 1
            ";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@pass", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var nv = new ThongTinNhanVien();

                                // MaNhanVien (int)
                                nv.MaNhanVien = reader.GetInt32(reader.GetOrdinal("MaNhanVien"));

                                // HoTen
                                nv.HoTen = reader.IsDBNull(reader.GetOrdinal("HoTen")) ? null : reader.GetString(reader.GetOrdinal("HoTen"));

                                // CCCD
                                nv.CCCD = reader.IsDBNull(reader.GetOrdinal("CCCD")) ? null : reader.GetString(reader.GetOrdinal("CCCD"));

                                // Gmail
                                nv.Gmail = reader.IsDBNull(reader.GetOrdinal("Gmail")) ? null : reader.GetString(reader.GetOrdinal("Gmail"));

                                // SoDienThoai (lưu ý tên cột trong DB là SoDienThoai)
                                nv.DienThoai = reader.IsDBNull(reader.GetOrdinal("SoDienThoai")) ? null : reader.GetString(reader.GetOrdinal("SoDienThoai"));

                                // DiaChi
                                nv.DiaChi = reader.IsDBNull(reader.GetOrdinal("DiaChi")) ? null : reader.GetString(reader.GetOrdinal("DiaChi"));

                                // VaiTro lấy từ TAIKHOAN
                                nv.VaiTro = reader.IsDBNull(reader.GetOrdinal("VaiTro")) ? null : reader.GetString(reader.GetOrdinal("VaiTro"));

                                // TrangThai lấy từ TAIKHOAN (SQL BIT => bool)
                                nv.TrangThai = !reader.IsDBNull(reader.GetOrdinal("TrangThai")) && reader.GetBoolean(reader.GetOrdinal("TrangThai"));

                                return nv;
                            }
                        }
                    }
                }

                return null; // không tìm thấy hoặc sai thông tin
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu!\nChi tiết: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi đăng nhập!\nChi tiết: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }


        // số vé đã bán trong tháng hiện tại 
        public static int GetSoVeBanTrongThang()
        {
            string query = @"
        SELECT ISNULL(COUNT(MaVe), 0)
        FROM VE
        WHERE MONTH(NgayDat) = MONTH(GETDATE())
          AND YEAR(NgayDat) = YEAR(GETDATE())";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }


        // doanh thu trong ngày hiện tại
        public static decimal GetDoanhThuHomNay()
        {
            string query = @"
        SELECT ISNULL(SUM(TongTien), 0)
        FROM LICHSUGIAODICH
        WHERE CONVERT(date, ThoiGianDat) = CONVERT(date, GETDATE())";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        //số chuyến tàu đã khởi hành hôm nay 
        public static int GetSoChuyenTauKhoiHanhHomNay()
        {
            string query = @"
        SELECT COUNT(*)
        FROM CHUYENTAU
        WHERE CONVERT(date, NgayDi) = CONVERT(date, GETDATE())
          AND GioDi <= CONVERT(time, GETDATE())";  // tức là đã khởi hành

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // tiem kiếm bảng vé bằng tên, sdt, cccd hoặc ngày đặt
        public static DataTable TimKiemVe(string tuKhoa)
        {
            string query = @"
        SELECT 
            v.MaVe,
            v.TenNguoiSoHuu,
            v.SoDienThoai,
            v.CCCD,
            v.LoaiGhe,
            v.SoGhe,
            v.GiaTien,
            v.NgayDat
        FROM VE v
        WHERE 
            v.TenNguoiSoHuu LIKE @TuKhoa
            OR v.SoDienThoai LIKE @TuKhoa
            OR v.CCCD LIKE @TuKhoa
            OR CONVERT(NVARCHAR(10), v.NgayDat, 120) LIKE @TuKhoa";
            

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TuKhoa", "%" + tuKhoa + "%");

                conn.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        //tìm kiếm bảng chuyến tàu bằng nơi đi hoạc nơi đến
        public static DataTable TimKiemChuyenTau(string tuKhoa)
        {
            string query = @"
        SELECT 
           *
        FROM CHUYENTAU
        WHERE 
            NoiDi LIKE '%' + @TuKhoa + '%'
            OR NoiDen LIKE '%' + @TuKhoa + '%'
        ORDER BY NgayDi ASC";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TuKhoa", tuKhoa ?? "");

                conn.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
        public static bool ThemTuyenTauMoi(
           string noiDi,
           string noiDen,
           TimeSpan gioDi,
           TimeSpan gioDen,
           DateTime ngayDi,
           int tongSoGhe,
           decimal giaGheMem,
           decimal giaGheCung,
           int nguoiTao)
        {
            string query = @"
                INSERT INTO CHUYENTAU 
                (NoiDi, NoiDen, GioDi, GioDen, NgayDi, TongSoGhe, GiaGheMem, GiaGheCung, NguoiTao)
                VALUES 
                (@NoiDi, @NoiDen, @GioDi, @GioDen, @NgayDi, @TongSoGhe, @GiaGheMem, @GiaGheCung, @NguoiTao);
            ";

            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NoiDi", noiDi);
                    cmd.Parameters.AddWithValue("@NoiDen", noiDen);
                    cmd.Parameters.AddWithValue("@GioDi", gioDi);
                    cmd.Parameters.AddWithValue("@GioDen", gioDen);
                    cmd.Parameters.AddWithValue("@NgayDi", ngayDi);
                    cmd.Parameters.AddWithValue("@TongSoGhe", tongSoGhe);
                    cmd.Parameters.AddWithValue("@GiaGheMem", giaGheMem);
                    cmd.Parameters.AddWithValue("@GiaGheCung", giaGheCung);
                    cmd.Parameters.AddWithValue("@NguoiTao", nguoiTao);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                // Bạn có thể log lỗi ra file hoặc bảng NHATKY_HOATDONG
                Console.WriteLine("Lỗi khi thêm tuyến tàu mới: " + ex.Message);
                return false;
            }
        }
        public static DataTable LayChuyenTauHomNay()
        {
            string query = @"
        SELECT 
            MaChuyen, 
            NoiDi, 
            NoiDen, 
            GioDi, 
            GioDen, 
            NgayDi,
            CASE 
                WHEN GETDATE() BETWEEN CAST(NgayDi AS DATETIME) + CAST(GioDi AS DATETIME)
                                 AND CAST(NgayDi AS DATETIME) + CAST(GioDen AS DATETIME)
                THEN N'Đang khởi hành'
                WHEN GETDATE() < CAST(NgayDi AS DATETIME) + CAST(GioDi AS DATETIME)
                THEN N'Chưa khởi hành'
                ELSE N'Đã kết thúc'
            END AS TrangThai
        FROM CHUYENTAU
        WHERE CAST(NgayDi AS DATE) = CAST(GETDATE() AS DATE)
        ORDER BY GioDi ASC";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        //ghi thông báo đăng nhập
        public static void GhiThongBaoDangNhap(int maNguoi, string role)
        {
            string query = @"
    INSERT INTO THONGBAO_NV (NoiDung, MaNguoiNhan, Role, ThoiGian, DaXem)
    VALUES (@NoiDung, @MaNguoiNhan, @Role, @ThoiGian, 0)";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                string noiDung = $"Bạn đã đăng nhập vào lúc {DateTime.Now:HH:mm:ss} ngày {DateTime.Now:dd/MM/yyyy}.";

                cmd.Parameters.AddWithValue("@NoiDung", noiDung);
                cmd.Parameters.AddWithValue("@MaNguoiNhan", maNguoi);
                cmd.Parameters.AddWithValue("@Role", role);
                cmd.Parameters.AddWithValue("@ThoiGian", DateTime.Now);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static bool CoThongBaoChuaXem(int maNguoiNhan, string role)
        {
            string query = @"
    SELECT COUNT(*) 
    FROM THONGBAO_NV
    WHERE (MaNguoiNhan = @MaNguoiNhan OR MaNguoiNhan IS NULL)
          AND DaXem = 0
          AND Role = @Role";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaNguoiNhan", maNguoiNhan);
                cmd.Parameters.AddWithValue("@Role", role);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public static void DanhDauDaXem(int maNguoiNhan, string role)
        {
            string query = @"
    UPDATE THONGBAO_NV
    SET DaXem = 1
    WHERE (MaNguoiNhan = @MaNguoiNhan OR MaNguoiNhan IS NULL)
    AND Role = @Role";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaNguoiNhan", maNguoiNhan);
                cmd.Parameters.AddWithValue("@Role", role);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static DataTable LayThongBaoTheoNguoiNhan(int maNguoiNhan, string role)
        {
            DataTable dt = new DataTable();
            string query = @"
    SELECT 
        MaThongBao,
        NoiDung,
        FORMAT(ThoiGian, 'dd/MM/yyyy HH:mm') AS ThoiGian,
        DaXem
    FROM THONGBAO_NV
    WHERE (MaNguoiNhan IS NULL OR MaNguoiNhan = 0 OR MaNguoiNhan = @MaNguoiNhan)
          AND Role = @Role
    ORDER BY ThoiGian DESC";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaNguoiNhan", maNguoiNhan);
                cmd.Parameters.AddWithValue("@Role", role);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }


    }
}
