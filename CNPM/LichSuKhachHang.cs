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
        ThongTinKhachHang kh;
        private Panel panelChiTiet;
        private Timer slideTimer;
        private int targetHeight = 180;

        public LichSuKhachHang(ThongTinKhachHang kh)
        {
            this.kh = kh;
            InitializeComponent();
            Grid_LichSu.AutoGenerateColumns = false;
            string text = "Từ ngày";
            string text1 = "Đến ngày";
            ModernGridStyle.ApplyPlaceholder(date_TuNgay, text);
            ModernGridStyle.ApplyPlaceholder(date_DenNgay, text1);
            LichSuGiaoDich_Load();
        }
        private void LichSuGiaoDich_Load()
        {
            HienThiLichSu();
        }

        private void DateFilter_Changed(object sender, EventArgs e)
        {
            DateTime? tuNgay = null;
            DateTime? denNgay = null;

            // 🔹 Nếu user có tick checkbox thì mới lấy giá trị
            if (date_TuNgay.Checked)
                tuNgay = date_TuNgay.Value.Date;

            if (date_DenNgay.Checked)
                denNgay = date_DenNgay.Value.Date;

            // 🔹 Logic tự động suy luận
            if (tuNgay == null && denNgay == null)
            {
                // Không chọn gì → Lấy toàn bộ
                Grid_LichSu.DataSource = KhachHangRepository.LayTatCa(kh.MaKhachHang);
                return;
            }
            else if (tuNgay != null && denNgay == null)
            {
                // Chỉ chọn từ ngày → đến hôm nay
                denNgay = DateTime.Now.Date;
            }
            else if (tuNgay == null && denNgay != null)
            {
                // Chỉ chọn đến ngày → từ rất sớm
                tuNgay = new DateTime(2000, 1, 1);
            }

            // 🔹 Kiểm tra hợp lệ
            if (tuNgay > denNgay)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 🔹 Gọi dữ liệu từ DB
            DataTable dt = KhachHangRepository.LayLichSuTheoNgay(kh.MaKhachHang, tuNgay.Value, denNgay.Value);
            Grid_LichSu.DataSource = dt;
        }

        private void HienThiLichSu()
        {
            DataTable dt = KhachHangRepository.LayLichSuTheoKhach(kh.MaKhachHang);
            Grid_LichSu.DataSource = dt;

            Grid_LichSu.Columns["TongTien"].DefaultCellStyle.Format = "N0";
            Grid_LichSu.Columns["ThoiGianDat"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void Grid_LichSu_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && Grid_LichSu.Columns[e.ColumnIndex].Name == "ChiTiet")
            {
                int maGD = Convert.ToInt32(Grid_LichSu.Rows[e.RowIndex].Cells["MaGiaoDich"].Value);
                DataTable dt = KhachHangRepository.LayChiTietGiaoDich(maGD);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu chi tiết!", "Thông báo");
                    return;
                }

                HienThiChiTietTuNut(e, dt);
            }
        }
        private void HienThiChiTietTuNut(DataGridViewCellEventArgs e, DataTable dt)
        {
            if (panelChiTiet != null && this.Controls.Contains(panelChiTiet))
                this.Controls.Remove(panelChiTiet);

            Rectangle cellRect = Grid_LichSu.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            Point cellLocation = Grid_LichSu.PointToScreen(cellRect.Location);
            Point relative = this.PointToClient(cellLocation);

            Point gridOnForm = this.PointToClient(Grid_LichSu.PointToScreen(Point.Empty));
            int widthChiTiet = Grid_LichSu.Columns["ChiTiet"].Width;

            // 🔹 Tạo panel
            panelChiTiet = new Panel
            {
                Width = Grid_LichSu.Width,
                Height = 0,
                BackColor = Color.LightCyan,
                BorderStyle = BorderStyle.FixedSingle
            };

            // 🔹 Tính chỗ trống bên dưới hàng được click
            int spaceBelow = this.ClientSize.Height - (relative.Y + cellRect.Height);

            bool veLenTren = spaceBelow < targetHeight + 30; // nếu không đủ chỗ thì vẽ ngược lên

            // 🔹 Xác định vị trí
            if (veLenTren)
                panelChiTiet.Location = new Point(gridOnForm.X, relative.Y - targetHeight + 80 - 2);
            else
                panelChiTiet.Location = new Point(gridOnForm.X, relative.Y + cellRect.Height + 2);

            // === Các phần nội dung bên trong giữ nguyên ===
            var first = dt.Rows[0];
            Label lblRoute = new Label
            {
                Text = $"🚆 {first["NoiDi"]} → {first["NoiDen"]}\n📅 {first["NgayDi"]}  🕓 {first["GioDi"]} → {first["GioDen"]}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            panelChiTiet.Controls.Add(lblRoute);

            TextBox txt = new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true,
                ReadOnly = true,
                BackColor = Color.White,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9),
                Location = new Point(10, 50),
                Size = new Size(panelChiTiet.Width - 30, 100)
            };

            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                string loaiGheRaw = row["LoaiGhe"].ToString().Trim();
                string loaiGheVN = loaiGheRaw == "GheMem" ? "Ghế mềm"
                                  : loaiGheRaw == "GheCung" ? "Ghế cứng"
                                  : loaiGheRaw;

                sb.AppendLine($"💺 Ghế {row["SoGhe"]} ({loaiGheVN}) – {row["GiaTien"]}₫");
                sb.AppendLine($"👤 {row["TenNguoiSoHuu"]} – ☎ {row["SoDienThoai"]} – 🪪 {row["CCCD"]}");
                sb.AppendLine();
            }
            txt.Text = sb.ToString().Trim();
            panelChiTiet.Controls.Add(txt);

            Button btnClose = new Button
            {
                Text = "Đóng",
                Size = new Size(60, 25),
                Location = new Point(panelChiTiet.Width - 80, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightGray
            };
            btnClose.Click += (s, e2) =>
            {
                Timer closeTimer = new Timer { Interval = 5 };
                closeTimer.Tick += (s2, e3) =>
                {
                    if (panelChiTiet.Height > 0)
                        panelChiTiet.Height -= 20;
                    else
                    {
                        closeTimer.Stop();
                        this.Controls.Remove(panelChiTiet);
                    }
                };
                closeTimer.Start();
            };
            panelChiTiet.Controls.Add(btnClose);

            this.Controls.Add(panelChiTiet);
            panelChiTiet.BringToFront();

            // 🔹 Animation mở xuống hoặc mở lên
            slideTimer = new Timer { Interval = 5 };
            slideTimer.Tick += (s, e2) =>
            {
                if (panelChiTiet.Height < targetHeight)
                    panelChiTiet.Height += 20;
                else
                    slideTimer.Stop();
            };

            // Nếu vẽ ngược lên thì khởi đầu từ chiều cao tối đa và thu lại xuống
            if (veLenTren)
            {
                panelChiTiet.Height = 0;
                slideTimer.Tick += (s, e2) =>
                {
                    if (panelChiTiet.Height < targetHeight)
                    {
                        panelChiTiet.Top -= 20; // đi ngược hướng
                        panelChiTiet.Height += 20;
                    }
                    else
                        slideTimer.Stop();
                };
            }

            slideTimer.Start();
        }


    }
}
