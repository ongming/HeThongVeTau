using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourNamespace;

namespace CNPM
{
    public partial class TaoChuyenMoi : Form
    {
        public TaoChuyenMoi()
        {
            InitializeComponent();
            ModernGridStyle.ApplyPlaceholder(date_NgayDi, "Ngày đi");
            comboBox_Tu.Text = "Chọn điểm đi";
            comboBox_Den.Text = "Chọn điểm đến";
            date_GioDi.Checked  = false;
        }

        private void lb_Closed_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
