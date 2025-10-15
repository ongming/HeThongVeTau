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
    public partial class Role : UserControl
    {
        public Role()
        {
            InitializeComponent();
        }

        // Hai sự kiện chuyển màn hình
        public event EventHandler SwitchToNhanVien;
        public event EventHandler SwitchToKhachHang;

        private void btn_NhanVien_Click(object sender, EventArgs e)
        {
            SwitchToNhanVien?.Invoke(this, EventArgs.Empty);
        }
        private void btn_KhachHang_Click_1(object sender, EventArgs e)
        {
            SwitchToKhachHang?.Invoke(this, EventArgs.Empty);
        }
    }
}

