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
    public partial class Chung : UserControl
    {
        public Chung()
        {
            InitializeComponent();
        }
        private void Chung_Load(object sender, EventArgs e)
        {
            ComboBox_ThongKeVe.Text = "Tháng";
            comboBox_DoanhThu.Text = "Tháng";
            string text = "Từ ngày";
            string text1 = "Đến ngày";
            ModernGridStyle.ApplyPlaceholder(date_TuNgayVe, text);
            ModernGridStyle.ApplyPlaceholder(date_DenNgayVe, text1);
            ModernGridStyle.ApplyPlaceholder(date_TuNgayDoanhThu, text);
            ModernGridStyle.ApplyPlaceholder(date_DenNgayDoanhThu, text1);
            loadData();
        }
        private void loadData()
        {
            //tổng số vẽ đã bán
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Ve";
                SqlCommand cmd = new SqlCommand(query, conn);
                int totalTicketsSold = (int)cmd.ExecuteScalar();
                lbVedaban.Text = totalTicketsSold.ToString();
            }
            //tổng doanh thu
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT SUM(TongTien) FROM LICHSUGIAODICH";
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();
                decimal totalRevenue = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                lbTongdoanhthu.Text = totalRevenue.ToString("C0");
            }
            //đánh giá trung bình
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT AVG(DanhGia) FROM YKIENPHANHOI";
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();
                decimal averageRating = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                lbDiemtrungbinh.Text = averageRating.ToString("0.0");
            }

            //biểu đồ thống kê vé

            //biểu đồ thống kê đánh giá
            using (SqlConnection conn = new SqlConnection("Data Source=.;Initial Catalog=BanVeTau;Integrated Security=True"))
            {
                conn.Open();

                string query = "SELECT DanhGia, COUNT(*) AS SoLuong FROM YKIENPHANHOI GROUP BY DanhGia ORDER BY DanhGia";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                chartDanhGia.Series.Clear();
                Series series = new Series("Đánh giá");
                series.ChartType = SeriesChartType.Pie;

                while (reader.Read())
                {
                    string danhGia = reader["DanhGia"].ToString();
                    int soLuong = Convert.ToInt32(reader["SoLuong"]);
                    series.Points.AddXY(danhGia + " sao", soLuong);
                }

                chartDanhGia.Series.Add(series);
                //chartDanhGia.Series[0].IsValueShownAsLabel = true;
                chartDanhGia.Series[0].Label = "#PERCENT{P0}"; // ví dụ: 45%
                chartDanhGia.Series[0].LegendText = "#VALX";   // hiển thị tên cột trong chú thích
            }

        }

        private void guna2GradientPanel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
