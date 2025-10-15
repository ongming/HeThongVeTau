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
            string connString = "Data Source=LAPTOP-MKNGM2HG;Initial Catalog=TestVe;Integrated Security=True;TrustServerCertificate=True";
            string query = "SELECT * FROM ActivityLog";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_NhatKy.DataSource = dt;
            }
            ModernGridStyle.ApplyActivityFeed(Grid_NhatKy);
        }
    }
}
