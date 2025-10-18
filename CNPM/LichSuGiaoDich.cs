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
using YourNamespace;

namespace CNPM
{
    public partial class LichSuGiaoDich : UserControl
    {
        public LichSuGiaoDich()
        {
            InitializeComponent();
            
        }
        private void LichSu_Load(object sender, EventArgs e)
        {
            // Apply giao diện hiện đại
            string text = "Từ ngày";
            string text1 = "Đến ngày";
            ModernGridStyle.ApplyPlaceholder(date_TuNgay, text);
            ModernGridStyle.ApplyPlaceholder(date_DenNgay, text1);
            ModernGridStyle.Apply(Grid_LichSu);

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = "SELECT LS.MaGiaoDich, KH.HoTen, LS.ThoiGianDat, LS.PhuongThucThanhToan, LS.TongTien " +
                               "FROM LICHSUGIAODICH AS LS " +
                               "JOIN KHACHHANG AS KH ON LS.MaKhachHang = KH.MaKhachHang";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_LichSu.DataSource = dt;
            }
        }
        private void timkiemlichsukhachhang()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query =
                    "SELECT LS.MaGiaoDich, KH.HoTen, LS.ThoiGianDat, LS.PhuongThucThanhToan, LS.TongTien " +
                    "FROM LICHSUGIAODICH AS LS " +
                    "JOIN KHACHHANG AS KH ON LS.MaKhachHang = KH.MaKhachHang " +
                    "WHERE 1=1 "; // luôn đúng, để nối điều kiện động

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = new SqlCommand();
                da.SelectCommand.Connection = conn;

                // Nếu có chọn Từ ngày
                if (date_TuNgay.Checked)
                {
                    query += " AND CONVERT(date, LS.ThoiGianDat) >= @tungay";
                    da.SelectCommand.Parameters.AddWithValue("@tungay", date_TuNgay.Value.Date);
                }

                // Nếu có chọn Đến ngày
                if (date_DenNgay.Checked)
                {
                    query += " AND CONVERT(date, LS.ThoiGianDat) <= @denngay";
                    da.SelectCommand.Parameters.AddWithValue("@denngay", date_DenNgay.Value.Date);
                }

                da.SelectCommand.CommandText = query;

                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_LichSu.DataSource = dt;
            }
        }

        private void date_TuNgay_ValueChanged(object sender, EventArgs e)
        {
            timkiemlichsukhachhang();
        }

        private void date_DenNgay_ValueChanged(object sender, EventArgs e)
        {
            timkiemlichsukhachhang();
        }
    }
}
