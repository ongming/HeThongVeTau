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
    public partial class TrangChu : Form
    {
        public TrangChu()
        {
            InitializeComponent();
            ShowControl(new MuaVe());
        }
        private void ShowControl(UserControl control)
        {
            panel_main.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panel_main.Controls.Add(control);
        }
    }
}
