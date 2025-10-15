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
    public partial class NhanVien : UserControl
    {
        public NhanVien()
        {
            InitializeComponent();
        }
        public event EventHandler SwitchToRole;
        private bool isEyeOpen = false;

        private void pass_IconRightClick(object sender, EventArgs e)
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

        private void lb_back_Click(object sender, EventArgs e)
        {
            if (SwitchToRole != null)
                SwitchToRole(this, EventArgs.Empty);
        }

        private void lb_QuenPass_Click_1(object sender, EventArgs e)
        {
            ForgotPass forgotForm = new ForgotPass();

            // Lấy form chứa UserControl này (Login)
            Form parentForm = this.FindForm();

            if (parentForm != null)
            {
                forgotForm.Owner = parentForm; // ✅ gán Login làm chủ sở hữu
                parentForm.Hide();              // Ẩn Login
                forgotForm.Show();              // Mở ForgotPass
            }
            else
            {
                // fallback - nếu không có form cha
                forgotForm.Show();
            }
        }

        private void btn_DangNhap_Click(object sender, EventArgs e)
        {
            // Lấy form chứa usercontrol (ở đây là FormLogin)
            Form parentForm = this.FindForm();

            if (parentForm != null)
            {
                // Ẩn form login
                parentForm.Hide();

                // Mở form khách hàng
                Main main = new Main();

                // Khi form Khách hàng đóng → hiện lại form login
                main.FormClosed += (s2, e2) => parentForm.Show();
                main.Show();
            }
        }
    }
}
