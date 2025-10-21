using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    public class KhachHangRepository
    {
        //kiểm tra đăng nhập
        public ThongTinKhachHang CheckLogin(string username, string password)
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"
                    SELECT kh.MaKhachHang, kh.HoTen, kh.Gmail, kh.SoDienThoai, kh.DiaChi, kh.CCCD, t.TrangThai
                    FROM TAIKHOAN t
                    JOIN KHACHHANG kh ON t.MaLienKet = kh.MaKhachHang
                    WHERE t.TenDangNhap = @user 
                      AND t.MatKhau = @pass 
                      AND t.VaiTro = 'KhachHang'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@pass", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bool trangThai = reader.GetBoolean(6);                                // cột 7: TrangThai

                                if (!trangThai)
                                {
                                    MessageBox.Show("Tài khoản của bạn đã bị chặn.",
                                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return null;
                                }

                                // Nếu hoạt động bình thường
                                return new ThongTinKhachHang
                                {
                                    MaKhachHang = reader.GetInt32(0),
                                    HoTen = reader.GetString(1),
                                    Gmail = reader.GetString(2),
                                    DienThoai = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    DiaChi = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    CCCD = reader.IsDBNull(5) ? "" : reader.GetString(5)
                                };
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu!\nChi tiết: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi đăng nhập!\nChi tiết: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MessageBox.Show("Sai tài khoản hoặc mật khẩu!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            // 🔹 Nếu không có kết quả nào, trả về null
            return null;
        }


        // đặt lại mật khẩu theo gmail
        public static void DatLaiMatKhau(string gmail)
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                UPDATE TAIKHOAN SET MatKhau = '123'
                WHERE MaTaiKhoan IN (
                    SELECT t.MaTaiKhoan FROM TAIKHOAN t
                    LEFT JOIN KHACHHANG kh ON t.MaLienKet = kh.MaKhachHang AND t.VaiTro = 'KhachHang'
                    LEFT JOIN NHANVIEN nv ON t.MaLienKet = nv.MaNhanVien AND t.VaiTro = 'NhanVien'
                    LEFT JOIN QUANLY ql ON t.MaLienKet = ql.MaQuanLy AND t.VaiTro = 'QuanLy'
                    WHERE kh.Gmail = @gmail OR nv.Gmail = @gmail OR ql.Gmail = @gmail
                )";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gmail", gmail);
                        int rows = cmd.ExecuteNonQuery();

                        MessageBox.Show(rows > 0 ? "✅ Đặt lại mật khẩu thành công!" : "❌ Không tìm thấy Gmail!");
                    }
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("❌ Lỗi khi kết nối hoặc cập nhật dữ liệu!");
            }
        }

        // hàm đặt vé khi nhấn nút hoàn tất
        public static bool DatVe(
             int maKhachHang,
             int maChuyen,
             List<int> gheList,
             List<NguoiSuDungVe> nguoiSuDungList,
             decimal tongTien,
             string phuongThucThanhToan,
             decimal GheCung,
             decimal GheMem
            )
        {
            if (gheList.Count != nguoiSuDungList.Count)
            {
                MessageBox.Show("Số ghế và số người sử dụng không khớp!");
                return false;
            }

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // 🔹 1. Thêm vào LICHSUGIAODICH (tổng giao dịch)
                    string sqlGD = @"
                INSERT INTO LICHSUGIAODICH (MaKhachHang, ThoiGianDat, PhuongThucThanhToan, TongTien)
                OUTPUT INSERTED.MaGiaoDich
                VALUES (@makh, GETDATE(), @pttt, @tong)";

                    int maGiaoDich;
                    using (SqlCommand cmd = new SqlCommand(sqlGD, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@makh", maKhachHang);
                        cmd.Parameters.AddWithValue("@pttt", phuongThucThanhToan);
                        cmd.Parameters.AddWithValue("@tong", tongTien);
                        maGiaoDich = (int)cmd.ExecuteScalar();
                    }

                    // 🔹 2. Duyệt từng ghế để thêm vào CHITIETGIAODICH và VE


                    string sqlVe = @"
                INSERT INTO VE (MaGiaoDich, MaChuyen, MaKhachHang, SoGhe, LoaiGhe, GiaTien, TenNguoiSoHuu, SoDienThoai, CCCD, NgayDat)
                VALUES (@magd, @machuyen, @makh, @soghe, @loaighe, @gia, @ten, @sdt, @cccd, GETDATE())";

                    for (int i = 0; i < gheList.Count; i++)
                    {
                        int ghe = gheList[i];
                        var nguoi = nguoiSuDungList[i];

                        string loaiGhe = ghe > 20 ? "GheCung" : "GheMem";
                        decimal giaTien = ghe > 20 ? GheCung : GheMem;


                        // 🔸 Thêm vé tương ứng
                        using (SqlCommand cmdVe = new SqlCommand(sqlVe, conn, trans))
                        {
                            cmdVe.Parameters.AddWithValue("@machuyen", maChuyen);
                            cmdVe.Parameters.AddWithValue("@makh", maKhachHang);
                            cmdVe.Parameters.AddWithValue("@soghe", ghe);
                            cmdVe.Parameters.AddWithValue("@magd", maGiaoDich);
                            cmdVe.Parameters.AddWithValue("@loaighe", loaiGhe);
                            cmdVe.Parameters.AddWithValue("@gia", giaTien);
                            cmdVe.Parameters.AddWithValue("@ten", nguoi.TenNguoiSuDung);
                            cmdVe.Parameters.AddWithValue("@sdt", nguoi.SoDienThoai);
                            cmdVe.Parameters.AddWithValue("@cccd", nguoi.CCCD);
                            cmdVe.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("❌ Lỗi khi lưu giao dịch: " + ex.Message);
                    return false;
                }
            }
        }
        public static bool GuiYKien(int maKhachHang, int danhGia, string noiDung)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                try
                {
                    // 🔹 1. Kiểm tra xem hôm nay KH đã gửi chưa
                    string sqlCheck = @"
                    SELECT COUNT(*) 
                    FROM YKIENPHANHOI 
                    WHERE MaKhachHang = @makh 
                      AND CAST(NgayGopY AS DATE) = CAST(GETDATE() AS DATE)";

                    using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@makh", maKhachHang);
                        int count = (int)cmdCheck.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("❌ Bạn đã gửi ý kiến hôm nay rồi!\nVui lòng quay lại vào ngày mai.",
                                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    // 🔹 2. Thêm ý kiến mới
                    string sqlInsert = @"
                    INSERT INTO YKIENPHANHOI (MaKhachHang, NgayGopY, DanhGia, NoiDung)
                    VALUES (@makh, GETDATE(), @danhgia, @noidung)";

                    using (SqlCommand cmd = new SqlCommand(sqlInsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@makh", maKhachHang);
                        cmd.Parameters.AddWithValue("@danhgia", danhGia);
                        cmd.Parameters.AddWithValue("@noidung", noiDung);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("✅ Cảm ơn bạn đã gửi ý kiến phản hồi!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Lỗi khi gửi ý kiến: " + ex.Message);
                    return false;
                }
            }
        }
        public static DataTable LayLichSuTheoKhach(int maKhachHang)
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
                WHERE MaKhachHang = @makh
                ORDER BY ThoiGianDat DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@makh", maKhachHang);

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        public static DataTable LayChiTietGiaoDich(int maGiaoDich)
        {
            DataTable dt = new DataTable();
            string query = @"
                        SELECT 
                            c.NoiDi,
                            c.NoiDen,
                            FORMAT(c.NgayDi, 'dd/MM/yyyy') AS NgayDi,
                            LEFT(CONVERT(VARCHAR(8), c.GioDi, 108), 5) AS GioDi,  -- ✅ FIX: giờ đi
                            LEFT(CONVERT(VARCHAR(8), c.GioDen, 108), 5) AS GioDen,
                            v.SoGhe,
                            v.LoaiGhe,
                            FORMAT(v.GiaTien, 'N0') AS GiaTien,
                            v.TenNguoiSoHuu,
                            v.SoDienThoai,
                            v.CCCD
                        FROM VE v
                        INNER JOIN CHUYENTAU c ON v.MaChuyen = c.MaChuyen
                        WHERE v.MaGiaoDich = @MaGiaoDich";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaGiaoDich", maGiaoDich);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }
        public static DataTable LayLichSuTheoNgay(int maKhachHang, DateTime tuNgay, DateTime denNgay)
        {
            DataTable dt = new DataTable();
            string query = @"
            SELECT 
                MaGiaoDich,
                PhuongThucThanhToan,
                FORMAT(TongTien, 'N0') AS TongTien,
                FORMAT(ThoiGianDat, 'dd/MM/yyyy') AS ThoiGianDat
            FROM LICHSUGIAODICH
            WHERE MaKhachHang = @MaKH 
              AND ThoiGianDat BETWEEN @TuNgay AND @DenNgay
            ORDER BY ThoiGianDat DESC";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaKH", maKhachHang);
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay.AddDays(1).AddTicks(-1)); // lấy đủ hết ngày
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        public static DataTable LayTatCa(int maKhachHang)
        {
            DataTable dt = new DataTable();

            string query = @"
            SELECT 
                MaGiaoDich,
                PhuongThucThanhToan,
                FORMAT(TongTien, 'N0') AS TongTien,
                FORMAT(ThoiGianDat, 'dd/MM/yyyy') AS ThoiGianDat
            FROM LICHSUGIAODICH
            WHERE MaKhachHang = @MaKH
            ORDER BY ThoiGianDat DESC";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaKH", maKhachHang);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }
        public static DataTable LayThongTin(int maKhachHang)
        {
            DataTable dt = new DataTable();

            string query = @"
                SELECT 
                    kh.HoTen,
                    kh.CCCD,
                    kh.NgaySinh,
                    kh.DiaChi,
                    kh.SoDienThoai,
                    kh.Gmail,
                    kh.NgayTao,
                    kh.TrangThai,
                    kh.Avatar,
                    tk.TenDangNhap,
                    tk.MatKhau
                FROM KHACHHANG kh
                INNER JOIN TAIKHOAN tk ON tk.MaLienKet = kh.MaKhachHang
                WHERE kh.MaKhachHang = @MaKhachHang
                      AND tk.VaiTro = 'KhachHang'";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }
        public static void CapNhatThongTin(
        int maKhachHang,
        string hoTen,
        string cccd,
        DateTime ngaySinh,
        string diaChi,
        string soDienThoai,
        string gmail,
        string tenDangNhap,
        byte[] avatarData = null)
            {
            string query = @"
            BEGIN TRANSACTION;

            -- 🧩 Cập nhật thông tin KHACHHANG
            UPDATE KHACHHANG
            SET 
                HoTen = @HoTen,
                CCCD = @CCCD,
                NgaySinh = @NgaySinh,
                DiaChi = @DiaChi,
                SoDienThoai = @SoDienThoai,
                Gmail = @Gmail
                " + (avatarData != null ? ", Avatar = @Avatar" : "") + @"
            WHERE MaKhachHang = @MaKhachHang;

            -- 🔐 Cập nhật tên đăng nhập
            UPDATE TAIKHOAN
            SET TenDangNhap = @TenDangNhap
            WHERE MaLienKet = @MaKhachHang AND VaiTro = 'KhachHang';

            COMMIT TRANSACTION;
        ";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                cmd.Parameters.AddWithValue("@HoTen", hoTen);
                cmd.Parameters.AddWithValue("@CCCD", cccd);
                cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                cmd.Parameters.AddWithValue("@Gmail", gmail);
                cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);

                if (avatarData != null)
                    cmd.Parameters.Add("@Avatar", SqlDbType.VarBinary).Value = avatarData;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // 🔹 Kiểm tra mật khẩu cũ của khách hàng
        public static bool KiemTraMatKhauCu(int maKhachHang, string matKhauCu)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM TAIKHOAN 
                WHERE MaLienKet = @MaKhachHang 
                      AND VaiTro = 'KhachHang' 
                      AND MatKhau = @MatKhauCu";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                cmd.Parameters.AddWithValue("@MatKhauCu", matKhauCu);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        // 🔹 Cập nhật mật khẩu mới cho khách hàng
        public static void DoiMatKhau(int maKhachHang, string matKhauMoi)
        {
            string query = @"
                UPDATE TAIKHOAN
                SET MatKhau = @MatKhauMoi
                WHERE MaLienKet = @MaKhachHang AND VaiTro = 'KhachHang'";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                cmd.Parameters.AddWithValue("@MatKhauMoi", matKhauMoi);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public static DataTable LayThongBaoTheoKhachHang(int maKhachHang)
        {
            DataTable dt = new DataTable();
            string query = @"
            SELECT 
                MaThongBao,
                NoiDung,
                FORMAT(ThoiGian, 'dd/MM/yyyy HH:mm') AS ThoiGian,
                DaXem
            FROM THONGBAO_KH
            WHERE MaKhachHang IS NULL OR MaKhachHang = 0 OR MaKhachHang = @MaKhachHang
            ORDER BY ThoiGian DESC";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        public static bool CoThongBaoChuaXem(int maKhachHang)
        {
            string query = @"
        SELECT COUNT(*) 
        FROM THONGBAO_KH
        WHERE (MaKhachHang = @MaKhachHang OR MaKhachHang IS NULL)
              AND DaXem = 0";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }
        public static void DanhDauDaXem(int maKhachHang)
        {
            string query = @"
        UPDATE THONGBAO_KH
        SET DaXem = 1
        WHERE MaKhachHang = @MaKhachHang OR MaKhachHang IS NULL";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public static void GhiThongBaoDangNhap(int maKhachHang)
        {
            string query = @"
        INSERT INTO THONGBAO_KH (NoiDung, MaKhachHang, ThoiGian, DaXem)
        VALUES (@NoiDung, @MaKhachHang, @ThoiGian, 0)";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                string noiDung = $"Bạn đã đăng nhập vào lúc {DateTime.Now:HH:mm:ss} ngày {DateTime.Now:dd/MM/yyyy}.";
                cmd.Parameters.AddWithValue("@NoiDung", noiDung);
                cmd.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                cmd.Parameters.AddWithValue("@ThoiGian", DateTime.Now);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
