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

namespace CNPM
{
    public partial class BangDieuKhien : Form
    {
        public BangDieuKhien()
        {
            InitializeComponent();
            LoadChart_DoanhThu();
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
            TaoChuyenMoi taoChuyenMoi = new TaoChuyenMoi();
            taoChuyenMoi.ShowDialog();
        }
    }
}
