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
    public partial class LichSuKhachHang : Form
    {
        public LichSuKhachHang()
        {
            InitializeComponent();
            string text = "Từ ngày";
            string text1 = "Đến ngày";
            ModernGridStyle.ApplyPlaceholder(date_TuNgay, text);
            ModernGridStyle.ApplyPlaceholder(date_DenNgay, text1);
        }
    }
}
