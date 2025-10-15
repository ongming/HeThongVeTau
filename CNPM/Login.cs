using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            ResetToRole();
           
        }
        public void ResetToRole()
        {
            // 🟢 Tạo usercontrol Role
            var role = new Role();

            // 🔹 Khi chọn "Nhân viên"
            role.SwitchToNhanVien += (s, e) =>
            {
                var nv = new NhanVien();
                nv.SwitchToRole += (s2, e2) => ShowControl(role);
                ShowControl(nv);
            };

            // 🔹 Khi chọn "Khách hàng"
            role.SwitchToKhachHang += (s, e) =>
            {
                // 👉 Tạo UserControl Khách hàng và Đăng ký
                var kh = new KhachHang();


                // Khi bấm quay lại trong Khách hàng
                kh.SwitchToRole += (s2, e2) => ShowControl(role);


                // Hiển thị control Khách hàng đầu tiên
                ShowControl(kh);
            };

            // 🟣 Hiển thị màn hình chọn Role đầu tiên
            ShowControl(role);
        }
        private void ShowControl(UserControl control)
        {
            panel1.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panel1.Controls.Add(control);
        }
    }
}
