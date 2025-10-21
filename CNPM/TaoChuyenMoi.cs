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
using static Guna.UI2.Native.WinApi;

namespace CNPM
{
    public partial class TaoChuyenMoi : Form
    {
        private ThongTinNhanVien nv;
        public TaoChuyenMoi(ThongTinNhanVien nv)
        {
            InitializeComponent();
            ModernGridStyle.ApplyPlaceholder(date_NgayDi, "Ngày đi");
            comboBox_Tu.Text = "Chọn điểm đi";
            comboBox_Den.Text = "Chọn điểm đến";
            date_GioDi.Checked = false;
            this.nv = nv;
        }

        private void lb_Closed_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txt_HovaTen_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void btn_ThemChuyen_Click(object sender, EventArgs e)
        {

            bool check = NhanVienRepository.ThemTuyenTauMoi(
                        comboBox_Tu.SelectedItem?.ToString() ?? comboBox_Tu.Text,
                        comboBox_Den.SelectedItem?.ToString() ?? comboBox_Den.Text,
                        date_GioDi.Value.TimeOfDay,
                        date_GioDen.Value.TimeOfDay,
                        date_NgayDi.Value,
                        int.Parse(ComboBox_SoGhe.Text),
                        decimal.Parse(ComboBox_GheMem.Text),
                        decimal.Parse(ComboBox_GheCung.Text),
                        nv.MaNhanVien
            );
            if (check)
            {
                MessageBox.Show("Thêm chuyến tàu mới thành công", "Thông báo");
                this.Close();

            }
            else {
                MessageBox.Show("Lỗi hệ thống, hãy thử lại sau", "Thông báo");
            }
        }

        
    }
}
