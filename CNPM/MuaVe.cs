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


namespace CNPM
{
    public partial class MuaVe : UserControl
    {
        private ThongTinChuyenTau thongTinChuyenDuocChon = null;
        private List<int> gheDuocChon = new List<int>();
        private Buoc1 buoc1;

        public MuaVe()
        {
            InitializeComponent();
            btn_Buoc2.Enabled = false; // lúc đầu chưa chọn thì disable
            btn_Buoc3.Enabled = false; // lúc đầu chưa chọn thì disable

        }
        private void ShowControl(UserControl control)
        {
            panel.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panel.Controls.Add(control);
        }

        private void btn_Buoc1_Click(object sender, EventArgs e)
        {
            ShowBuoc1();
        }
        private void ShowBuoc1()
        {
            buoc1 = new Buoc1();

            // Đăng ký sự kiện khi chọn 1 chuyến tàu
            buoc1.ChuyenTauDaChon += (thongTin) =>
            {
                thongTinChuyenDuocChon = thongTin;
                btn_Buoc2.Enabled = true;
            };

            ShowControl(buoc1);
            btn_Buoc2.Enabled = false; // lúc đầu chưa chọn thì disable
            btn_Buoc3.Enabled = false; // lúc đầu chưa chọn thì disable
        }
        private void btn_Buoc2_Click(object sender, EventArgs e)
        {
            Buoc2 buoc2 = new Buoc2(thongTinChuyenDuocChon);
            btn_Buoc3.Enabled = false; // lúc đầu chưa chọn thì disable
            buoc2.DuLieuChonGheChanged += (thongTin, gheDaChon) =>
            {
                // Lưu vào biến toàn cục
                this.thongTinChuyenDuocChon = thongTin;
                this.gheDuocChon = gheDaChon;

                // Cập nhật trạng thái nút Bước 3
                btn_Buoc3.Enabled = gheDaChon.Count > 0;
            };
            ShowControl(buoc2);
        }

        private void btn_Buoc3_Click(object sender, EventArgs e)
        {
            Buoc3 buoc3 = new Buoc3(thongTinChuyenDuocChon, gheDuocChon);
            ShowControl(buoc3);
        }
    }
}
