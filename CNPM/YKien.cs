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
            string connString = "Data Source=LAPTOP-MKNGM2HG;Initial Catalog=TestVe;Integrated Security=True;TrustServerCertificate=True";
            string query = "SELECT * FROM GopY";
            string text = "Ngày đánh giá";
            using (SqlConnection conn = new SqlConnection(connString))
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
    }
}
