using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
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
                Font = new System.Drawing.Font("Segoe UI", 9, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold),
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
                Font = new System.Drawing.Font("Segoe UI", 9),
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
            grid.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI", 9, FontStyle.Bold);
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

        private void btn_XuatFile_Click(object sender, EventArgs e)
        {
            try
            {
                int thang = DateTime.Now.Month;
                int nam = DateTime.Now.Year;

                // ===== 1) LẤY DỮ LIỆU =====
                DataTable dtDoanhThuNgay = GetDoanhThuTheoNgay(thang, nam); // LICHSUGIAODICH
                (decimal tong, int soGD) = TinhTongDoanhThuVaSoGD(dtDoanhThuNgay);
                (int soVe, int gheMem, int gheCung) = DemVeTheoLoai(thang, nam); // VE + LSGD
                (decimal momo, decimal nganHang) = TongTheoPhuongThuc(thang, nam); // LSGD
                double diemTB = DiemTrungBinhYKien(thang, nam); // YKIENPHANHOI

                if (dtDoanhThuNgay.Rows.Count == 0 && soVe == 0)
                {
                    MessageBox.Show("Không có dữ liệu trong tháng này.");
                    return;
                }

                // ===== 2) TẠO CHART ẢNH =====
                var imgBarRevenue = CreateBarChart(
                    dtDoanhThuNgay, "Doanh thu theo ngày", "Ngày", "VNĐ",
                    "Ngay", "TongDoanhThu");

                // Biểu đồ tròn loại ghế
                var dtLoaiGhe = new DataTable();
                dtLoaiGhe.Columns.Add("Ten", typeof(string));
                dtLoaiGhe.Columns.Add("GiaTri", typeof(int));
                dtLoaiGhe.Rows.Add("Ghế mềm", gheMem);
                dtLoaiGhe.Rows.Add("Ghế cứng", gheCung);

                var imgPieLoaiGhe = CreatePieChart(
                    dtLoaiGhe, "Cơ cấu loại vé (số vé)", "Ten", "GiaTri");

                // Biểu đồ tròn phương thức thanh toán
                var dtPay = new DataTable();
                dtPay.Columns.Add("Ten", typeof(string));
                dtPay.Columns.Add("GiaTri", typeof(decimal));
                dtPay.Rows.Add("Momo", momo);
                dtPay.Rows.Add("Ngân hàng", nganHang);

                var imgPiePay = CreatePieChart(
                    dtPay, "Cơ cấu phương thức thanh toán", "Ten", "GiaTri");

                // ===== 3) CHỌN NƠI LƯU =====
                string filePath;
                using (var sfd = new SaveFileDialog()
                {
                    Filter = "PDF (*.pdf)|*.pdf",
                    FileName = $"BaoCao_DoanhThu_{thang}_{nam}.pdf"
                })
                {
                    if (sfd.ShowDialog() != DialogResult.OK) return;
                    filePath = sfd.FileName;
                }

                // ===== 4) TẠO PDF =====
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    var doc = new Document(PageSize.A4, 36, 36, 36, 36);
                    PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    // ==== FONT UNICODE (sửa lỗi mất dấu) ====
                    string fontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                    string arialUni = Path.Combine(fontsDir, "ARIALUNI.TTF"); // ưu tiên
                    string arial = Path.Combine(fontsDir, "arial.ttf");    // fallback
                    string fontPath = File.Exists(arialUni) ? arialUni : arial;

                    BaseFont bf = BaseFont.CreateFont(
                        fontPath,
                        BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED
                    );

                    var fTitle = new iTextSharp.text.Font(bf, 18, iTextSharp.text.Font.BOLD);
                    var fH1 = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD);
                    var fText = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);
                    var fTextBold = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD);

                    // Header đơn vị
                    var header = new Paragraph("HỆ THỐNG BÁN VÉ TÀU\n\n", fH1);
                    header.Alignment = Element.ALIGN_LEFT;
                    doc.Add(header);

                    // Tiêu đề báo cáo
                    var title = new Paragraph($"BÁO CÁO DOANH THU THÁNG {thang}/{nam}\n\n", fTitle);
                    title.Alignment = Element.ALIGN_CENTER;
                    doc.Add(title);

                    // 4.1 Thông tin tổng quan
                    var p = new Paragraph();
                    p.Add(new Phrase("1) Thông tin tổng quan\n", fH1));
                    p.Add(new Phrase($"• Tổng doanh thu: {FormatMoney(tong)} VNĐ\n", fText));
                    p.Add(new Phrase($"• Tổng số giao dịch: {soGD}\n", fText));
                    p.Add(new Phrase($"• Tổng số vé bán ra: {soVe}\n", fText));
                    if (soVe > 0)
                    {
                        var tyLeMem = 100.0 * gheMem / (double)soVe;
                        var tyLeCung = 100.0 * gheCung / (double)soVe;
                        var tyLeMomo = (momo + nganHang) == 0 ? 0 : (double)(momo / (momo + nganHang) * 100);
                        var tyLeNH = 100 - tyLeMomo;

                        p.Add(new Phrase($"• Tỷ lệ loại vé: Ghế mềm {tyLeMem:0.#}% – Ghế cứng {tyLeCung:0.#}%\n", fText));
                        p.Add(new Phrase($"• Tỷ lệ thanh toán: Momo {tyLeMomo:0.#}% – Ngân hàng {tyLeNH:0.#}%\n", fText));
                    }
                    p.Add(new Phrase($"• Điểm đánh giá trung bình: {(diemTB <= 0 ? "Chưa có" : $"{diemTB:0.0}/5")}\n\n", fText));
                    doc.Add(p);

                    // 4.2 Bảng doanh thu theo ngày
                    doc.Add(new Paragraph("2) Doanh thu theo ngày\n", fH1));
                    var table = new PdfPTable(5) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 12, 12, 22, 22, 22 });

                    AddHeaderCell(table, "Ngày", fTextBold);
                    AddHeaderCell(table, "Số GD", fTextBold);
                    AddHeaderCell(table, "Tổng doanh thu", fTextBold);
                    AddHeaderCell(table, "Doanh thu Momo", fTextBold);
                    AddHeaderCell(table, "Doanh thu NH", fTextBold);

                    foreach (DataRow r in dtDoanhThuNgay.Rows)
                    {
                        AddCell(table, r["Ngay"].ToString(), fText);
                        AddCell(table, r["SoGiaoDich"].ToString(), fText);
                        AddCell(table, FormatMoney(r["TongDoanhThu"]), fText);
                        AddCell(table, FormatMoney(r["DoanhThuMomo"]), fText);
                        AddCell(table, FormatMoney(r["DoanhThuNganHang"]), fText);
                    }
                    // Dòng tổng
                    var totalLbl = new PdfPCell(new Phrase("TỔNG", fTextBold))
                    {
                        Colspan = 2,
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Padding = 6
                    };
                    table.AddCell(totalLbl);

                    var totalVal = new PdfPCell(new Phrase($"{FormatMoney(tong)} VNĐ", fTextBold))
                    {
                        Colspan = 3,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 6
                    };
                    table.AddCell(totalVal);

                    doc.Add(table);
                    doc.Add(new Paragraph("\n"));

                    // 4.3 Biểu đồ
                    doc.Add(new Paragraph("3) Biểu đồ thống kê\n", fH1));

                    // Biểu đồ cột doanh thu theo ngày
                    AddChartImageToPdf(doc, imgBarRevenue, 520f);

                    // Biểu đồ tròn loại vé
                    AddChartImageToPdf(doc, imgPieLoaiGhe, 250f);

                    // Biểu đồ tròn phương thức thanh toán
                    AddChartImageToPdf(doc, imgPiePay, 250f);

                    // 4.4 Nhận xét tự động gợi ý
                    doc.Add(new Paragraph("\n4) Nhận xét & đề xuất\n", fH1));
                    var nx = new Paragraph("", fText);
                    nx.Add(new Phrase("- Doanh thu tập trung vào các ngày có nhiều giao dịch; cân nhắc đẩy mạnh khuyến mãi vào ngày thấp.\n", fText));
                    nx.Add(new Phrase("- Loại ghế bán chạy: " + (gheMem >= gheCung ? "Ghế mềm" : "Ghế cứng") + " → tối ưu cơ cấu ghế và giá cho tuyến chính.\n", fText));
                    nx.Add(new Phrase("- Tăng ưu đãi thanh toán Momo để gia tăng tỷ lệ cashless.\n", fText));
                    doc.Add(nx);

                    doc.Close();
                }

                MessageBox.Show("✅ Xuất PDF thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất PDF: " + ex.Message);
            }
        }
        private DataTable GetDoanhThuTheoNgay(int thang, int nam)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var da = new SqlDataAdapter(@"
                SELECT 
                    DAY(ThoiGianDat) AS Ngay,
                    COUNT(*) AS SoGiaoDich,
                    SUM(TongTien) AS TongDoanhThu,
                    SUM(CASE WHEN PhuongThucThanhToan = N'Momo' THEN TongTien ELSE 0 END) AS DoanhThuMomo,
                    SUM(CASE WHEN PhuongThucThanhToan = N'Ngân hàng' THEN TongTien ELSE 0 END) AS DoanhThuNganHang
                FROM LICHSUGIAODICH
                WHERE MONTH(ThoiGianDat) = @thang AND YEAR(ThoiGianDat) = @nam
                GROUP BY DAY(ThoiGianDat)
                ORDER BY Ngay", conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@thang", thang);
                da.SelectCommand.Parameters.AddWithValue("@nam", nam);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private (decimal tong, int soGD) TinhTongDoanhThuVaSoGD(DataTable dt)
        {
            decimal t = 0; int s = 0;
            foreach (DataRow r in dt.Rows)
            {
                t += r["TongDoanhThu"] == DBNull.Value ? 0 : Convert.ToDecimal(r["TongDoanhThu"]);
                s += r["SoGiaoDich"] == DBNull.Value ? 0 : Convert.ToInt32(r["SoGiaoDich"]);
            }
            return (t, s);
        }

        private (int soVe, int gheMem, int gheCung) DemVeTheoLoai(int thang, int nam)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var da = new SqlDataAdapter(@"
                SELECT v.LoaiGhe, COUNT(*) AS SoVe
                FROM VE v
                JOIN LICHSUGIAODICH l ON l.MaGiaoDich = v.MaGiaoDich
                WHERE MONTH(l.ThoiGianDat) = @thang AND YEAR(l.ThoiGianDat) = @nam
                GROUP BY v.LoaiGhe", conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@thang", thang);
                da.SelectCommand.Parameters.AddWithValue("@nam", nam);
                var dt = new DataTable();
                da.Fill(dt);

                int mem = 0, cung = 0;
                foreach (DataRow r in dt.Rows)
                {
                    var loai = r["LoaiGhe"].ToString();
                    var sl = Convert.ToInt32(r["SoVe"]);
                    if (string.Equals(loai, "GheMem", StringComparison.OrdinalIgnoreCase)) mem = sl;
                    else cung = sl;
                }
                return (mem + cung, mem, cung);
            }
        }

        private (decimal momo, decimal nganHang) TongTheoPhuongThuc(int thang, int nam)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var da = new SqlDataAdapter(@"
                SELECT PhuongThucThanhToan, SUM(TongTien) AS Tien
                FROM LICHSUGIAODICH
                WHERE MONTH(ThoiGianDat) = @thang AND YEAR(ThoiGianDat) = @nam
                GROUP BY PhuongThucThanhToan", conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@thang", thang);
                da.SelectCommand.Parameters.AddWithValue("@nam", nam);
                var dt = new DataTable();
                da.Fill(dt);
                decimal momo = 0, nh = 0;
                foreach (DataRow r in dt.Rows)
                {
                    var k = r["PhuongThucThanhToan"].ToString();
                    var v = r["Tien"] == DBNull.Value ? 0 : Convert.ToDecimal(r["Tien"]);
                    if (k == "Momo") momo = v; else nh = v;
                }
                return (momo, nh);
            }
        }

        private double DiemTrungBinhYKien(int thang, int nam)
        {
            using (var conn = DatabaseConnection.GetConnection())
            using (var cmd = new SqlCommand(@"
                SELECT AVG(CAST(DanhGia AS FLOAT)) 
                FROM YKIENPHANHOI 
                WHERE MONTH(NgayGopY) = @thang AND YEAR(NgayGopY) = @nam", conn))
            {
                cmd.Parameters.AddWithValue("@thang", thang);
                cmd.Parameters.AddWithValue("@nam", nam);
                conn.Open();
                var val = cmd.ExecuteScalar();
                return val == DBNull.Value || val == null ? 0 : Convert.ToDouble(val);
            }
        }

        // ================== PDF HELPERS ==================
        private static void AddHeaderCell(PdfPTable tbl, string text, iTextSharp.text.Font font)
        {
            var cell = new PdfPCell(new Phrase(text, font))
            {
                BackgroundColor = BaseColor.LIGHT_GRAY,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 6
            };
            tbl.AddCell(cell);
        }

        private static void AddCell(PdfPTable tbl, string text, iTextSharp.text.Font font)
        {
            var c = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            tbl.AddCell(c);
        }

        private static string FormatMoney(object v)
        {
            if (v == null || v == DBNull.Value) return "0";
            return string.Format("{0:#,##0}", Convert.ToDecimal(v));
        }

        // ================== CHART HELPERS ==================
        private static byte[] CreateBarChart(DataTable dt, string title, string xTitle, string yTitle,
                                             string xField, string yField)
        {
            var chart = new Chart { Width = 900, Height = 350, BackColor = Color.White };
            var area = new ChartArea("ca");
            area.AxisX.Title = xTitle;
            area.AxisY.Title = yTitle;
            area.AxisX.Interval = 1;
            chart.ChartAreas.Add(area);

            var series = new Series("Doanh thu")
            {
                ChartType = SeriesChartType.Column,
                XValueMember = xField,
                YValueMembers = yField,
                IsValueShownAsLabel = true
            };

            chart.Series.Add(series);
            chart.Titles.Add(new Title(title, Docking.Top,
                new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold), Color.Black));
            chart.DataSource = dt;
            chart.DataBind();

            using (var ms = new MemoryStream())
            {
                chart.SaveImage(ms, ChartImageFormat.Png);
                return ms.ToArray();
            }
        }

        private static byte[] CreatePieChart(DataTable dt, string title, string nameField, string valueField)
        {
            var chart = new Chart { Width = 430, Height = 300, BackColor = Color.White };
            var area = new ChartArea("ca");
            chart.ChartAreas.Add(area);

            var s = new Series("Pie")
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true
            };
            s.XValueMember = nameField;
            s.YValueMembers = valueField;
            chart.Series.Add(s);

            chart.Titles.Add(new Title(title, Docking.Top,
                new System.Drawing.Font("Segoe UI", 9, FontStyle.Bold), Color.Black));
            chart.DataSource = dt;
            chart.DataBind();

            using (var ms = new MemoryStream())
            {
                chart.SaveImage(ms, ChartImageFormat.Png);
                return ms.ToArray();
            }
        }

        private static void AddChartImageToPdf(Document doc, byte[] imgBytes, float width)
        {
            var img = iTextSharp.text.Image.GetInstance(imgBytes);
            img.ScaleToFit(width, 1000f);
            img.Alignment = Element.ALIGN_CENTER;
            img.SpacingBefore = 6f;
            img.SpacingAfter = 12f;
            doc.Add(img);
        }


    }
}
