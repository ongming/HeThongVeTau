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
            this.Load += form_load;
        }
        private void form_load(object sender, EventArgs e)
        {
            DataTable table = NhanVienRepository.LayThongTinChuyenTau(machuyentau);

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];

                comboBox_Tu.Text = row["NoiDi"].ToString();
                comboBox_Den.Text = row["NoiDen"].ToString();

                date_GioDi.Value = DateTime.Today.Add(TimeSpan.Parse(row["GioDi"].ToString()));
                date_GioDen.Value = DateTime.Today.Add(TimeSpan.Parse(row["GioDen"].ToString()));
                date_NgayDi.Value = Convert.ToDateTime(row["NgayDi"]);

                ComboBox_SoGhe.Text = row["TongSoGhe"].ToString();
                ComboBox_GheMem.Text = Convert.ToInt32( row["GiaGheMem"]).ToString();
                ComboBox_GheCung.Text = Convert.ToInt32(row["GiaGheCung"]).ToString();
            }

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
                this.Close();
            }
            else
            {
                MessageBox.Show("Lỗi hệ thống, hãy thử lại sau", "Thông báo");
            }
        }
    }
}
