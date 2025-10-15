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
    public partial class ThongKe_BaoCao : Form
    {
        public ThongKe_BaoCao()
        {
            InitializeComponent();
            ShowControl(new Chung());
            btn_Chung.Checked = true;
        }
        private void ShowControl(UserControl control)
        {
            panel_main.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panel_main.Controls.Add(control);
        }

        private void btn_Chung_Click(object sender, EventArgs e)
        {
            ShowControl(new Chung());
        }

        private void btn_LichSu_Click(object sender, EventArgs e)
        {
            ShowControl(new LichSuGiaoDich());
        }

        private void btn_PhanHoi_Click(object sender, EventArgs e)
        {
            ShowControl(new YKien());
        }
    }
}
