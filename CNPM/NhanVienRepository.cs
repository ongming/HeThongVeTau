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

                    // Lấy vai trò trước
                    string roleQuery = "SELECT VaiTro FROM TAIKHOAN WHERE TenDangNhap = @user AND MatKhau = @pass AND TrangThai = 1";
                    string role = null;

                    using (SqlCommand cmdRole = new SqlCommand(roleQuery, conn))
                    {
                        cmdRole.Parameters.AddWithValue("@user", username);
                        cmdRole.Parameters.AddWithValue("@pass", password);
                        object result = cmdRole.ExecuteScalar();
                        role = result?.ToString();
                    }

                    if (string.IsNullOrEmpty(role)) return null;

                    string query = "";

                    if (role == "NhanVien")
                    {
                        query = @"
                    SELECT NV.MaNhanVien AS MaNguoiDung, NV.HoTen, NV.CCCD, NV.Gmail, NV.SoDienThoai, NV.DiaChi, TK.VaiTro, TK.TrangThai
                    FROM TAIKHOAN TK
                    JOIN NHANVIEN NV ON TK.MaLienKet = NV.MaNhanVien
                    WHERE TK.TenDangNhap = @user AND TK.MatKhau = @pass AND TK.TrangThai = 1";
                    }
                    else if (role == "QuanLy")
                    {
                        query = @"
                    SELECT QL.MaQuanLy AS MaNguoiDung, QL.HoTen, QL.CCCD, QL.Gmail, QL.SoDienThoai, QL.DiaChi, TK.VaiTro, TK.TrangThai
                    FROM TAIKHOAN TK
                    JOIN QUANLY QL ON TK.MaLienKet = QL.MaQuanLy
                    WHERE TK.TenDangNhap = @user AND TK.MatKhau = @pass AND TK.TrangThai = 1";
                    }
                    else return null;

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@pass", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var nv = new ThongTinNhanVien();

                                nv.MaNhanVien = reader.GetInt32(reader.GetOrdinal("MaNguoiDung"));
                                nv.HoTen = reader["HoTen"]?.ToString();
                                nv.CCCD = reader["CCCD"]?.ToString();
                                nv.Gmail = reader["Gmail"]?.ToString();
                                nv.DienThoai = reader["SoDienThoai"]?.ToString();
                                nv.DiaChi = reader["DiaChi"]?.ToString();
                                nv.VaiTro = reader["VaiTro"]?.ToString();
                                nv.TrangThai = (bool)reader["TrangThai"];

                                return nv;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng nhập: " + ex.Message);
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

        public static DataTable LayDanhSachVe()
        {
            string query = @"
        SELECT 
            v.MaVe,
            v.MaChuyen,
            v.SoGhe,
            v.LoaiGhe,
            v.MaKhachHang,
            v.GiaTien,
            v.TenNguoiSoHuu,
            v.SoDienThoai,
            v.CCCD,
            FORMAT(v.NgayDat, 'dd/MM/yyyy') AS NgayDat
        FROM VE v
        ORDER BY v.MaVe ASC";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }
        public static DataRow LayThongTinVe(int maVe)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"
                SELECT 
                    c.MaChuyen,
                    c.NoiDi,
                    c.NoiDen,
                    c.GioDi,
                    c.GioDen,
                    c.NgayDi,
                    l.PhuongThucThanhToan
                FROM VE v
                INNER JOIN CHUYENTAU c ON v.MaChuyen = c.MaChuyen
                INNER JOIN LICHSUGIAODICH l ON v.MaKhachHang = l.MaKhachHang
                WHERE v.MaVe = @MaVe";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaVe", maVe);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
                else
                    return null;
            }
        }
        public static bool CapNhatThongTinKhachHang(int maVe, string tenNguoi, string soDienThoai, string cccd)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"
                UPDATE VE
                SET 
                    TenNguoiSoHuu = @TenNguoi,
                    SoDienThoai = @SoDienThoai,
                    CCCD = @CCCD
                WHERE MaVe = @MaVe";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenNguoi", tenNguoi);
                cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                cmd.Parameters.AddWithValue("@CCCD", cccd);
                cmd.Parameters.AddWithValue("@MaVe", maVe);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();

                return rows > 0;
            }
        }
        // sửa chuyến tàu
        public static bool SuaChuyenTau(
        int maChuyen,
        string noiDi,
        string noiDen,
        TimeSpan gioDi,
        TimeSpan gioDen,
        DateTime ngayDi,
        int tongSoGhe,
        decimal giaGheMem,
        decimal giaGheCung,
        int nguoiTao
        )
        {
            string query = @"
        UPDATE CHUYENTAU
        SET NoiDi = @NoiDi,
            NoiDen = @NoiDen,
            GioDi = @GioDi,
            GioDen = @GioDen,
            NgayDi = @NgayDi,
            TongSoGhe = @TongSoGhe,
            GiaGheMem = @GiaGheMem,
            GiaGheCung = @GiaGheCung,
            NguoiTao = @NguoiTao
        WHERE MaChuyen = @MaChuyen";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaChuyen", maChuyen);
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
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0; // Trả về true nếu có dòng được cập nhật
            }
        }
        public static DataTable LayTatCa()
        {
            DataTable dt = new DataTable();

            string query = @"
            SELECT 
                MaGiaoDich,
                PhuongThucThanhToan,
                FORMAT(TongTien, 'N0') AS TongTien,
                FORMAT(ThoiGianDat, 'dd/MM/yyyy') AS ThoiGianDat
            FROM LICHSUGIAODICH
            ORDER BY ThoiGianDat DESC";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;

        }

        public static DataTable LayDoanhThuTheoNgay()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"
                SELECT 
                    CONVERT(date, ThoiGianDat) AS Ngay,
                    SUM(TongTien) AS DoanhThu
                FROM LICHSUGIAODICH
                GROUP BY CONVERT(date, ThoiGianDat)
                ORDER BY Ngay";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        public static DataTable LayBaoCaoVeTheoNgay()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"
                SELECT 
                    CONVERT(date, v.NgayDat) AS NgayDat,
                    COUNT(v.MaVe) AS SoLuongVe,
                    SUM(CASE WHEN v.LoaiGhe = N'GheMem' THEN 1 ELSE 0 END) AS GheMem,
                    SUM(CASE WHEN v.LoaiGhe = N'GheCung' THEN 1 ELSE 0 END) AS GheCung
                FROM VE v
                GROUP BY CONVERT(date, v.NgayDat)
                ORDER BY NgayDat";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        

        }

        public static DataTable LayLichSuTheoKhach()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"
                SELECT 
                    MaGiaoDich,
                    PhuongThucThanhToan,
                    TongTien,
                    ThoiGianDat
                FROM LICHSUGIAODICH

                ORDER BY ThoiGianDat DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        public static DataTable LayLichSuTheoNgay( DateTime tuNgay, DateTime denNgay)
        {
            DataTable dt = new DataTable();
            string query = @"
            SELECT 
                MaGiaoDich,
                PhuongThucThanhToan,
                FORMAT(TongTien, 'N0') AS TongTien,
                FORMAT(ThoiGianDat, 'dd/MM/yyyy') AS ThoiGianDat
            FROM LICHSUGIAODICH
            WHERE ThoiGianDat BETWEEN @TuNgay AND @DenNgay
            ORDER BY ThoiGianDat DESC";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay.AddDays(1).AddTicks(-1)); // lấy đủ hết ngày
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        // lấy thông tin chuyến tàu 
        public static DataTable LayThongTinChuyenTau(int maChuyen)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT MaChuyen, NoiDi, NoiDen, GioDi, GioDen, NgayDi, 
                                    TongSoGhe, GiaGheMem, GiaGheCung, NguoiTao
                             FROM CHUYENTAU
                             WHERE MaChuyen = @MaChuyen";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaChuyen", maChuyen);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy thông tin chuyến tàu: " + ex.Message);
            }

            return dt;
        }

    }

}
