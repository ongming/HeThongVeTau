using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    public partial class NganHang : UserControl
    {
        public NganHang()
        {
            InitializeComponent();
        }
        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private void btn_TaoMa_Click(object sender, EventArgs e)
        {
            try
            {
                // 🔹 Độ dài mã (Nam có thể chỉnh)
                int length = 10;
                string prefix = "";

                // 🔹 StringBuilder để nối ký tự
                var sb = new StringBuilder(prefix);

                // 🔹 Tạo mảng byte ngẫu nhiên
                byte[] data = new byte[length];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(data);
                }

                // 🔹 Map từng byte thành ký tự trong CHARS
                for (int i = 0; i < length; i++)
                {
                    int idx = data[i] % CHARS.Length;
                    sb.Append(CHARS[idx]);
                }

                // 🔹 Hiển thị mã ra textbox hoặc label
                txt_Ma.Text = sb.ToString();

                // (tuỳ chọn) Copy luôn mã vào clipboard
                Clipboard.SetText(sb.ToString());
                MessageBox.Show("Đã tạo mã thanh toán: " + sb.ToString(), "Thông báo");
                btn_TaoMa.Enabled = false; // disable nút sau khi tạo mã
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo mã: " + ex.Message);
            }

        }
    }
}
