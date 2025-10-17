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
        public QuanLyBanVe()
        {
            InitializeComponent();
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            // Apply giao diện hiện đại
            ModernGridStyle.Apply(Grid_Ve);

            string query = "SELECT * FROM VE";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_Ve.DataSource = dt;
            }

            //ModernGridStyle.HighlightStatus(Grid_Ve);
        }

        private void btn_TaoVe_Click(object sender, EventArgs e)
        {
            TaoChuyenMoi taoVe = new TaoChuyenMoi();
            taoVe.ShowDialog();
        }
    }
}
