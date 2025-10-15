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
    public partial class BaoMat : UserControl
    {
        public BaoMat()
        {
            InitializeComponent();
        }
        private bool isEyeOpen = false;
        private void TogglePassword(Guna.UI2.WinForms.Guna2TextBox txt)
        {
            bool isEyeOpen = txt.Tag as bool? ?? false;
            isEyeOpen = !isEyeOpen;
            txt.Tag = isEyeOpen;

            if (isEyeOpen)
            {
                txt.IconRight = Properties.Resources.witness_1518564;
                txt.UseSystemPasswordChar = false;
                txt.PasswordChar = '\0';
            }
            else
            {
                txt.IconRight = Properties.Resources.invisible_98494;
                txt.UseSystemPasswordChar = false;
                txt.PasswordChar = '•';
            }
        }
        private void txt_Pass_IconRightClick(object sender, EventArgs e)
        {
            TogglePassword(txt_MKMoi);
        }

        private void txt_RePass_IconRightClick(object sender, EventArgs e)
        {
            TogglePassword(txt_NhapLaiMKMoi);
        }
        private void tip_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Bạn nên đổi mật khẩu 1-2 tháng 1 lần để tăng tính bảo mật cho tài khoản", pictureBox_Tip);
        }

        private void tip_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(pictureBox_Tip);
        }
    }
}
