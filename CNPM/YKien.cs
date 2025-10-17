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
    public partial class YKien : UserControl
    {
        public YKien()
        {
            InitializeComponent();
        }
        private void YKien_Load(object sender, EventArgs e)
        {
            string query = "SELECT YK.MaGopY, KH.HoTen, YK.NgayGopY, YK.DanhGia, Yk.NoiDung " +
                "FROM YKIENPHANHOI AS YK " +
                "JOIN KHACHHANG AS KH ON YK.MaKhachHang = KH.MaKhachHang";
            string text = "Ngày đánh giá";
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_PhanHoi.DataSource = dt;
            }
            ModernGridStyle.ApplyModern(Grid_PhanHoi);
            ModernGridStyle.ApplyPlaceholder(date_NgayPhanHoi, text);
            comboBox_DanhGia.Text = "Đánh giá";
        }
        
        // tìm kiếm ý kiến theo tên
        private void TimKiemDanhGia()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT YK.MaGopY, KH.HoTen, YK.NgayGopY, YK.DanhGia, Yk.NoiDung " +
                    "FROM YKIENPHANHOI AS YK " +
                    "JOIN KHACHHANG AS KH ON YK.MaKhachHang = KH.MaKhachHang " +
                    "WHERE KH.HoTen LIKE @ten AND (@danhgia = '' OR YK.DanhGia = @danhgia) " +
                    "AND ((@ngay IS NULL) OR (CONVERT(date, YK.NgayGopY) BETWEEN @ngay AND CONVERT(date, GETDATE())))", conn);
                //tìm kiếm theo tên
                da.SelectCommand.Parameters.AddWithValue("@ten", "%" + txt_Search.Text + "%");
                
                //tìm kiếm theo đánh giá
                if (comboBox_DanhGia.SelectedItem != null && comboBox_DanhGia.SelectedItem.ToString() != "Đánh giá")
                {
                    da.SelectCommand.Parameters.AddWithValue("@danhgia", comboBox_DanhGia.SelectedIndex);
                }
                else
                {
                    da.SelectCommand.Parameters.AddWithValue("@danhgia", "");
                }

                //tìm kiếm theo ngày
                if (date_NgayPhanHoi.Checked)
                {
                    da.SelectCommand.Parameters.AddWithValue("@ngay", date_NgayPhanHoi.Value.Date);
                }
                else
                {
                    da.SelectCommand.Parameters.AddWithValue("@ngay", DBNull.Value);
                }
                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_PhanHoi.DataSource = dt;

            }
        }
        private void txt_Search_TextChanged(object sender, EventArgs e)
        {
            TimKiemDanhGia();
        }

        private void comboBox_DanhGia_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimKiemDanhGia();
        }

        private void date_NgayPhanHoi_ValueChanged(object sender, EventArgs e)
        {
            TimKiemDanhGia();
        }
    }
}
