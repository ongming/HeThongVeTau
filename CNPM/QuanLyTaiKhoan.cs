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
        private ThongTinNhanVien nv;
        public QuanLyTaiKhoan(ThongTinNhanVien nv)
        {
            InitializeComponent();
            this.nv = nv;
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
                FROM NHATKY_HOATDONG
                ORDER BY ThoiGian DESC";

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
            ThemNhanVien themNhanVien = new ThemNhanVien(nv);
            themNhanVien.ShowDialog();
            QuanLyTaiKhoan_Load();
        }

        private void Grid_NhatKy_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu click vào header hoặc hàng trống thì bỏ qua
            if (e.RowIndex < 0)
                return;

            // Lấy hàng được click
            DataGridViewRow selectedRow = Grid_NhatKy.Rows[e.RowIndex];

            // Lấy giá trị trong các cột
            string maNhatKy = selectedRow.Cells["MaNhatKy"].Value?.ToString();
            string nguoiThucHien = selectedRow.Cells["NguoiThucHien"].Value?.ToString();
            string hanhDong = selectedRow.Cells["HanhDong"].Value?.ToString();
            string thoiGian = selectedRow.Cells["ThoiGian"].Value?.ToString();

            // Hiển thị thông tin ra MessageBox (bạn có thể thay bằng form chi tiết hoặc textbox)
            MessageBox.Show(
                $"📋 Chi tiết nhật ký:\n\n" +
                $"🆔 Mã nhật ký: {maNhatKy}\n" +
                $"👤 Người thực hiện: {nguoiThucHien}\n" +
                $"⚙️ Hành động: {hanhDong}\n" +
                $"⏰ Thời gian: {thoiGian}",
                "Chi tiết nhật ký",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}
