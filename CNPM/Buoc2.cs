using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    public partial class Buoc2 : UserControl
    {
        ThongTinChuyenTau ThongTinChuyenTau;
        public event Action<ThongTinChuyenTau, List<int>> DuLieuChonGheChanged;

        public Buoc2(ThongTinChuyenTau thongTinChuyenTau)
        {
            InitializeComponent();
            ThongTinChuyenTau = thongTinChuyenTau;
            LoadDanhSachGhe(ThongTinChuyenTau.MaChuyen);

        }
        private void CapNhatTrangThaiBuoc3()
        {
            bool coGhe = flow_GheDaChon.Controls.Count > 0;

            // 🔹 Lấy danh sách số ghế (chỉ số thôi)
            var gheDaChon = flow_GheDaChon.Controls
                   .OfType<Guna2Button>()
                   .Select(b => int.Parse(b.Text.Replace("Ghế ", "")))
                   .ToList();

            // 🔹 Báo cho form cha biết có ghế hay không + dữ liệu ghế
            DuLieuChonGheChanged?.Invoke(ThongTinChuyenTau, gheDaChon);
        }
        private void LoadDanhSachGhe(int maChuyen)
        {
            lblTuyen.Text = ThongTinChuyenTau.Tuyen;
            lblNgay.Text = ThongTinChuyenTau.Ngay;
            lblGio.Text = ThongTinChuyenTau.Gio;
            lblGia.Text = ThongTinChuyenTau.Gia;
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                // Không cần open/close thủ công
                // Vì SqlDataAdapter và ExecuteScalar tự open khi cần

                // 1️⃣ Lấy tổng số ghế của chuyến tàu
                int tongGhe = 0;
                using (SqlCommand cmdTong = new SqlCommand("SELECT TongSoGhe FROM ChuyenTau WHERE MaChuyen = @ma", conn))
                {
                    cmdTong.Parameters.AddWithValue("@ma", maChuyen);
                    conn.Open();
                    tongGhe = Convert.ToInt32(cmdTong.ExecuteScalar());
                    conn.Close();
                }

                // 2️⃣ Lấy danh sách ghế đã được đặt trong bảng Ve
                List<string> gheDaDat = new List<string>();
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT SoGhe FROM Ve WHERE MaChuyen = @ma", conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@ma", maChuyen);
                    DataTable dtVe = new DataTable();
                    da.Fill(dtVe);
                    gheDaDat = dtVe.AsEnumerable()
                                   .Select(r => r["SoGhe"].ToString())
                                   .ToList();
                }

                // 3️⃣ Xóa các ghế cũ (nếu có)
                panel_Ghe1.Controls.Clear();
                panel_Ghe2.Controls.Clear();
                panel_Ghe3.Controls.Clear();
                panel_Ghe4.Controls.Clear();

                // 4️⃣ Tạo và bố trí ghế trong 4 panel (dọc xuống)
                int soCot = 4;
                int spacing = 10;      // khoảng cách giữa các ghế
                int gheHeight = 45;
                int gheWidth = 55;
                int y1 = 0, y2 = 0, y3 = 0, y4 = 0;

                for (int i = 1; i <= tongGhe; i++)
                {
                    int gheIndex = i;
                    int cot = (i - 1) % soCot;

                    Guna2Button ghe = new Guna2Button();
                    ghe.GetType().GetProperty("ButtonMode").SetValue(ghe, Guna.UI2.WinForms.Enums.ButtonMode.ToogleButton);
                    ghe.Text = i.ToString();
                    ghe.Width = gheWidth;
                    ghe.Height = gheHeight;
                    ghe.BorderRadius = 8;
                    ghe.Margin = new Padding(0);
                    ghe.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    ghe.Cursor = Cursors.Hand;
                    ghe.BorderColor = Color.Gray;
                    ghe.BorderThickness = 1;
                    ghe.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;

                    if (gheDaDat.Contains(i.ToString()))
                    {
                        ghe.Enabled = false;
                        ghe.DisabledState.FillColor = Color.RoyalBlue;
                        ghe.DisabledState.ForeColor = Color.White;
                    }
                    else
                    {
                        if (i <= 20)
                        {
                            ghe.FillColor = Color.Yellow;
                        }
                        else
                        {
                            ghe.FillColor = Color.White;
                        }                       
                        ghe.ForeColor = Color.Black;
                    }

                    ghe.CheckedChanged += (s, e) =>
                    {
                        var btn = (Guna2Button)s;

                        if (btn.Checked)
                        {
                            btn.FillColor = Color.LightGray;
                            btn.CheckedState.FillColor = Color.LightGray; // 💥 Bắt Guna giữ màu khi checked

                            // 🔹 Đặt Tag là chính button gốc
                            Guna2Button gheChon = new Guna2Button();
                            gheChon.Text = "Ghế " + btn.Text;
                            gheChon.Tag = btn;
                            gheChon.Width = 65;
                            gheChon.Height = 35;
                            gheChon.BorderRadius = 8;
                            gheChon.BorderColor = Color.Gray;
                            gheChon.BorderThickness = 1;
                            gheChon.FillColor = Color.WhiteSmoke;
                            gheChon.ForeColor = Color.Black;
                            gheChon.Cursor = Cursors.Hand;
                            gheChon.Animated = true;
                            gheChon.HoverState.FillColor = Color.LightCoral;

                            // 🧩 Khi click vào ghế trong danh sách
                            gheChon.Click += (s2, e2) =>
                            {
                                var gheGoc = (Guna2Button)gheChon.Tag;
                                gheGoc.Checked = false;
                                flow_GheDaChon.Controls.Remove(gheChon);
                                gheChon.Dispose();
                                CapNhatTrangThaiBuoc3();
                            };

                            flow_GheDaChon.Controls.Add(gheChon);
                            CapNhatTrangThaiBuoc3();
                        }
                        else
                        {

                            btn.BeginInvoke((Action)(() =>
                            {
                                if (gheIndex <= 20)
                                    btn.FillColor = Color.Yellow;
                                else
                                    btn.FillColor = Color.White;
                            }));
                            foreach (Control c in flow_GheDaChon.Controls)
                            {
                                if (c is Guna2Button b && b.Tag == btn)
                                {
                                    flow_GheDaChon.Controls.Remove(b);
                                    b.Dispose();
                                    CapNhatTrangThaiBuoc3();
                                    break;
                                }
                            }
                        }

                    };


                    // 🧮 Đặt vị trí theo panel tương ứng (dọc)
                    switch (cot)
                    {
                        case 0:
                            ghe.Location = new Point((panel_Ghe1.ClientSize.Width - gheWidth) / 2, y1);
                            panel_Ghe1.Controls.Add(ghe);
                            y1 += gheHeight + spacing;
                            break;

                        case 1:
                            ghe.Location = new Point((panel_Ghe2.ClientSize.Width - gheWidth) / 2, y2);
                            panel_Ghe2.Controls.Add(ghe);
                            y2 += gheHeight + spacing;
                            break;

                        case 2:
                            ghe.Location = new Point((panel_Ghe3.ClientSize.Width - gheWidth) / 2, y3);
                            panel_Ghe3.Controls.Add(ghe);
                            y3 += gheHeight + spacing;
                            break;

                        case 3:
                            ghe.Location = new Point((panel_Ghe4.ClientSize.Width - gheWidth) / 2, y4);
                            panel_Ghe4.Controls.Add(ghe);
                            y4 += gheHeight + spacing;
                            break;
                    }
                }
            }

        }
    }
}
