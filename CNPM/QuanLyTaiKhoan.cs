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
    public partial class QuanLyTaiKhoan : Form
    {
        public QuanLyTaiKhoan()
        {
            InitializeComponent();
        }
        private void FormTK_Load(object sender, EventArgs e)
        {
           
            QuanLyTaiKhoan_Load();
            ModernGridStyle.ApplyActivityFeed(Grid_NhatKy);
            ModernGridStyle.Apply(Grid_TaiKhoanNhanVien);
        }

        //load form lên
        private void QuanLyTaiKhoan_Load()
        {   //grid tài khoản nhân viên
            string query = "SELECT * FROM TAIKHOAN WHERE VaiTro IN ('QuanLy', 'NhanVien');";
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_TaiKhoanNhanVien.DataSource = dt;
            }
            //grid nhật ký hoạt động
            string query1 = @"
                SELECT 
                    MaNhatKy,
                    CASE 
                        WHEN MaQuanLy IS NOT NULL THEN N'Quản lý - ' + CAST(MaQuanLy AS NVARCHAR(10))
                        WHEN MaNhanVien IS NOT NULL THEN N'Nhân viên - ' + CAST(MaNhanVien AS NVARCHAR(10))
                        ELSE N'Không xác định'
                    END AS NguoiThucHien,
                   
                    HanhDong,
                    ThoiGian
                FROM NHATKY_HOATDONG";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(query1, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_NhatKy.DataSource = dt;
            }
        }

        private void txt_TimKiem_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT * FROM TAIKHOAN WHERE (MaTaiKhoan LIKE @keyword OR TenDangNhap LIKE @keyword) AND (VaiTro IN ('QuanLy', 'NhanVien'))", conn);
                da.SelectCommand.Parameters.AddWithValue("@keyword", "%" + txt_TimKiem.Text + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_TaiKhoanNhanVien.DataSource = dt;
            }
        }

        private void btn_ThemNhanVien_Click(object sender, EventArgs e)
        {
            ThemNhanVien themNhanVien = new ThemNhanVien();
            themNhanVien.ShowDialog();
        }
    }
}
