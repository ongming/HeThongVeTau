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
        public int MaKhachHang;
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
            Grid_Ve.AutoGenerateColumns = false;
            DataTable ve = NhanVienRepository.LayDanhSachVe();
            MaKhachHang = Convert.ToInt32(ve.Rows[0]["MaKhachHang"]);
            Grid_Ve.DataSource = ve;
            Grid_Ve.Columns["GiaTien"].DefaultCellStyle.Format = "N0";
            Grid_Ve.Columns["GiaTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Grid_Ve.Columns["NgayDat"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Grid_Ve.Columns["NgayDat"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ModernGridStyle.Apply(Grid_Ve);

        }
        private void Grid_Ve_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // 🔹 Kiểm tra đúng cột nút "Hủy vé"
            if (e.ColumnIndex >= 0 && Grid_Ve.Columns[e.ColumnIndex].Name == "HuyVe" && e.RowIndex >= 0)
            {
                e.PaintBackground(e.CellBounds, true);

                // Nền đỏ
                using (Brush brush = new SolidBrush(Color.FromArgb(220, 53, 69))) // đỏ Bootstrap
                {
                    e.Graphics.FillRectangle(brush, e.CellBounds);
                }

                // Viền tròn nhẹ
                using (Pen pen = new Pen(Color.DarkRed, 1))
                {
                    e.Graphics.DrawRectangle(pen, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width - 1, e.CellBounds.Height - 1);
                }

                // Chữ trắng, canh giữa
                TextRenderer.DrawText(e.Graphics, "🗑️ Hủy vé", e.CellStyle.Font,
                    e.CellBounds, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
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
        private void Grid_Ve_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Nếu click ra ngoài vùng dữ liệu thì bỏ qua
            if (e.RowIndex < 0) return;

            // Lấy tên cột hiện tại
            string columnName = Grid_Ve.Columns[e.ColumnIndex].Name;

            // Kiểm tra nếu là nút "Chi tiết vé"
            if (columnName == "ChiTiet")
            {
                // Lấy dữ liệu hàng được chọn
                DataGridViewRow row = Grid_Ve.Rows[e.RowIndex];
                string maVe = row.Cells["MaVe"].Value.ToString();
                string tenKH = row.Cells["TenNguoiSoHuu"].Value.ToString();
                string soDT = row.Cells["SoDienThoai"].Value.ToString();
                string cccd = row.Cells["CCCD"].Value.ToString();
                string SoGhe = row.Cells["SoGhe"].Value.ToString();
                string loaiGhe = row.Cells["LoaiGhe"].Value.ToString();
                string giaTien = row.Cells["GiaTien"].Value.ToString();
                string ngayDat = row.Cells["NgayDat"].Value.ToString();
               

                // Mở form chi tiết và truyền dữ liệu sang
                ChiTietVe frm = new ChiTietVe(
                    nv, maVe, MaKhachHang, tenKH, soDT, cccd, SoGhe, loaiGhe, giaTien, ngayDat
                );
                frm.ShowDialog();
            }
        }

    }
}
