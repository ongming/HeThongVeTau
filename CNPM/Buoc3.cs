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
        decimal tongTien;
        ThongTinKhachHang khachHang;
        decimal giaGheCung = 0;
        decimal giaGheMem = 0;
        public Buoc3(ThongTinChuyenTau thongTinChuyenTau, List<int> gheDuocChon, ThongTinKhachHang kh)
        {
            this.khachHang = kh;
            InitializeComponent();
            ThongTinChuyenTau = thongTinChuyenTau;
            GheDuocChon = gheDuocChon;
            HienThiThongTin();
            TaoCacFormHanhKhach();
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


            string[] parts = ThongTinChuyenTau.Gia.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            giaGheCung = 0;
            giaGheMem = 0;

            if (parts.Length >= 1)
                giaGheCung = decimal.Parse(parts[0].Replace(" VND", "").Trim(), System.Globalization.NumberStyles.AllowThousands);
            if (parts.Length >= 2)
                giaGheMem = decimal.Parse(parts[1].Replace(" VND", "").Trim(), System.Globalization.NumberStyles.AllowThousands);

            // 🔹 Tính tổng tiền: ghế > 20 = ghế cứng, còn lại = ghế mềm
            tongTien = GheDuocChon.Sum(g => g > 20 ? giaGheCung : giaGheMem);
            lb_TongTien.Text = tongTien.ToString("N0") + " VND";
        }

        private void TaoCacFormHanhKhach()
        {
            flow_HanhKhach.Controls.Clear();

            foreach (int ghe in GheDuocChon)
            {
                var uc = new UCtrl_HanhKhach();
                uc.SetSoGhe(ghe);
                uc.Margin = new Padding(10);
                uc.Width = 280;
                uc.Height = 280;

                flow_HanhKhach.Controls.Add(uc);
            }
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

        private void btn_HoanTat_Click(object sender, EventArgs e)
        {
            try
            {
                // 🔹 Lấy thông tin người sử dụng từng vé
                List<NguoiSuDungVe> danhSachNguoi = new List<NguoiSuDungVe>();

                foreach (UCtrl_HanhKhach uc in flow_HanhKhach.Controls.OfType<UCtrl_HanhKhach>())
                {
                    danhSachNguoi.Add(uc.LayThongTin());
                }

                // 🔹 Xác định phương thức thanh toán
                string phuongThuc = radio_NganHang.Checked ? "Ngân hàng" :
                                    radio_Momo.Checked ? "Momo" : "Không xác định";

                // 🔹 Gọi repository để lưu vào DB
                bool datThanhCong = KhachHangRepository.DatVe(
                    khachHang.MaKhachHang,           // từ login
                    ThongTinChuyenTau.MaChuyen,      // chuyến tàu đang đặt
                    GheDuocChon,                     // danh sách ghế
                    danhSachNguoi,                   // danh sách người dùng
                    tongTien,                        // tổng tiền đã tính
                    phuongThuc,                     // phương thức thanh toán
                    giaGheCung,
                    giaGheMem
                );

                if (datThanhCong)
                {
                    MessageBox.Show("✅ Đặt vé thành công!", "Thông báo");
                }
                else
                {
                    MessageBox.Show("❌ Có lỗi khi đặt vé!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xử lý: " + ex.Message);
            }
        }

        private void radio_Momo_CheckedChanged(object sender, EventArgs e)
        {
            ShowControl(new Momo());
        }
    }
}
