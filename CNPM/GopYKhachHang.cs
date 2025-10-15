using Guna.UI2.WinForms;
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
    public partial class GopYKhachHang : Form
    {
        public GopYKhachHang()
        {
            InitializeComponent();
        }
        private void LoadGopY()
        {
            guna2RatingStar1.ValueChanged += (s, e) =>
            {
                int soSao = (int)guna2RatingStar1.Value;
                
            };
        }
    }
}
