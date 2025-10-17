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
    public partial class QuanLyKhachHang : Form
    {
        public QuanLyKhachHang()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            string query = "SELECT * FROM KhachHang";
            string text = "Ngày tạo";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_KhachHang.DataSource = dt;
            }
            ModernGridStyle.ApplyPlaceholder(date_NgayTaoTK, text);
            ModernGridStyle.Apply(Grid_KhachHang);
            ModernGridStyle.AddMiniActionColumns(Grid_KhachHang); // ✅ thêm icon sau khi gán DataSource
            //ModernGridStyle.HighlightStatus(Grid_KhachHang);
            ComboBox_TrangThai.Text = "Trạng thái";
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
    }
}
