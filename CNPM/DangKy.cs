using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    public partial class DangKy : Form
    {
        public DangKy()
        {
            InitializeComponent();
        }

        private void lb_Closed_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_TaoTK_Click(object sender, EventArgs e)
        {
            if (!IsInputValid()) return;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                // Dùng transaction để đảm bảo 2 thao tác thêm là đồng bộ
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1️⃣ Thêm khách hàng mới và lấy MaKhachHang vừa được tạo
                    string insertKhachHangQuery = @"
                INSERT INTO KhachHang (HoTen, CCCD, NgaySinh, SoDienThoai, NgayTao, TrangThai)
                OUTPUT INSERTED.MaKhachHang
                VALUES (@HoTen, @CCCD, @NgaySinh, @SoDienThoai, GETDATE(), N'Hoạt Động');";

                    SqlCommand cmdKhach = new SqlCommand(insertKhachHangQuery, conn, transaction);
                    cmdKhach.Parameters.AddWithValue("@HoTen", txt_Ten.Text.Trim());
                    cmdKhach.Parameters.AddWithValue("@CCCD", txt_CCCD.Text.Trim());
                    cmdKhach.Parameters.AddWithValue("@NgaySinh", date_NgaySinh.Value);
                    cmdKhach.Parameters.AddWithValue("@SoDienThoai", txt_SDT.Text.Trim());

                    int maKhachHangMoi = (int)cmdKhach.ExecuteScalar();

                    // 2️⃣ Thêm tài khoản và gán MaLienKet = MaKhachHang vừa tạo
                    string insertTaiKhoanQuery = @"
                INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro, MaLienKet, TrangThai)
                VALUES (@TenDangNhap, @MatKhau, 'KhachHang', @MaLienKet, 1);";

                    SqlCommand cmdTK = new SqlCommand(insertTaiKhoanQuery, conn, transaction);
                    cmdTK.Parameters.AddWithValue("@TenDangNhap", txt_username.Text.Trim());
                    cmdTK.Parameters.AddWithValue("@MatKhau", txt_Pass.Text.Trim());
                    cmdTK.Parameters.AddWithValue("@MaLienKet", maKhachHangMoi);

                    cmdTK.ExecuteNonQuery();

                    // 3️⃣ Xác nhận transaction
                    transaction.Commit();

                    MessageBox.Show("Tạo tài khoản thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Lỗi khi tạo tài khoản: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //hàm kiểm tra thông tin nhập vào
        private bool IsInputValid()
        {
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(txt_Ten.Text) ||
                string.IsNullOrWhiteSpace(txt_CCCD.Text) ||
                string.IsNullOrWhiteSpace(txt_SDT.Text) ||
                string.IsNullOrWhiteSpace(txt_username.Text) ||
                string.IsNullOrWhiteSpace(txt_Pass.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin bắt buộc.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // kiểm tra ngày sinh
            if (date_NgaySinh.Value >= DateTime.Now)
            {
                MessageBox.Show("Ngày sinh không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Kiểm tra cccd trùng trong bảng khách hàng và username trùng trong bảng tài khoản
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM KhachHang WHERE CCCD = @CCCD";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CCCD", txt_CCCD.Text.Trim());
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("CCCD đã tồn tại. Vui lòng kiểm tra lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap = @TenDangNhap";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenDangNhap", txt_username.Text.Trim());
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }
    }
}
