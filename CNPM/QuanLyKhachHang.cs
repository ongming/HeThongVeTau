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
        ThongTinNhanVien nv;
        public QuanLyKhachHang(ThongTinNhanVien nv)
        {
            InitializeComponent();
            this.nv = nv;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            string query = "SELECT MaKhachHang, HoTen, CCCD, NgaySinh, DiaChi, SoDienThoai, Gmail, NgayTao, TrangThai FROM KhachHang";
            string text = "Ngày tạo";
            Grid_KhachHang.Columns.Clear();
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Grid_KhachHang.DataSource = dt;
            }
            ModernGridStyle.ApplyPlaceholder(date_NgayTaoTK, text);
            ModernGridStyle.Apply(Grid_KhachHang);
            //ModernGridStyle.AddMiniActionColumns(Grid_KhachHang); // ✅ thêm icon sau khi gán DataSource
            // Tắt chế độ tự giãn cột của toàn bảng (nếu đang bật)
            Grid_KhachHang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // 🗑 Cột Xóa
            DataGridViewImageColumn btnDelete = new DataGridViewImageColumn();
            btnDelete.Name = "btnDelete";
            btnDelete.HeaderText = "";
            btnDelete.Image = Properties.Resources.delete; // hoặc Image.FromFile("icons/delete.png")
            btnDelete.ToolTipText = "Xóa";
            btnDelete.Width = 30; // cố định kích thước nhỏ
            btnDelete.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid_KhachHang.Columns.Add(btnDelete);

            // 💬 Cột Nhắn tin
            DataGridViewImageColumn btnMessage = new DataGridViewImageColumn();
            btnMessage.Name = "btnMessage";
            btnMessage.HeaderText = "";
            btnMessage.Image = Properties.Resources.nhantin;
            btnMessage.ToolTipText = "Nhắn tin";
            btnMessage.Width = 30;
            btnMessage.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Grid_KhachHang.Columns.Add(btnMessage);

            btnDelete.ImageLayout = DataGridViewImageCellLayout.Stretch;
            btnMessage.ImageLayout = DataGridViewImageCellLayout.Stretch;
            Grid_KhachHang.AllowUserToAddRows = false;

            // Cho phép các cột dữ liệu khác tự giãn (nếu muốn)
            Grid_KhachHang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


            //ModernGridStyle.HighlightStatus(Grid_KhachHang);
            ComboBox_TrangThai.Text = "Trạng thái";

            // số lượng khách hàng
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM KHACHHANG;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    int count = (int)cmd.ExecuteScalar();
                    lbTongKhachHang.Text = count.ToString();
                }
            }

            //khách hàng hôm nay
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM KhachHang WHERE CONVERT(date, NgayTao) = CONVERT(date, GETDATE())";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    int count = (int)cmd.ExecuteScalar();
                    lbKhachHangHomNay.Text = count.ToString();
                }
            }
            // khách hàng hoạt động
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM KhachHang WHERE TrangThai = N'Hoạt động'";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    int count = (int)cmd.ExecuteScalar();
                    lbKhachHangHoatDong.Text = count.ToString();
                }
            }
            // khách hàng bị chăn
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM KhachHang WHERE TrangThai = N'Bị chặn'";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    int count = (int)cmd.ExecuteScalar();
                    lbKhachHangBiChan.Text = count.ToString();
                }
            }



        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void txt_Search_TextChanged(object sender, EventArgs e)
        {
            TimKiemKhachHang();
        }

        private void TimKiemKhachHang()
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT MaKhachHang, HoTen, CCCD, NgaySinh, DiaChi, SoDienThoai, Gmail, NgayTao, TrangThai FROM KhachHang WHERE 1=1";
                // Tìm kiếm theo từ khóa
                if (!string.IsNullOrEmpty(txt_Search.Text))
                {
                    query += " AND (HoTen LIKE @keyword OR Gmail LIKE @keyword OR SoDienThoai LIKE @keyword)";
                }
                // Lọc theo trạng thái
                if (ComboBox_TrangThai.SelectedItem != null && ComboBox_TrangThai.SelectedItem.ToString() != "Trạng thái")
                {
                    query += " AND TrangThai = @trangthai";
                }
                // Lọc theo ngày tạo tài khoản
                if (date_NgayTaoTK.Checked)
                {
                    query += " AND CONVERT(date, NgayTao) BETWEEN CONVERT(date, @ngaytao) AND CONVERT(date, GETDATE())";
                }
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(txt_Search.Text))
                    {
                        cmd.Parameters.AddWithValue("@keyword", "%" + txt_Search.Text + "%");
                    }
                    if (ComboBox_TrangThai.SelectedItem != null && ComboBox_TrangThai.SelectedItem.ToString() != "Trạng thái")
                    {
                        cmd.Parameters.AddWithValue("@trangthai", ComboBox_TrangThai.SelectedItem.ToString());
                    }
                    if (date_NgayTaoTK.Checked)
                    {
                        cmd.Parameters.AddWithValue("@ngaytao", date_NgayTaoTK.Value.Date);
                    }
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    Grid_KhachHang.DataSource = dt;
                }
            }
        }

        private void ComboBox_TrangThai_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimKiemKhachHang();
        }

        private void date_NgayTaoTK_ValueChanged(object sender, EventArgs e)
        {
            TimKiemKhachHang();
        }

        private void Grid_KhachHang_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string columnName = Grid_KhachHang.Columns[e.ColumnIndex].Name;
            int maKH = Convert.ToInt32(Grid_KhachHang.Rows[e.RowIndex].Cells["MaKhachHang"].Value);

            if (columnName == "btnMessage")
            {
                Messenger form = new Messenger(maKH, nv);
                form.ShowDialog();
            }
            else if (columnName == "btnDelete")
            {
                try
                {
                    using (SqlConnection conn = DatabaseConnection.GetConnection())
                    {
                        conn.Open();

                        //Lấy trạng thái hiện tại
                        string getStatusQuery = "SELECT TrangThai FROM KhachHang WHERE MaKhachHang = @maKH";
                        string trangThaiHienTai = "";
                        using (SqlCommand cmd = new SqlCommand(getStatusQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@maKH", maKH);
                            object result = cmd.ExecuteScalar();
                            trangThaiHienTai = result?.ToString() ?? "Hoạt động";
                        }

                        //Xác định trạng thái mới
                        string trangThaiMoi = trangThaiHienTai == "Hoạt động" ? "Bị chặn" : "Hoạt động";
                        string thongBao = trangThaiHienTai == "Hoạt động"
                            ? "Khách hàng này đang hoạt động. Bạn có muốn CHẶN khách hàng này không?"
                            : "Khách hàng này đang bị chặn. Bạn có muốn KÍCH HOẠT lại khách hàng này không?";

                        //Hỏi xác nhận
                        var confirm = MessageBox.Show(thongBao, "Xác nhận thay đổi trạng thái", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (confirm != DialogResult.Yes)
                            return;

                        //Cập nhật trạng thái khách hàng
                        string updateKH = @"
                                        UPDATE KhachHang 
                                        SET TrangThai = @trangThaiMoi 
                                        WHERE MaKhachHang = @maKH";
                        using (SqlCommand cmd = new SqlCommand(updateKH, conn))
                        {
                            cmd.Parameters.AddWithValue("@trangThaiMoi", trangThaiMoi);
                            cmd.Parameters.AddWithValue("@maKH", maKH);
                            cmd.ExecuteNonQuery();
                        }

                        //Cập nhật trạng thái tài khoản tương ứng
                        string updateTK = @"
                                UPDATE TaiKhoan 
                                SET TrangThai = @trangThaiTK
                                WHERE MaLienKet = @maKH AND VaiTro = 'KhachHang'";

                        using (SqlCommand cmd = new SqlCommand(updateTK, conn))
                        {
                            // Nếu khách hàng bị chặn => tài khoản bị khóa (0)
                            // Nếu khách hàng hoạt động => tài khoản mở (1)
                            int trangThaiTK = (trangThaiMoi == "Hoạt động") ? 1 : 0;

                            cmd.Parameters.AddWithValue("@trangThaiTK", trangThaiTK);
                            cmd.Parameters.AddWithValue("@maKH", maKH);
                            cmd.ExecuteNonQuery();
                        }
                        //Ghi nhật ký hoạt động
                        string insertLog = "";
                        if (nv.VaiTro == "NhanVien")
                            insertLog = "INSERT INTO NhatKy_HoatDong (MaNhanVien, HanhDong, ThoiGian) VALUES (@maNV, @hanhDong, GETDATE())";
                        else if (nv.VaiTro == "QuanLy")
                            insertLog = "INSERT INTO NhatKy_HoatDong (MaQuanLy, HanhDong, ThoiGian) VALUES (@maQL, @hanhDong, GETDATE())";

                        if (!string.IsNullOrEmpty(insertLog))
                        {
                            using (SqlCommand cmd = new SqlCommand(insertLog, conn))
                            {
                                if (nv.VaiTro == "NhanVien")
                                    cmd.Parameters.AddWithValue("@maNV", nv.MaNhanVien);
                                else
                                    cmd.Parameters.AddWithValue("@maQL", nv.MaNhanVien);

                                cmd.Parameters.AddWithValue("@hanhDong", $"Thay đổi trạng thái khách hàng có mã {maKH} từ '{trangThaiHienTai}' sang '{trangThaiMoi}'");
                                cmd.ExecuteNonQuery();
                            }
                        }
                        //Thông báo + Reload
                        MessageBox.Show("✅ Thay đổi trạng thái khách hàng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.FormMain_Load(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Lỗi khi thay đổi trạng thái: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
