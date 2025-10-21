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
    public partial class ThemNhanVien : Form
    {
        ThongTinNhanVien nv;
        public ThemNhanVien(ThongTinNhanVien nv)
        {
            InitializeComponent();
            this.nv = nv;
        }

        private void lb_Closed_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_ThemChuyen_Click(object sender, EventArgs e)
        {
            if (!IsInputValid()) return;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction(); // đảm bảo an toàn dữ liệu

                try
                {
                    // 1️⃣ Thêm nhân viên mới
                    string insertNV = @"
                INSERT INTO NHANVIEN (HoTen, CCCD, NgaySinh, DiaChi, SoDienThoai, Gmail, NgayVaoLam, MaQuanLy)
                OUTPUT INSERTED.MaNhanVien
                VALUES (@HoTen, @CCCD, @NgaySinh, @DiaChi, @SoDienThoai, @Gmail, GETDATE(), @MaQuanLy)";

                    SqlCommand cmdNV = new SqlCommand(insertNV, conn, transaction);
                    cmdNV.Parameters.AddWithValue("@HoTen", txt_HovaTen.Text);
                    cmdNV.Parameters.AddWithValue("@CCCD", txt_CCCD.Text);
                    cmdNV.Parameters.AddWithValue("@NgaySinh", date_NgaySinh.Value);
                    cmdNV.Parameters.AddWithValue("@DiaChi", txt_DiaChi.Text);
                    cmdNV.Parameters.AddWithValue("@SoDienThoai", txt_SDT.Text);
                    cmdNV.Parameters.AddWithValue("@Gmail", txt_Email.Text);
                    cmdNV.Parameters.AddWithValue("@MaQuanLy", nv.MaNhanVien); // 👈 ID quản lý đang đăng nhập

                    int maNhanVienMoi = Convert.ToInt32(cmdNV.ExecuteScalar());

                    // 2️⃣ Tự động tạo tài khoản cho nhân viên
                    string tenDangNhap = txt_TenDangNhap.Text;
                    string matKhauMacDinh = "123456";

                    string insertTK = @"
                INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, VaiTro, MaLienKet, TrangThai)
                VALUES (@TenDangNhap, @MatKhau, 'NhanVien', @MaLienKet, 1)";

                    SqlCommand cmdTK = new SqlCommand(insertTK, conn, transaction);
                    cmdTK.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                    cmdTK.Parameters.AddWithValue("@MatKhau", matKhauMacDinh);
                    cmdTK.Parameters.AddWithValue("@MaLienKet", maNhanVienMoi);
                    cmdTK.ExecuteNonQuery();

                    // 3️⃣ Ghi nhật ký hoạt động
                    string insertNK = @"
                INSERT INTO NHATKY_HOATDONG (MaQuanLy, HanhDong, ThoiGian)
                VALUES (@MaQuanLy, @HanhDong, @ThoiGian)";

                    SqlCommand cmdNK = new SqlCommand(insertNK, conn, transaction);
                    cmdNK.Parameters.AddWithValue("@MaQuanLy", nv.MaNhanVien);
                    cmdNK.Parameters.AddWithValue("@HanhDong",
                        $"Thêm nhân viên mới: {txt_HovaTen.Text} (Tài khoản: {tenDangNhap})");
                    cmdNK.Parameters.AddWithValue("@ThoiGian", DateTime.Now);
                    cmdNK.ExecuteNonQuery();

                    // 4️⃣ Xác nhận giao dịch
                    transaction.Commit();

                    MessageBox.Show($"Thêm nhân viên thành công!\nTài khoản: {tenDangNhap}\nMật khẩu: 123456",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("❌ Lỗi khi thêm nhân viên: " + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private bool IsInputValid()
        {
            // 1️⃣ Kiểm tra ô trống
            if (string.IsNullOrWhiteSpace(txt_TenDangNhap.Text) ||
                string.IsNullOrWhiteSpace(txt_HovaTen.Text) ||
                string.IsNullOrWhiteSpace(txt_CCCD.Text) ||
                string.IsNullOrWhiteSpace(txt_SDT.Text) ||
                string.IsNullOrWhiteSpace(txt_Email.Text) ||
                string.IsNullOrWhiteSpace(txt_DiaChi.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            // 3️⃣ Kiểm tra định dạng email
            if (!System.Text.RegularExpressions.Regex.IsMatch(txt_Email.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không hợp lệ.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // 4️⃣ Kiểm tra trùng tên đăng nhập
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM TAIKHOAN WHERE TenDangNhap = @TenDangNhap";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TenDangNhap", txt_TenDangNhap.Text.Trim());
                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.", "Trùng tên", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            //kiểm tra trùng cccd
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM NHANVIEN WHERE CCCD = @CCCD";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CCCD", txt_CCCD.Text.Trim());
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("CCCD đã tồn tại. Vui lòng kiểm tra lại.", "Trùng CCCD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
