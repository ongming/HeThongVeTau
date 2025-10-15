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
    public partial class Buoc3 : UserControl
    {
        private ThongTinChuyenTau ThongTinChuyenTau;
        private List<int> GheDuocChon;
        public Buoc3(ThongTinChuyenTau thongTinChuyenTau, List<int> gheDuocChon)
        {
            InitializeComponent();
            ThongTinChuyenTau = thongTinChuyenTau;
            GheDuocChon = gheDuocChon;
            HienThiThongTin();
            VeCacGhe();
        }
        private void ShowControl(UserControl control)
        {
            panel.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panel.Controls.Add(control);
        }

        private void radio_NganHang_CheckedChanged(object sender, EventArgs e)
        {
            ShowControl(new NganHang());
        }
        private void HienThiThongTin()
        {
            lblTuyen.Text = ThongTinChuyenTau.Tuyen;
            lblNgay.Text = ThongTinChuyenTau.Ngay;
            lblGio.Text = ThongTinChuyenTau.Gio;
            lblGia.Text = ThongTinChuyenTau.Gia;
        }

        private void VeCacGhe()
        {
            flow_GheDaChon.Controls.Clear();

            foreach (int ghe in GheDuocChon)
            {
                Guna2Panel pnl = new Guna2Panel();
                pnl.Width = 55;
                pnl.Height = 30;
                pnl.BorderRadius = 8;
                pnl.BorderColor = Color.LightGray;
                pnl.BorderThickness = 1;
                pnl.FillColor = Color.WhiteSmoke;
                pnl.Margin = new Padding(6);

                Label lbl = new Label();
                lbl.Text = "Ghế " + ghe;
                lbl.Dock = DockStyle.Fill;
                lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.BackColor = Color.Transparent;
                pnl.Controls.Add(lbl);
                flow_GheDaChon.Controls.Add(pnl);
            }
        }
    }
}
