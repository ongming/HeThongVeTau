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
    public partial class ForgotPass : Form
    {
        public ForgotPass()
        {
            InitializeComponent();
        }

        private void lb_DangNhap_Click(object sender, EventArgs e)
        {
            if (this.Owner is Login loginForm)
            {
                // Gọi hàm reset lại trang Role
                loginForm.ResetToRole();

                // Hiện lại form Login
                loginForm.Show();
            }

            // Đóng form quên mật khẩu
            this.Close();
        }

        private void btn_GuiYeuCau_Click(object sender, EventArgs e)
        {
            String mail= txt_gmail.Text.Trim();
            if (mail.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập email vào ô");
            }
            KhachHangRepository.DatLaiMatKhau(mail);
        }
    }
}
