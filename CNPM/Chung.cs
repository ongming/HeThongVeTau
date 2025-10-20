using System;
using System.CodeDom;
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
                lbTongdoanhthu.Text = totalRevenue.ToString("N0");
            }
            
            //tuyến phổ biến
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = @"SELECT TOP 1 
                                    CT.MaChuyen,
                                    CT.NoiDi,
                                    CT.NoiDen,
                                    CT.NgayDi,
                                    COUNT(V.MaVe) AS SoLuongDat
                                FROM VE AS V
                                JOIN CHUYENTAU AS CT ON V.MaChuyen = CT.MaChuyen
                                GROUP BY CT.MaChuyen, CT.NoiDi, CT.NoiDen, CT.NgayDi
                                ORDER BY SoLuongDat DESC;
                                ";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string Noidi = reader["NoiDi"].ToString();
                    string Noiden = reader["NoiDen"].ToString();
                    lbTuyenphobien.Text = Noidi +" - "+ Noiden;
                }
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
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT LoaiGhe, COUNT(*) AS SoLuong FROM VE GROUP BY LoaiGhe ORDER BY LoaiGhe ";
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                chartLoaiVe.Series.Clear();
                Series series = new Series("Loại Ghế");
                series.ChartType = SeriesChartType.Pie;

                while (reader.Read())
                {
                    string loaiGhe = reader["LoaiGhe"].ToString();
                    int soLuong = Convert.ToInt32(reader["SoLuong"]);
                    switch (loaiGhe.ToLower())
                    {
                        case "ghecung":
                            loaiGhe = "Ghế cứng";
                            break;
                        case "ghemem":
                            loaiGhe = "Ghế mềm";
                            break;
                        default:
                            loaiGhe = "Khác";
                            break;
                    }

                    series.Points.AddXY(loaiGhe, soLuong);
                }

                chartLoaiVe.Series.Add(series);
                //chartDanhGia.Series[0].IsValueShownAsLabel = true;
                chartLoaiVe.Series[0].Label = "#PERCENT{P0}"; // ví dụ: 45%
                chartLoaiVe.Series[0].LegendText = "#VALX";   // hiển thị tên cột trong chú thích
            }

            //biểu đồ thống kê đánh giá
            using (SqlConnection conn = DatabaseConnection.GetConnection())
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

            //biểu đồ doanh thu
            LoadChartDoanhThu();
            LoadChartVe();

        }

        public void LoadChartDoanhThu()
        {
            string type;
            DateTime? fromDate = null;
            DateTime? toDate = null;
            
            type = comboBox_DoanhThu.SelectedItem.ToString();
            if (type == "Năm")
            {
                type = "Year";
            }
            else if (type == "Quý")
            {
                type = "Quarter";
            }
            else if (type == "Tháng")
            {
                type = "Month";
            }
            else if (type == "Tuần")
            {
                type = "Week";
            }
            if (date_TuNgayDoanhThu.Checked)
                fromDate = date_TuNgayDoanhThu.Value;
            if (date_DenNgayDoanhThu.Checked)
                toDate = date_DenNgayDoanhThu.Value;

            DataTable dt = new DataTable();

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"
                -- Báo cáo doanh thu theo Month / Quarter / Year / Week
                DECLARE @FromDate DATETIME = @pFromDate;
                DECLARE @ToDate DATETIME = @pToDate;
                DECLARE @Type NVARCHAR(10) = @pType;

                IF @FromDate IS NULL OR @ToDate IS NULL
                BEGIN
                    SET @FromDate = DATEFROMPARTS(YEAR(GETDATE()),1,1);
                    SET @ToDate = GETDATE();
                END

                IF @Type = 'Month'
                BEGIN
                    SELECT YEAR(ThoiGianDat) AS Nam, MONTH(ThoiGianDat) AS Thang, SUM(TongTien) AS DoanhThu
                    FROM LICHSUGIAODICH
                    WHERE ThoiGianDat BETWEEN @FromDate AND @ToDate
                    GROUP BY YEAR(ThoiGianDat), MONTH(ThoiGianDat)
                    ORDER BY Nam, Thang;
                END
                ELSE IF @Type = 'Quarter'
                BEGIN
                    SELECT YEAR(ThoiGianDat) AS Nam, DATEPART(QUARTER, ThoiGianDat) AS Quy, SUM(TongTien) AS DoanhThu
                    FROM LICHSUGIAODICH
                    WHERE ThoiGianDat BETWEEN @FromDate AND @ToDate
                    GROUP BY YEAR(ThoiGianDat), DATEPART(QUARTER, ThoiGianDat)
                    ORDER BY Nam, Quy;
                END
                ELSE IF @Type = 'Year'
                BEGIN
                    SELECT YEAR(ThoiGianDat) AS Nam, SUM(TongTien) AS DoanhThu
                    FROM LICHSUGIAODICH
                    WHERE ThoiGianDat BETWEEN @FromDate AND @ToDate
                    GROUP BY YEAR(ThoiGianDat)
                    ORDER BY Nam;
                END
                ELSE IF @Type = 'Week'
                BEGIN
                    SELECT DATEPART(YEAR, ThoiGianDat) AS Nam, DATEPART(WEEK, ThoiGianDat) AS Tuan, SUM(TongTien) AS DoanhThu
                    FROM LICHSUGIAODICH
                    WHERE ThoiGianDat BETWEEN @FromDate AND @ToDate
                    GROUP BY DATEPART(YEAR, ThoiGianDat), DATEPART(WEEK, ThoiGianDat)
                    ORDER BY Nam, Tuan;
                END
            ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pFromDate", (object)fromDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@pToDate", (object)toDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@pType", type);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            // Xóa dữ liệu cũ
            chartDoanhThu.Series.Clear();

            Series series = new Series("DoanhThu");
            series.ChartType = SeriesChartType.Column; // Column hoặc Line
            series.XValueType = ChartValueType.String;
            series.YValueType = ChartValueType.Double;

            foreach (DataRow row in dt.Rows)
            {
                string xValue = "";
                if (row.Table.Columns.Contains("Thang"))
                    xValue = row["Nam"].ToString() + "-" + row["Thang"].ToString();
                else if (row.Table.Columns.Contains("Quy"))
                    xValue = row["Nam"].ToString() + "-Q" + row["Quy"].ToString();
                else if (row.Table.Columns.Contains("Tuan"))
                    xValue = row["Nam"].ToString() + "-W" + row["Tuan"].ToString();
                else
                    xValue = row["Nam"].ToString(); // Year

                double yValue = Convert.ToDouble(row["DoanhThu"]);
                series.Points.AddXY(xValue, yValue);
            }

            chartDoanhThu.Series.Add(series);

            // Tùy chỉnh nhãn X
            chartDoanhThu.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chartDoanhThu.ChartAreas[0].AxisX.Interval = 1;
        }

        public void LoadChartVe()
        {
            string type;
            DateTime? fromDate = null;
            DateTime? toDate = null;

            // Lấy type từ ComboBox
            type = ComboBox_ThongKeVe.SelectedItem.ToString();
            if (type == "Năm")
                type = "Year";
            else if (type == "Quý")
                type = "Quarter";
            else if (type == "Tháng")
                type = "Month";
            else if (type == "Tuần")
                type = "Week";
            else if (type == "Ngày")
                type = "Day";

            // Lấy from/to date từ DateTimePicker
            if (date_TuNgayVe.Checked)
                fromDate = date_TuNgayVe.Value;
            if (date_DenNgayVe.Checked)
                toDate = date_DenNgayVe.Value;

            DataTable dt = new DataTable();

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = @"
        DECLARE @FromDate DATETIME = @pFromDate;
        DECLARE @ToDate DATETIME = @pToDate;
        DECLARE @Type NVARCHAR(10) = @pType;

        IF @FromDate IS NULL OR @ToDate IS NULL
        BEGIN
            SET @FromDate = (SELECT MIN(NgayDat) FROM VE);
            SET @ToDate = (SELECT MAX(NgayDat) FROM VE);
        END

        IF @Type = 'Day'
        BEGIN
            SELECT CAST(NgayDat AS DATE) AS Ngay, COUNT(*) AS SoVeDat
            FROM VE
            WHERE NgayDat BETWEEN @FromDate AND @ToDate
            GROUP BY CAST(NgayDat AS DATE)
            ORDER BY Ngay;
        END
        ELSE IF @Type = 'Month'
        BEGIN
            SELECT YEAR(NgayDat) AS Nam, MONTH(NgayDat) AS Thang, COUNT(*) AS SoVeDat
            FROM VE
            WHERE NgayDat BETWEEN @FromDate AND @ToDate
            GROUP BY YEAR(NgayDat), MONTH(NgayDat)
            ORDER BY Nam, Thang;
        END
        ELSE IF (@Type = 'Quarter')
        BEGIN
            SELECT YEAR(NgayDat) AS Nam, DATEPART(QUARTER, NgayDat) AS Quy, COUNT(*) AS SoVeDat
            FROM VE
            WHERE NgayDat BETWEEN @FromDate AND @ToDate
            GROUP BY YEAR(NgayDat), DATEPART(QUARTER, NgayDat)
            ORDER BY Nam, Quy;
        END
        ELSE IF @Type = 'Year'
        BEGIN
            SELECT YEAR(NgayDat) AS Nam, COUNT(*) AS SoVeDat
            FROM VE
            WHERE NgayDat BETWEEN @FromDate AND @ToDate
            GROUP BY YEAR(NgayDat)
            ORDER BY Nam;
        END
        ELSE IF @Type = 'Week'
        BEGIN
            SELECT DATEPART(YEAR, NgayDat) AS Nam, DATEPART(WEEK, NgayDat) AS Tuan, COUNT(*) AS SoVeDat
            FROM VE
            WHERE NgayDat BETWEEN @FromDate AND @ToDate
            GROUP BY DATEPART(YEAR, NgayDat), DATEPART(WEEK, NgayDat)
            ORDER BY Nam, Tuan;
        END
        ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@pFromDate", (object)fromDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@pToDate", (object)toDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@pType", type);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            // Xóa dữ liệu cũ trên Chart
            chartLuongVe.Series.Clear();

            Series series = new Series("Số vé đã đặt");
            series.ChartType = SeriesChartType.Column;
            series.XValueType = ChartValueType.String;
            series.YValueType = ChartValueType.Int32;

            // Đẩy dữ liệu lên Chart
            foreach (DataRow row in dt.Rows)
            {
                string xValue = "";
                if (row.Table.Columns.Contains("Ngay"))
                    xValue = Convert.ToDateTime(row["Ngay"]).ToString("dd/MM/yyyy");
                else if (row.Table.Columns.Contains("Thang"))
                    xValue = row["Nam"].ToString() + "-" + row["Thang"].ToString();
                else if (row.Table.Columns.Contains("Quy"))
                    xValue = row["Nam"].ToString() + "-Q" + row["Quy"].ToString();
                else if (row.Table.Columns.Contains("Tuan"))
                    xValue = row["Nam"].ToString() + "-W" + row["Tuan"].ToString();
                else
                    xValue = row["Nam"].ToString(); // Year

                int yValue = Convert.ToInt32(row["SoVeDat"]);
                series.Points.AddXY(xValue, yValue);
            }

            chartLuongVe.Series.Add(series);

            // Tùy chỉnh nhãn X
            chartLuongVe.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chartLuongVe.ChartAreas[0].AxisX.Interval = 1;
        }

        private void guna2GradientPanel4_Paint(object sender, PaintEventArgs e)
        {
        }

        private void ComboBox_ThongKeVe_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadChartVe();
        }

        private void comboBox_DoanhThu_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadChartDoanhThu();
        }

        private void date_TuNgayDoanhThu_ValueChanged(object sender, EventArgs e)
        {
            LoadChartDoanhThu();
        }

        private void date_DenNgayDoanhThu_ValueChanged(object sender, EventArgs e)
        {
            LoadChartDoanhThu();
        }

        private void date_TuNgayVe_ValueChanged(object sender, EventArgs e)
        {
            LoadChartVe();
        }

        private void date_DenNgayVe_ValueChanged(object sender, EventArgs e)
        {
            LoadChartVe();
        }
    }
}
