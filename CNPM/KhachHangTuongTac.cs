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
    public partial class KhachHangTuongTac : Form
    {
        private Guna.UI2.WinForms.Guna2Panel pnThongBao;
        private FlowLayoutPanel flowThongBao;
        public KhachHangTuongTac()
        {
            InitializeComponent();
            pnThongBao = new Guna.UI2.WinForms.Guna2Panel()
            {
                Size = new Size(300, 250),
                FillColor = Color.White,
                BorderColor = Color.Gray,
                BackColor = Color.White,
                BorderThickness = 1,
                BorderRadius = 10,
                ShadowDecoration = { Enabled = true },
                Visible = false
            };
            pnThongBao.ShadowDecoration.Enabled = false;
            this.Controls.Add(pnThongBao);
            // 👇 Khi click ra ngoài thì ẩn panel và reset nút toggle
            this.Click += (s, e2) =>
            {
                if (pnThongBao.Visible)
                {
                    pnThongBao.Visible = false;
                    btn_ThongBao.Checked = false;
                }
            };
        }

        private void btn_DangXuat_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
        "Bạn có chắc muốn đăng xuất không?",
        "Xác nhận",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                this.Close();
            }
        }
        private void Container(Form fm)
        {
            panel_main.Controls.Clear();

            fm.TopLevel = false;
            fm.FormBorderStyle = FormBorderStyle.None;
            fm.Dock = DockStyle.Fill;

            panel_main.Controls.Add(fm);
            panel_main.Tag = fm;
            fm.Show();
        }
        private void btnThongBao_CheckedChanged(object sender, EventArgs e)
        {
            if (btn_ThongBao.Checked)
            {
                // ✅ Lấy tọa độ thật của nút Thông báo trên Form
                Point buttonScreenPos = btn_ThongBao.PointToScreen(Point.Empty);
                Point formPos = this.PointToClient(buttonScreenPos);

                // ✅ Hiển thị panel ngay bên dưới nút
                pnThongBao.BringToFront();
                pnThongBao.Location = new Point(
                    formPos.X + btn_ThongBao.Width - pnThongBao.Width,
                    formPos.Y + btn_ThongBao.Height + 5
                );

                pnThongBao.Visible = true;
            }
            else
            {
                pnThongBao.Visible = false;
            }
        }

        private void btn_TrangChu_Click(object sender, EventArgs e)
        {
            TrangChu trangChu = new TrangChu();
            Container(trangChu);
        }

        private void btn_DanhGia_Click(object sender, EventArgs e)
        {
            GopYKhachHang gopYKhachHang = new GopYKhachHang();
            Container(gopYKhachHang);
        }

        private void btn_LichSu_Click(object sender, EventArgs e)
        {
            LichSuKhachHang lichSuKhachHang = new LichSuKhachHang();
            Container(lichSuKhachHang);
        }
    }
}
