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
    public partial class Main : Form
    {
        private Guna.UI2.WinForms.Guna2Panel pnThongBao;

        public Main()
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
        private void btn_ve_Click(object sender, EventArgs e)
        {
            QuanLyBanVe quanLyBanVe = new QuanLyBanVe();
            Container(quanLyBanVe);
        }

        private void btn_tau_Click(object sender, EventArgs e)
        {
            QuanLyChuyenTau quanLyChuyenTau = new QuanLyChuyenTau();
            Container(quanLyChuyenTau);
        }

        private void btn_KhachHang_Click(object sender, EventArgs e)
        {
            QuanLyKhachHang quanLyKhachHang = new QuanLyKhachHang();
            Container(quanLyKhachHang);
        }

        private void btn_ThongKe_Click(object sender, EventArgs e)
        {
            ThongKe_BaoCao thongKe_BaoCao = new ThongKe_BaoCao();
            Container(thongKe_BaoCao);
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

        private void avatar_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show("Nhấp vào để xem thông tin người dùng", pictureBox_avatar);
        }

        private void avatar_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(pictureBox_avatar);
        }

        private void pictureBox_avatar_Click(object sender, EventArgs e)
        {
            ThongTinCaNhan thongTinCaNhan = new ThongTinCaNhan();
            Container(thongTinCaNhan);
        }

        private void btn_TaiKhoan_Click(object sender, EventArgs e)
        {
            QuanLyTaiKhoan taiKhoan = new QuanLyTaiKhoan();
            Container(taiKhoan);
        }

        private void btn_Dashboard_Click(object sender, EventArgs e)
        {
            BangDieuKhien bangDieuKhien = new BangDieuKhien();
            Container(bangDieuKhien);
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
    }
}
