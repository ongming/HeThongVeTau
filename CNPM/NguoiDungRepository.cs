using System;
using System.Data;
using System.Data.SqlClient;

namespace CNPM
{
    public static class NguoiDungRepository
    {
        /// <summary>
        /// Lấy thông tin người dùng theo vai trò
        /// </summary>
        public static DataTable LayThongTin(int maNguoiDung, string role)
        {
            DataTable dt = new DataTable();
            string query = "";

            if (role == "KhachHang")
            {
                query = @"
                    SELECT 
                        kh.HoTen,
                        kh.CCCD,
                        kh.NgaySinh,
                        kh.DiaChi,
                        kh.SoDienThoai,
                        kh.Gmail,
                        kh.NgayTao,
                        kh.TrangThai,
                        tk.TenDangNhap,
                        tk.MatKhau,
                        kh.Avatar
                    FROM KHACHHANG kh
                    INNER JOIN TAIKHOAN tk ON tk.MaLienKet = kh.MaKhachHang
                    WHERE kh.MaKhachHang = @MaNguoiDung
                          AND tk.VaiTro = 'KhachHang'";
            }
            else if (role == "NhanVien")
            {
                query = @"
                    SELECT 
                        nv.HoTen,
                        nv.CCCD,
                        nv.NgaySinh,
                        nv.DiaChi,
                        nv.SoDienThoai,
                        nv.Gmail,
                        nv.NgayVaoLam,
                        tk.TenDangNhap,
                        tk.MatKhau,
                        nv.Avatar
                    FROM NHANVIEN nv
                    INNER JOIN TAIKHOAN tk ON tk.MaLienKet = nv.MaNhanVien
                    WHERE nv.MaNhanVien = @MaNguoiDung
                          AND tk.VaiTro = 'NhanVien'";
            }
            else if (role == "QuanLy")
            {
                query = @"
                    SELECT 
                        ql.HoTen,
                        ql.CCCD,
                        ql.NgaySinh,
                        ql.DiaChi,
                        ql.SoDienThoai,
                        ql.Gmail,
                        tk.TenDangNhap,
                        tk.MatKhau,
                        ql.Avatar
                    FROM QUANLY ql
                    INNER JOIN TAIKHOAN tk ON tk.MaLienKet = ql.MaQuanLy
                    WHERE ql.MaQuanLy = @MaNguoiDung
                          AND tk.VaiTro = 'QuanLy'";
            }

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaNguoiDung", maNguoiDung);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        /// <summary>
        /// Cập nhật thông tin người dùng (Khách hàng / Nhân viên / Quản lý)
        /// </summary>
        public static void CapNhatThongTin(
            int maNguoiDung,
            string role,
            string hoTen,
            string cccd,
            DateTime ngaySinh,
            string diaChi,
            string soDienThoai,
            string gmail,
            string tenDangNhap,
            byte[] avatarData = null)
        {
            string table;
            string idField;
            string vaiTroCheck;

            if (role == "KhachHang")
            {
                table = "KHACHHANG";
                idField = "MaKhachHang";
                vaiTroCheck = "KhachHang";
            }
            else if (role == "NhanVien")
            {
                table = "NHANVIEN";
                idField = "MaNhanVien";
                vaiTroCheck = "NhanVien";
            }
            else
            {
                table = "QUANLY";
                idField = "MaQuanLy";
                vaiTroCheck = "QuanLy";
            }

            string query = $@"
                UPDATE {table}
                SET 
                    HoTen = @HoTen,
                    CCCD = @CCCD,
                    NgaySinh = @NgaySinh,
                    DiaChi = @DiaChi,
                    SoDienThoai = @SoDienThoai,
                    Gmail = @Gmail
                    {(avatarData != null ? ", Avatar = @Avatar" : "")}
                WHERE {idField} = @MaNguoiDung;

                UPDATE TAIKHOAN
                SET TenDangNhap = @TenDangNhap
                WHERE MaLienKet = @MaNguoiDung AND VaiTro = @VaiTro;
            ";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaNguoiDung", maNguoiDung);
                cmd.Parameters.AddWithValue("@HoTen", hoTen);
                cmd.Parameters.AddWithValue("@CCCD", cccd);
                cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                cmd.Parameters.AddWithValue("@DiaChi", diaChi);
                cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);
                cmd.Parameters.AddWithValue("@Gmail", gmail);
                cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                cmd.Parameters.AddWithValue("@VaiTro", vaiTroCheck);

                if (avatarData != null)
                    cmd.Parameters.Add("@Avatar", SqlDbType.VarBinary).Value = avatarData;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // 🔹 Kiểm tra mật khẩu cũ có đúng không
        public static bool KiemTraMatKhauCu(int maNguoiDung, string matKhauCu, string vaiTro)
        {
            string query = @"
            SELECT COUNT(*) 
            FROM TAIKHOAN 
            WHERE MaLienKet = @MaLienKet 
                  AND VaiTro = @VaiTro 
                  AND MatKhau = @MatKhauCu";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaLienKet", maNguoiDung);
                cmd.Parameters.AddWithValue("@VaiTro", vaiTro);
                cmd.Parameters.AddWithValue("@MatKhauCu", matKhauCu);

                conn.Open();
                object result = cmd.ExecuteScalar();
                int count = (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                return count > 0;
            }
        }

        // 🔹 Đổi mật khẩu mới
        public static bool DoiMatKhau(int maNguoiDung, string matKhauMoi, string vaiTro)
        {
            string query = @"
            UPDATE TAIKHOAN
            SET MatKhau = @MatKhauMoi
            WHERE MaLienKet = @MaLienKet AND VaiTro = @VaiTro";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaLienKet", maNguoiDung);
                cmd.Parameters.AddWithValue("@MatKhauMoi", matKhauMoi);
                cmd.Parameters.AddWithValue("@VaiTro", vaiTro);

                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0; // true nếu đổi thành công
            }
        }

        // ghi thông báo 
        public static void GhiThongBao(string noiDung, string role, int? maNguoi = null)
        {
            string connectionString = "Data Source=.;Initial Catalog=BanVeTau;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query;

                if (role == "KhachHang")
                {
                    // 🔔 Thông báo khách hàng
                    query = @"
                    INSERT INTO THONGBAO_KH (NoiDung, MaKhachHang, ThoiGian, DaXem)
                    VALUES (@NoiDung, @MaNguoi, GETDATE(), 0)";
                }
                else
                {
                    // 🔔 Thông báo nhân viên hoặc quản lý
                    query = @"
                    INSERT INTO THONGBAO_NV (NoiDung, MaNguoiNhan, Role, ThoiGian, DaXem)
                    VALUES (@NoiDung, @MaNguoi, @Role, GETDATE(), 0)";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NoiDung", noiDung);
                    cmd.Parameters.AddWithValue("@Role", role);

                    if (maNguoi.HasValue)
                        cmd.Parameters.AddWithValue("@MaNguoi", maNguoi.Value);
                    else
                        cmd.Parameters.AddWithValue("@MaNguoi", DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }   
}
