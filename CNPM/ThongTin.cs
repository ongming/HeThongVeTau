using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YourNamespace;

namespace CNPM
{
    public partial class ThongTin : UserControl
    {
        int MaUser;
        string Role;
        bool click = false;
        public ThongTin(int MaUser, string Role)
        {
            this.MaUser = MaUser;
            this.Role = Role;
            InitializeComponent();
            LoadThongTin();
            //ModernGridStyle.ApplyPlaceholder(date_NgaySinh,null);
        }
        private void LoadThongTin()
        {
            // Ví dụ: gọi repository để lấy dữ liệu từ bảng tương ứng
            DataTable dt = NguoiDungRepository.LayThongTin(MaUser, Role);

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                txt_HovaTen.Text = row["HoTen"].ToString();
                txt_Email.Text = row["Gmail"].ToString();
                txt_SDT.Text = row["SoDienThoai"].ToString();
                txt_DiaChi.Text = row["DiaChi"].ToString();
                date_NgaySinh.Value = row["NgaySinh"] != DBNull.Value ? Convert.ToDateTime(row["NgaySinh"]) : DateTime.Now;
                txt_Username.Text = row["TenDangNhap"].ToString();
                txt_CCCD.Text = row["CCCD"].ToString();
                lb_VaiTro.Text = Role;
                lb_Email.Text = row["Gmail"].ToString();
                lb_Ten.Text = row["HoTen"].ToString();

                // 🖼️ Load avatar binary
                if (row["Avatar"] != DBNull.Value)
                {
                    byte[] bytes = (byte[])row["Avatar"];
                    using (var ms = new MemoryStream(bytes))
                        pic_Avatar.Image = Image.FromStream(ms);
                }
                else
                {
                    pic_Avatar.Image = Properties.Resources.androgynous_avatar_non_binary_queer_person;
                }
            }
        }

        private void pic_Avatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                click = true;
                ofd.Title = "Chọn ảnh đại diện";
                ofd.Filter = "Ảnh (*.jpg; *.jpeg; *.png)|*.jpg;*.jpeg;*.png";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Hiển thị ảnh lên PictureBox
                    pic_Avatar.Image = Image.FromFile(ofd.FileName);
                    pic_Avatar.Tag = ofd.FileName; // lưu đường dẫn tạm thời để dùng khi lưu DB
                }
            }
        }

        private void btn_Luu_Click(object sender, EventArgs e)
        {

            try
            {
                // Kiểm tra thông tin hợp lệ
                if (string.IsNullOrWhiteSpace(txt_HovaTen.Text) ||
                    string.IsNullOrWhiteSpace(txt_SDT.Text))
                {
                    MessageBox.Show("⚠️ Vui lòng nhập đầy đủ thông tin bắt buộc.",
                        "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xử lý ảnh nếu có
                byte[] avatarData = null;
                if (pic_Avatar.Tag != null)
                {
                    string imagePath = pic_Avatar.Tag.ToString();
                    avatarData = File.ReadAllBytes(imagePath);
                }              

                if(avatarData != null)
                {
                    NguoiDungRepository.CapNhatThongTin(
                    MaUser,
                    Role,
                    txt_HovaTen.Text.Trim(),
                    txt_CCCD.Text.Trim(),
                    date_NgaySinh.Value,
                    txt_DiaChi.Text.Trim(),
                    txt_SDT.Text.Trim(),
                    txt_Email.Text.Trim(),
                    txt_Username.Text.Trim(),
                    avatarData
                );
                }
                else
                {
                    // Gọi repository để cập nhật
                    NguoiDungRepository.CapNhatThongTin(
                        MaUser,
                        Role,
                        txt_HovaTen.Text.Trim(),
                        txt_CCCD.Text.Trim(),
                        date_NgaySinh.Value,
                        txt_DiaChi.Text.Trim(),
                        txt_SDT.Text.Trim(),
                        txt_Email.Text.Trim(),
                        txt_Username.Text.Trim()
                    );
                }

                    MessageBox.Show("✅ Lưu thay đổi thành công!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Lỗi khi lưu thông tin: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_ChinhSua_Click(object sender, EventArgs e)
        {
            txt_Username.ReadOnly = false;
            txt_CCCD.ReadOnly = false;
            txt_DiaChi.ReadOnly = false;
            date_NgaySinh.Enabled = true;
            txt_SDT.ReadOnly = false;
            txt_Email.ReadOnly = false;
            txt_Username.ReadOnly = false;
        }
    }
}
