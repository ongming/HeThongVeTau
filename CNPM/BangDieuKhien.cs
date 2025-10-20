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
using System.Windows.Forms.DataVisualization.Charting;
using YourNamespace;

namespace CNPM
{
    public partial class BangDieuKhien : Form
    {
        ThongTinNhanVien nv;
        private Guna.UI2.WinForms.Guna2Panel pn_ChuyenHomNay;

        public BangDieuKhien(ThongTinNhanVien nv)
        {
            InitializeComponent();
            LoadChart_DoanhThu();
            this.nv = nv;
        }
        private void LoadChart_DoanhThu()
        {
            label5.Text = NhanVienRepository.GetSoVeBanTrongThang().ToString(); 
            label7.Text= NhanVienRepository.GetDoanhThuHomNay().ToString("N0") + " VNĐ";
            label9.Text = NhanVienRepository.GetSoChuyenTauKhoiHanhHomNay().ToString();

            string query = @"SELECT MONTH(ThoiGianDat) AS Thang,
                            SUM(TongTien) AS TongDoanhThu
                            FROM LICHSUGIAODICH
                            GROUP BY MONTH(ThoiGianDat)
                            ORDER BY Thang;";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                chart_DoanhThu.Series.Clear();
                Series series = new Series("Doanh thu theo tháng");
                series.ChartType = SeriesChartType.Line;   // 🔹 Biểu đồ đường
                series.BorderWidth = 3;                    // 🔹 Độ dày đường
                series.Color = Color.FromArgb(52, 152, 219); // 🔹 Màu xanh hiện đại
                series.MarkerStyle = MarkerStyle.Circle;   // 🔹 Dấu tròn tại mỗi điểm
                series.MarkerSize = 8;
                series.MarkerColor = Color.FromArgb(41, 128, 185);
                series.IsValueShownAsLabel = true;         // 🔹 Hiện giá trị trên từng điểm

                while (reader.Read())
                {
                    int thang = Convert.ToInt32(reader["Thang"]);
                    double doanhThu = Convert.ToDouble(reader["TongDoanhThu"]);
                    series.Points.AddXY("Tháng " + thang, doanhThu);
                }

                chart_DoanhThu.Series.Add(series);

                // 🎨 Tùy chỉnh vùng hiển thị
                var area = chart_DoanhThu.ChartAreas[0];
                area.AxisX.Title = "Tháng";
                area.AxisY.Title = "Doanh thu (VNĐ)";
                area.AxisX.MajorGrid.Enabled = false;
                area.AxisY.MajorGrid.LineColor = Color.LightGray;
                area.AxisX.LabelStyle.Angle = -45; // 🔹 Xoay nhãn tháng cho gọn
                area.BackColor = Color.White;
                chart_DoanhThu.BackColor = Color.White;

                // 💫 Làm đường mượt hơn (spline)
                series.ChartType = SeriesChartType.Spline;
                series.BorderWidth = 3; 

                // 🔹 Tắt viền vùng chart
                area.BorderDashStyle = ChartDashStyle.NotSet;
            }
        }

        private void btn_TaoChuyenNhanh_Click(object sender, EventArgs e)
        {
            TaoChuyenMoi taoChuyenMoi = new TaoChuyenMoi(nv);
            taoChuyenMoi.ShowDialog();
        }

        private void btn_LichTau_Click(object sender, EventArgs e)
        {
            // 🔹 Lấy danh sách chuyến hôm nay
            DataTable dt = NhanVienRepository.LayChuyenTauHomNay();

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("❌ Hôm nay không có chuyến tàu nào khởi hành.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Nếu panel đã mở rồi → đóng lại
            if (pn_ChuyenHomNay != null && pn_ChuyenHomNay.Visible)
            {
                pn_ChuyenHomNay.Visible = false;
                this.Controls.Remove(pn_ChuyenHomNay);
                return;
            }

            // 🔹 Tạo panel hiển thị
            pn_ChuyenHomNay = new Guna.UI2.WinForms.Guna2Panel()
            {
                Size = new Size(850, 400),
                BorderRadius = 10,
                BorderColor = Color.Silver,
                BorderThickness = 1,
                FillColor = Color.White,
                ShadowDecoration = { Enabled = true },
                AutoScroll = true,
                Visible = true,
                BackColor = Color.White,
                Anchor = AnchorStyles.None
            };

            // Vị trí trung tâm form
            pn_ChuyenHomNay.Location = new Point(
                (this.ClientSize.Width - pn_ChuyenHomNay.Width) / 2,
                (this.ClientSize.Height - pn_ChuyenHomNay.Height) / 2
            );

            // 🔹 Nút đóng
            Guna.UI2.WinForms.Guna2Button btn_Close = new Guna.UI2.WinForms.Guna2Button()
            {
                Text = "Đóng",
                Size = new Size(80, 30),
                BorderRadius = 5,
                FillColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(pn_ChuyenHomNay.Width - 100, 10)
            };
            btn_Close.Click += (s, e2) =>
            {
                this.Controls.Remove(pn_ChuyenHomNay);
                pn_ChuyenHomNay.Dispose();
            };
            pn_ChuyenHomNay.Controls.Add(btn_Close);

            // 🔹 Tiêu đề
            Label lblTitle = new Label()
            {
                Text = "🚆 Danh sách chuyến tàu hôm nay",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            pn_ChuyenHomNay.Controls.Add(lblTitle);

            // 🔹 Bảng dữ liệu
            // 🔹 Bảng dữ liệu Guna2DataGridView
            var grid = new Guna.UI2.WinForms.Guna2DataGridView()
            {
                DataSource = dt,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9),
                Location = new Point(20, 60),
                Size = new Size(pn_ChuyenHomNay.Width - 40, pn_ChuyenHomNay.Height - 80),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToResizeRows = false,
                GridColor = Color.FromArgb(231, 229, 255)
            };

            // Header style
            grid.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(0, 120, 215);
            grid.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            grid.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            grid.ThemeStyle.HeaderStyle.Height = 30;

            // Row style
            grid.ThemeStyle.RowsStyle.BackColor = Color.White;
            grid.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(0, 120, 215); // xanh chọn
            grid.ThemeStyle.RowsStyle.SelectionForeColor = Color.White;
            grid.ThemeStyle.RowsStyle.Height = 28;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.ClearSelection();
            grid.DefaultCellStyle.SelectionBackColor = grid.DefaultCellStyle.BackColor;
            grid.DefaultCellStyle.SelectionForeColor = grid.DefaultCellStyle.ForeColor;


            // ✅ Đổi màu theo trạng thái (sau khi load)
            grid.DataBindingComplete += (s, args) =>
            {
                foreach (DataGridViewRow row in grid.Rows)
                {
                    string trangThai = row.Cells["TrangThai"].Value?.ToString()?.Trim();

                    if (trangThai == "Đang khởi hành")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(198, 239, 206); // xanh nhạt
                    else if (trangThai == "Chưa khởi hành")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 249, 196); // vàng nhạt
                    else if (trangThai == "Đã kết thúc")
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 205, 210); // đỏ nhạt
                }
            };


            // Add to panel
            pn_ChuyenHomNay.Controls.Add(grid);


            // 🔹 Thêm panel vào form
            this.Controls.Add(pn_ChuyenHomNay);
            pn_ChuyenHomNay.BringToFront();
        }
    }
}
