using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    public partial class Main : Form
    {
        private Guna.UI2.WinForms.Guna2Panel pnThongBao;
        public ThongTinNhanVien nv;
        public Main(ThongTinNhanVien nv)
        {
            NhanVienRepository.GhiThongBaoDangNhap(nv.MaNhanVien,nv.VaiTro);
            this.nv = nv;
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
            this.nv = nv;
            CapNhatTrangThaiThongBao();
            CanhChamDoTheoThongBao();
            DataTable table = NguoiDungRepository.LayThongTin(nv.MaNhanVien, nv.VaiTro);
            if (table.Rows.Count > 0 && table.Rows[0]["Avatar"] != DBNull.Value)
            {
                byte[] avatarBytes = (byte[])table.Rows[0]["Avatar"];

                using (MemoryStream ms = new MemoryStream(avatarBytes))
                {
                    pictureBox_avatar.Image = Image.FromStream(ms);
                    pictureBox_avatar.SizeMode = PictureBoxSizeMode.StretchImage; // cho ảnh vừa khung
                }
            }
            else
            {
                // Nếu chưa có ảnh thì hiển thị ảnh mặc định
                pictureBox_avatar.Image = Properties.Resources.androgynous_avatar_non_binary_queer_person;
                pictureBox_avatar.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            if(nv.VaiTro == "NhanVien")
            {
                btn_ThongKe.Visible = false;
                btn_TaiKhoan.Visible = false;
            }
        }
        private void CapNhatTrangThaiThongBao()
        {
            bool coMoi = NhanVienRepository.CoThongBaoChuaXem(nv.MaNhanVien, nv.VaiTro);
            panel_DOT.Visible = coMoi;
        }
        private void CanhChamDoTheoThongBao()
        {
            // Nếu chấm đỏ không nằm cùng parent -> thêm vào nút
            if (panel_DOT.Parent != btn_ThongBao)
            {
                btn_ThongBao.Controls.Add(panel_DOT);
                panel_DOT.BringToFront();
            }

            // Canh vị trí trong phạm vi của nút
            panel_DOT.Location = new Point(
                btn_ThongBao.Width - panel_DOT.Width - 2, // sát mép phải
                0     // lệch xuống một chút
            );
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
            QuanLyBanVe quanLyBanVe = new QuanLyBanVe(nv);
            Container(quanLyBanVe);
        }

        private void btn_tau_Click(object sender, EventArgs e)
        {
            QuanLyChuyenTau quanLyChuyenTau = new QuanLyChuyenTau(nv);
            Container(quanLyChuyenTau);
        }

        private void btn_KhachHang_Click(object sender, EventArgs e)
        {
            QuanLyKhachHang quanLyKhachHang = new QuanLyKhachHang(nv);
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
            ThongTinCaNhan thongTinCaNhan = new ThongTinCaNhan(nv.MaNhanVien, nv.VaiTro);
            Container(thongTinCaNhan);
        }

        private void btn_TaiKhoan_Click(object sender, EventArgs e)
        {
            QuanLyTaiKhoan taiKhoan = new QuanLyTaiKhoan();
            Container(taiKhoan);
        }

        private void btn_Dashboard_Click(object sender, EventArgs e)
        {
            BangDieuKhien bangDieuKhien = new BangDieuKhien(nv);
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
                HienThiThongBao();
                NhanVienRepository.DanhDauDaXem(nv.MaNhanVien, nv.VaiTro);
                panel_DOT.Visible = false;
            }
            else
            {
                pnThongBao.Visible = false;
            }
        }

        private void HienThiThongBao()
        {
            // Xóa nội dung cũ
            pnThongBao.Controls.Clear();

            // Lấy dữ liệu từ DB
            DataTable dt = NhanVienRepository.LayThongBaoTheoNguoiNhan(nv.MaNhanVien, nv.VaiTro);

            if (dt.Rows.Count == 0)
            {
                Label lbl = new Label
                {
                    Text = "📭 Không có thông báo nào",
                    ForeColor = Color.Gray,
                    Font = new Font("Segoe UI", 9, FontStyle.Italic),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                pnThongBao.Controls.Add(lbl);
                return;
            }

            // 🧱 Tạo container Panel giữ layout ổn định
            Panel container = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(5),
                BackColor = Color.White
            };

            // 🔹 FlowLayoutPanel để chứa từng item
            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.White
            };

            // 🔹 Duyệt từng dòng trong DataTable
            foreach (DataRow row in dt.Rows)
            {
                // 📨 Panel cho từng thông báo
                Panel item = new Panel
                {
                    Width = pnThongBao.Width - 50,
                    Height = 55,
                    BackColor = Color.FromArgb(248, 250, 255),
                    Margin = new Padding(5),
                    BorderStyle = BorderStyle.FixedSingle
                };

                // Tiêu đề nội dung
                Label lblNoiDung = new Label
                {
                    Text = row["NoiDung"].ToString(),
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.Black,
                    AutoSize = false,
                    Size = new Size(item.Width - 10, 30),
                    Location = new Point(5, 5)
                };
                item.Controls.Add(lblNoiDung);

                // Thời gian
                Label lblTime = new Label
                {
                    Text = row["ThoiGian"].ToString(),
                    Font = new Font("Segoe UI", 8, FontStyle.Italic),
                    ForeColor = Color.DimGray,
                    AutoSize = true,
                    Location = new Point(5, 33)
                };
                item.Controls.Add(lblTime);

                // Nếu chưa xem → đánh dấu xanh nhạt
                if (row["DaXem"] != DBNull.Value && !(bool)row["DaXem"])
                    item.BackColor = Color.FromArgb(225, 240, 255);

                flow.Controls.Add(item);
            }

            // ✅ Gắn flow vào container
            container.Controls.Add(flow);
            pnThongBao.Controls.Add(container);
        }
    }
}
