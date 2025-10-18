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
    public partial class ThongTinCaNhan : Form
    {
        int MaUser;
        string Role;
        public ThongTinCaNhan(int MaUser, string Role)
        {
            this.MaUser = MaUser;
            this.Role = Role;
            InitializeComponent();
            ShowControl(new ThongTin(MaUser, Role));
            btn_ThongTin.Checked = true;
        }
        private void ShowControl(UserControl control)
        {
            panel_main.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panel_main.Controls.Add(control);
        }
        private void btn_ThongTin_Click(object sender, EventArgs e)
        {
            ShowControl(new ThongTin(MaUser,Role));
        }

        private void btn_BaoMat_Click(object sender, EventArgs e)
        {
            ShowControl(new BaoMat(MaUser, Role));
        }
    }
}
