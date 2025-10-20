using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Drawing.Drawing2D;

namespace CNPM
{
    public partial class Messenger : Form
    {
        int id_khachhang;
        bool isNhanVien; //true là khách hàng, false là nhân viên
        ThongTinNhanVien nv;
        public Messenger(int id_khachhang, ThongTinNhanVien nv)
        {
            InitializeComponent();
            this.nv = nv;
            this.id_khachhang = id_khachhang;
            this.isNhanVien = true;
            flowLayoutPanelMessages.AutoScroll = true;
            LoadMessages();
            name_user.Text = "tên khách hàng";
            display();
        }
        public Messenger(int id_khachhang)
        {
            InitializeComponent();
            this.id_khachhang = id_khachhang;
            this.isNhanVien = false;
            flowLayoutPanelMessages.AutoScroll = true;
            LoadMessages();
        }


        private void LoadMessages()
        {
            flowLayoutPanelMessages.Controls.Clear();

            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"
                SELECT MaKhachHang, MaNhanVienHoacQuanLy, Role, NoiDung, ThoiGian 
                FROM TinNhan
                WHERE MaKhachHang = @makh
                ORDER BY ThoiGian";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@makh", id_khachhang);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string message = reader["ThoiGian"].ToString()+reader["NoiDung"].ToString().ToLower();
                                bool isSender = ((reader["Role"]!= DBNull.Value) == isNhanVien);

                                AddMessageToPanel(EditMessage(message).Item1, EditMessage(message).Item2, isSender);
                            }
                        }

                    }
                }
                // Cuộn xuống dưới cùng sau khi tải xong tin nhắn
                flowLayoutPanelMessages.AutoScrollPosition = new Point(0, flowLayoutPanelMessages.VerticalScroll.Maximum);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải tin nhắn. Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddMessageToPanel(string message,int x, bool isSender)
        {
            // Tạo panel chứa tin nhắn
            Panel messagePanel = new Panel
            {
                AutoSize = true,
                Padding = new Padding(10),
                Margin = isSender ? new Padding(x, 5, 10, 5) : new Padding(10, 5, 150, 5),
                BackColor = isSender ? Color.FromArgb(204, 229, 255) : Color.FromArgb(204, 255, 204),
                MaximumSize = new Size(flowLayoutPanelMessages.Width - 40, 0),
            };

            // Bo tròn góc panel - chỉ khả thi với Region
            messagePanel.Paint += (s, e) =>
            {
                Panel p = (Panel)s;
                using (GraphicsPath path = new GraphicsPath())
                {
                    int radius = 15;
                    path.AddArc(0, 0, radius, radius, 180, 90);
                    path.AddArc(p.Width - radius, 0, radius, radius, 270, 90);
                    path.AddArc(p.Width - radius, p.Height - radius, radius, radius, 0, 90);
                    path.AddArc(0, p.Height - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();
                    p.Region = new Region(path);
                }
            };

            // Tạo label chứa nội dung tin nhắn
            Label messageLabel = new Label
            {
                Text = message,
                AutoSize = true,
                MaximumSize = new Size(300, 0), // Giới hạn chiều rộng tin nhắn
                //Font = new Font("Segoe UI", 10),
                Font = new Font("Consolas", 10),
                ForeColor = Color.Black,
            };

            messagePanel.Controls.Add(messageLabel);

            // Căn bên phải cho tin nhắn người gửi
            messagePanel.Anchor = isSender ? AnchorStyles.Right : AnchorStyles.Left;

            // Thêm vào flow panel
            flowLayoutPanelMessages.Controls.Add(messagePanel);
            flowLayoutPanelMessages.ScrollControlIntoView(messagePanel);
        }

        private (string, int) EditMessage(string input)
        {
            string firstPart = input.Substring(0, 14).Trim() + input[18] + input[19]; 

            string secondPart = input.Substring(20).Trim();
            int count = 0;
            int x = 200;
            if (secondPart.Length > 16)
            {
                int y = secondPart.Length;
                if (y > 23)
                {
                    y = 23;
                }
                x -= (int)((y-15) * 5.9);
            }    
            for (int i = 0; i < secondPart.Length; i++)
            {
                count += 1;
                if (count >= 20)
                {
                    if (secondPart[i].ToString() == " " || count == 23)
                    {
                        secondPart = secondPart.Substring(0, i) + '\n' + secondPart.Substring(i + 1);
                        count = 0;
                    }
                }
            }
            return ($"{firstPart}\n{secondPart}", x);
        }



        private void send_message_Click(object sender, EventArgs e)
        {
            real_send_message();
        }

        private void real_send_message()
        {
            string messageText;
            if (isNhanVien)
            {
                messageText = text_input.Text.Trim()+ "-" + nv.VaiTro.ToString() + nv.MaNhanVien.ToString();
            }
            else
            {
                messageText= text_input.Text.Trim();
            }
            if (string.IsNullOrEmpty(messageText))
            {
                MessageBox.Show("Vui lòng nhập nội dung tin nhắn.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    if (isNhanVien)
                    {
                        string query = @"
                        INSERT INTO TinNhan (MaKhachHang, MaNhanVienHoacQuanLy,Role , NoiDung, ThoiGian)
                        VALUES (@makh, @manv, @role, @MessageText, @SentAt)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@makh", id_khachhang);
                            cmd.Parameters.AddWithValue("@manv", nv.MaNhanVien);
                            cmd.Parameters.AddWithValue("@role", nv.VaiTro);
                            cmd.Parameters.AddWithValue("@MessageText", messageText);
                            cmd.Parameters.AddWithValue("@SentAt", DateTime.Now);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string query = @"
                        INSERT INTO TinNhan (MaKhachHang , NoiDung, ThoiGian)
                        VALUES (@makh, @MessageText, @SentAt)";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@makh", id_khachhang);
                            cmd.Parameters.AddWithValue("@MessageText", messageText);
                            cmd.Parameters.AddWithValue("@SentAt", DateTime.Now);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                text_input.Clear();
                LoadMessages();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể gửi tin nhắn. Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void display()
        {
            DataTable table = KhachHangRepository.LayThongTin(id_khachhang);
            if (table.Rows.Count > 0 && table.Rows[0]["Avatar"] != DBNull.Value)
            {
                byte[] avatarBytes = (byte[])table.Rows[0]["Avatar"];

                using (MemoryStream ms = new MemoryStream(avatarBytes))
                {
                    pictureBox_avatar.Image = Image.FromStream(ms);
                    pictureBox_avatar.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                name_user.Text=table.Rows[0]["HoTen"].ToString();
            }
            try
            {
                name_user.Text = table.Rows[0]["HoTen"].ToString();
            }
            catch (Exception ex) {
                MessageBox.Show("không thể load tên khách hàng");

            }
        }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pb_reset_Click(object sender, EventArgs e)
        {
            LoadMessages();
        }

        private void flowLayoutPanelMessages_Paint(object sender, PaintEventArgs e)
        {

        }

        private void text_input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter )
            {
                e.SuppressKeyPress = true;
                real_send_message();      
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
