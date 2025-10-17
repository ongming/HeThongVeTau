using System;
using System.Collections.Generic;
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
        public bool CheckLogin(string username, string password)
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"
                    SELECT COUNT(*) 
                    FROM TAIKHOAN 
                    WHERE TenDangNhap = @user 
                      AND MatKhau = @pass 
                      AND VaiTro = 'KhachHang'
                      AND TrangThai = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@pass", password);

                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu!\nChi tiết: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi đăng nhập!\nChi tiết: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
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
        public static bool DatVe(int maKhachHang, int maChuyen, List<int> gheList, decimal tongTien)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // 1. Thêm giao dịch
                    string sqlGD = "INSERT INTO LICHSUGIAODICH(MaKhachHang, NgayGD, TongTien) OUTPUT INSERTED.MaGiaoDich VALUES(@makh, GETDATE(), @tong)";
                    int maGD;
                    using (SqlCommand cmd = new SqlCommand(sqlGD, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@makh", maKhachHang);
                        cmd.Parameters.AddWithValue("@tong", tongTien);
                        maGD = (int)cmd.ExecuteScalar();
                    }

                    // 2. Thêm vé
                    string sqlVe = "INSERT INTO VE(MaChuyen, MaKhachHang, SoGhe, MaGiaoDich) VALUES(@machuyen, @makh, @soghe, @magd)";
                    foreach (int ghe in gheList)
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlVe, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@machuyen", maChuyen);
                            cmd.Parameters.AddWithValue("@makh", maKhachHang);
                            cmd.Parameters.AddWithValue("@soghe", ghe);
                            cmd.Parameters.AddWithValue("@magd", maGD);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // 3. Commit nếu thành công
                    trans.Commit();
                    return true;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    return false;
                }
            }
        }

    }
}
