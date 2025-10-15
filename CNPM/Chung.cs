using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourNamespace;

namespace CNPM
{
    public partial class Chung : UserControl
    {
        public Chung()
        {
            InitializeComponent();
        }
        private void Chung_Load(object sender, EventArgs e)
        {
            ComboBox_ThongKeVe.Text = "Tháng";
            comboBox_DoanhThu.Text = "Tháng";
            string text = "Từ ngày";
            string text1 = "Đến ngày";
            ModernGridStyle.ApplyPlaceholder(date_TuNgayVe, text);
            ModernGridStyle.ApplyPlaceholder(date_DenNgayVe, text1);
            ModernGridStyle.ApplyPlaceholder(date_TuNgayDoanhThu, text);
            ModernGridStyle.ApplyPlaceholder(date_DenNgayDoanhThu, text1);

        }
    }
}
