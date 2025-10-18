using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    public partial class GopYKhachHang : Form
    {
        ThongTinKhachHang khachHang;
        public GopYKhachHang(ThongTinKhachHang kh)
        {
            InitializeComponent();
            khachHang = kh;
        }

        private void btn_GuiGopY_Click(object sender, EventArgs e)
        {
            int danhGia = (int)guna2RatingStar.Value;
            string noiDung = txt_DanhGia.Text.Trim();

            if (danhGia == 0)
            {
                MessageBox.Show("Vui lòng chọn số sao đánh giá!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(noiDung))
            {
                MessageBox.Show("Vui lòng nhập nội dung góp ý!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int maKH = khachHang.MaKhachHang; // Lấy từ form đăng nhập

            bool ketQua = KhachHangRepository.GuiYKien(maKH, danhGia, noiDung);
            if (ketQua)
            {
                guna2RatingStar.Value = 0;
                txt_DanhGia.Clear();
            }
        }
    }
}
