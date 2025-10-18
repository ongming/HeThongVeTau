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
                      AND (VaiTro = 'NhanVien'
                      OR  VaiTro = 'QuanLy')
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
    }
}
