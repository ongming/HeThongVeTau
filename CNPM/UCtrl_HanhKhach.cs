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
    public partial class UCtrl_HanhKhach : UserControl
    {
        public int SoGhe { get; set; }
        public UCtrl_HanhKhach()
        {
            InitializeComponent();
        }
        public void SetSoGhe(int ghe)
        {
            SoGhe = ghe;
            lbl_Ghe.Text = $"Ghế {ghe}";
        }
        public NguoiSuDungVe LayThongTin()
        {
            return new NguoiSuDungVe
            {
                TenNguoiSuDung = txt_HovaTen.Text.Trim(),
                SoDienThoai = txt_SDT.Text.Trim(),
                CCCD = txt_CCCD.Text.Trim()
            };
        }
    }
}
