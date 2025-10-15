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
    public partial class QuanLyChuyenTau : Form
    {
        public QuanLyChuyenTau()
        {
            InitializeComponent();
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            // Apply giao diện hiện đại
            ModernGridStyle.Apply(Grid_Ve);

            // Load dữ liệu test
            string connString = "Data Source=LAPTOP-MKNGM2HG;Initial Catalog=TestVe;Integrated Security=True;TrustServerCertificate=True";
            string query = "SELECT * FROM Ve";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_Ve.DataSource = dt;
            }

            ModernGridStyle.HighlightStatus(Grid_Ve);
        }

        private void btn_ThemChuyen_Click(object sender, EventArgs e)
        {
            TaoChuyenMoi taoChuyenMoi = new TaoChuyenMoi();
            taoChuyenMoi.ShowDialog();
        }
    }
}
