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
        int MaUser;
        string Role;
        public BaoMat(int MaUser, string Role)
        {
            this.MaUser = MaUser;
            this.Role = Role;
            InitializeComponent();
        }
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

        private void btn_DoiMK_Click(object sender, EventArgs e)
        {
            try
            {
                string mkCu = txt_MKCu.Text.Trim();
                string mkMoi = txt_MKMoi.Text.Trim();
                string nhapLai = txt_NhapLaiMKMoi.Text.Trim();

                // 1️⃣ Kiểm tra rỗng
                if (string.IsNullOrEmpty(mkCu) || string.IsNullOrEmpty(mkMoi) || string.IsNullOrEmpty(nhapLai))
                {
                    MessageBox.Show("⚠️ Vui lòng nhập đầy đủ thông tin.", "Thiếu thông tin",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2️⃣ Kiểm tra trùng mật khẩu mới
                if (mkMoi != nhapLai)
                {
                    MessageBox.Show("⚠️ Mật khẩu mới nhập lại không khớp.", "Sai xác nhận",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3️⃣ Kiểm tra mật khẩu cũ có đúng không
                if (!KhachHangRepository.KiemTraMatKhauCu(MaUser, mkCu))
                {
                    MessageBox.Show("❌ Mật khẩu cũ không chính xác.", "Sai mật khẩu",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 4️⃣ Cập nhật mật khẩu mới
                KhachHangRepository.DoiMatKhau(MaUser, mkMoi);

                MessageBox.Show("✅ Đổi mật khẩu thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 5️⃣ Dọn dữ liệu sau khi đổi
                txt_MKCu.Clear();
                txt_MKMoi.Clear();
                txt_NhapLaiMKMoi.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi đổi mật khẩu: " + ex.Message,
                    "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
