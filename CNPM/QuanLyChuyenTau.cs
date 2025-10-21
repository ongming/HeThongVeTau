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
    public partial class QuanLyChuyenTau : Form
    {
        ThongTinNhanVien nv;
        public QuanLyChuyenTau(ThongTinNhanVien nv)
        {
            InitializeComponent();
            this.nv = nv;
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            grid_ve_load();
        }
        private void grid_ve_load()
        {
            //// Apply giao diện hiện đại
            ModernGridStyle.Apply(Grid_Ve);

            // Load dữ liệu test

            string query = "SELECT * FROM CHUYENTAU";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_Ve.DataSource = dt;
            }

            //ModernGridStyle.HighlightStatus(Grid_Ve);
        }
        private void btn_ThemChuyen_Click(object sender, EventArgs e)
        {
            TaoChuyenMoi taoChuyenMoi = new TaoChuyenMoi(nv);
            taoChuyenMoi.ShowDialog();
            FormMain_Load(sender, e);
        }

        private void txt_Search_TextChanged(object sender, EventArgs e)
        {
            string cantim = txt_Search.Text.Trim();
            Grid_Ve.DataSource = NhanVienRepository.TimKiemChuyenTau(cantim);

        }

        private void btn_ChinhSuaChuyenTau_Click(object sender, EventArgs e)
        {
            if (Grid_Ve.CurrentRow != null)
            {
                int maChuyenTau = Convert.ToInt32(Grid_Ve.CurrentRow.Cells["MaChuyen"].Value);
                ChinhSuaChuyenTau form = new ChinhSuaChuyenTau(maChuyenTau, nv);
                form.ShowDialog();
                FormMain_Load(sender, e);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn chuyến tàu cần chỉnh sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
