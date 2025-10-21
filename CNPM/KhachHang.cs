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

            KhachHangRepository repo = new KhachHangRepository();
            ThongTinKhachHang kh = repo.CheckLogin(user, pass);

            // Lấy form cha chứa usercontrol hiện tại (chính là Form Login)
            Form parentForm = this.FindForm();

            if (kh != null)
            {
                MessageBox.Show($"Đăng nhập thành công!\nXin chào {kh.HoTen}",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 🔹 Ẩn form Login
                parentForm.Hide();

                // 🔹 Tạo form Khách hàng và truyền thông tin đăng nhập
                KhachHangTuongTac frm = new KhachHangTuongTac(kh);

                // 🔹 Khi form khách hàng đóng → hiện lại form Login
                frm.FormClosed += (s2, e2) => parentForm.Show();

                frm.Show();
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
