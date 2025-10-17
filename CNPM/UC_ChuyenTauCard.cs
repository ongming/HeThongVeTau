using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace CNPM
{
    public partial class UC_ChuyenTauCard : UserControl
    {
        public int MaChuyen { get; set; }
        public event EventHandler CardSelected; // 🔹 sự kiện báo cho Form cha biết
        private bool isSelected = false;


        public UC_ChuyenTauCard()
        {
            InitializeComponent();
            this.Margin = new Padding(14);
            // đăng ký click cho toàn bộ vùng card
            this.Click += Card_Click;
            foreach (Control c in this.Controls)
                c.Click += Card_Click;
        }
        private void Card_Click(object sender, EventArgs e)
        {
            CardSelected?.Invoke(this, EventArgs.Empty); // báo cho form cha
        }
        public void SelectCard()
        {
            isSelected = true;
            guna2Panel1.BorderColor = Color.DodgerBlue;
            guna2Panel1.FillColor = SystemColors.Control;
        }

        // 🔹 Bỏ chọn
        public void DeselectCard()
        {
            isSelected = false;
            guna2Panel1.BorderColor = Color.LightGray;
            guna2Panel1.FillColor = Color.White;
        }
        // Hàm nạp dữ liệu từ DataRow
        public void LoadData(DataRow data)
        {
            MaChuyen = (int)data["MaChuyen"];
            lblTuyen.Text = $"{data["NoiDi"]} - {data["NoiDen"]}";
            lblNgay.Text = $"{Convert.ToDateTime(data["NgayDi"]):dd/MM/yyyy}";
            lblGio.Text = $"{data["GioDi"]} - {data["GioDen"]}";
            lblGia.Text = $"{Convert.ToDecimal(data["GiaGheCung"]):#,##0} VND\n" +
                          $"{Convert.ToDecimal(data["GiaGheMem"]):#,##0} VND";
            lblGia.ForeColor = Color.RoyalBlue;
            lblGia.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        }

        // Hover effect
        private void pnlMain_MouseEnter(object sender, EventArgs e)
        {
            guna2Panel1.BackColor = Color.FromArgb(240, 245, 255);
        }

        private void pnlMain_MouseLeave(object sender, EventArgs e)
        {
            guna2Panel1.BackColor = Color.White;
        }
    }
}

