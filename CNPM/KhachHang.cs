using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    public partial class KhachHang : UserControl
    {
        public KhachHang()
        {
            InitializeComponent();
        }

        public event EventHandler SwitchToRole;
        public event EventHandler SwitchToDangKy;
        private bool isEyeOpen = false;

        private void lb_back_Click(object sender, EventArgs e)
        {
            SwitchToRole?.Invoke(this, EventArgs.Empty);
        }

        private void lb_QuenPass_Click(object sender, EventArgs e)
        {
            ForgotPass forgotForm = new ForgotPass();

            // Lấy form chứa UserControl này (Login)
            Form parentForm = this.FindForm();

            if (parentForm != null)
            {
                forgotForm.Owner = parentForm; // ✅ gán Login làm chủ sở hữu
                parentForm.Hide();              // Ẩn Login
                forgotForm.Show();              // Mở ForgotPass
                parentForm.Show();
            }
            else
            {
                // fallback - nếu không có form cha
                forgotForm.Show();
            }
        }

        private void btn_DangNhap_Click(object sender, EventArgs e)
        {
            string user = txt_username.Text.Trim();
            string pass = txt_Pass.Text.Trim();
            NhanVienRepository repo = new NhanVienRepository();
            // Lấy form chứa usercontrol (ở đây là FormLogin)
            Form parentForm = this.FindForm();

            if (repo.CheckLogin(user, pass))
            {
                // Ẩn form login
                parentForm.Hide();

                // Mở form khách hàng
                KhachHangTuongTac fKhach = new KhachHangTuongTac();

                // Khi form Khách hàng đóng → hiện lại form login
                fKhach.FormClosed += (s2, e2) => parentForm.Show();
                fKhach.Show();
            }
            else
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu");
            }
        }

        private void lb_DangKy_Click(object sender, EventArgs e)
        {
            DangKy dangKyForm = new DangKy();
            dangKyForm.ShowDialog();
        }

        private void txt_Pass_IconRightClick(object sender, EventArgs e)
        {
            // Đảo trạng thái
            isEyeOpen = !isEyeOpen;

            if (isEyeOpen)
            {
                txt_Pass.IconRight = Properties.Resources.witness_1518564; // icon mới
                txt_Pass.UseSystemPasswordChar = false; // tắt chế độ ẩn hệ thống
                txt_Pass.PasswordChar = '\0'; // hiển thị text
            }
            else
            {
                txt_Pass.IconRight = Properties.Resources.invisible_98494; // icon cũ
                txt_Pass.UseSystemPasswordChar = false; // dùng char riêng (nếu cần)
                txt_Pass.PasswordChar = '•'; // ẩn text
            }
        }
    }
}
