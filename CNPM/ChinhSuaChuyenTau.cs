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
    public partial class ChinhSuaChuyenTau : Form
    {
        int machuyentau;
        ThongTinNhanVien nv;
        public ChinhSuaChuyenTau(int machuyentau, ThongTinNhanVien nv)
        {
            InitializeComponent();
            this.machuyentau = machuyentau;
            this.nv = nv;
        }

        private void lb_Closed_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_ThemChuyen_Click(object sender, EventArgs e)
        {
            bool check = NhanVienRepository.SuaChuyenTau(
                        machuyentau,
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
                MessageBox.Show("Sửa chuyến tàu thành công", "Thông báo");

            }
            else
            {
                MessageBox.Show("Lỗi hệ thống, hãy thử lại sau", "Thông báo");
            }
        }
    }
}
