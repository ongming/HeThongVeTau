using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourNamespace;

namespace CNPM
{
    public partial class LichSuKhachHang : Form
    {
        ThongTinKhachHang kh;
        public LichSuKhachHang(ThongTinKhachHang kh)
        {
            this.kh = kh;
            InitializeComponent();
            Grid_LichSu.AutoGenerateColumns = false;
            string text = "Từ ngày";
            string text1 = "Đến ngày";
            ModernGridStyle.ApplyPlaceholder(date_TuNgay, text);
            ModernGridStyle.ApplyPlaceholder(date_DenNgay, text1);
            LichSuGiaoDich_Load();
        }
        private void LichSuGiaoDich_Load()
        {
            HienThiLichSu();
        }

        private void HienThiLichSu()
        {
            DataTable dt = KhachHangRepository.LayLichSuTheoKhach(kh.MaKhachHang);
            Grid_LichSu.DataSource = dt;
            // Tuỳ chỉnh hiển thị
            //Grid_LichSu.Columns["MaGiaoDich"].HeaderText = "Mã giao dịch";
            //Grid_LichSu.Columns["PhuongThucThanhToan"].HeaderText = "Phương thức thanh toán";
            //Grid_LichSu.Columns["TongTien"].HeaderText = "Tổng tiền giao dịch";
            //Grid_LichSu.Columns["ThoiGianDat"].HeaderText = "Ngày mua";

            Grid_LichSu.Columns["TongTien"].DefaultCellStyle.Format = "N0";
            Grid_LichSu.Columns["ThoiGianDat"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }
        private void Grid_LichSu_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra có click đúng vào cột Button không
            if (Grid_LichSu.Columns[e.ColumnIndex].Name == "ChiTiet" && e.RowIndex >= 0)
            {
                int maGiaoDich = Convert.ToInt32(Grid_LichSu.Rows[e.RowIndex].Cells["MaGiaoDich"].Value);

                // 👉 Gọi form xem chi tiết (hiện thông tin các vé)
                //FormChiTietGiaoDich f = new FormChiTietGiaoDich(maGiaoDich);
                //f.ShowDialog();
            }
        }


    }
}
