using Guna.UI2.WinForms;
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
    public partial class QuanLyBanVe : Form
    {
        ThongTinNhanVien nv;
        public QuanLyBanVe(ThongTinNhanVien nv)
        {
            InitializeComponent();
            this.nv = nv;
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            grid_load();
        }

        private void grid_load()
        {
            // Apply giao diện hiện đại
            ModernGridStyle.Apply(Grid_Ve);

            string query = "SELECT v.MaVe, v.MaGiaoDich, v.MaChuyen, v.MaKhachHang, v.SoGhe, v.LoaiGhe, v.GiaTien, v.TenNguoiSoHuu, v.SoDienThoai, v.CCCD, v.NgayDat  FROM VE v";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_Ve.Columns.Clear();
                Grid_Ve.DataSource = dt;
            }
        }

        private void btn_TaoVe_Click(object sender, EventArgs e)
        {
            TaoChuyenMoi taoVe = new TaoChuyenMoi(nv);
            taoVe.ShowDialog();
        }

        private void txt_TimKiem_TextChanged(object sender, EventArgs e)
        {
            string tuKhoa = txt_TimKiem.Text.Trim();
            if (tuKhoa.Length > 0)
            {
                Grid_Ve.DataSource = NhanVienRepository.TimKiemVe(tuKhoa);
            }
            else
            {
                grid_load();
            }
        }

        private void btn_ChinhSuaVe_Click(object sender, EventArgs e)
        {

        }
    }
}
