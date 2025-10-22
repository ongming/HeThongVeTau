using Guna.UI2.WinForms;
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
    public partial class Buoc1 : UserControl
    {
        private UC_ChuyenTauCard cardDuocChon = null;
        public event Action<ThongTinChuyenTau> ChuyenTauDaChon;
        public Buoc1()
        {
            InitializeComponent();
        }

        private void LoadDanhSachChuyenTau(object sender, EventArgs e)
        {
            flow_ChuyenTau.Controls.Clear(); // Xóa card cũ
            DataTable dsChuyen = LayDanhSachChuyenTau(); // Lấy danh sách từ DB

            foreach (DataRow row in dsChuyen.Rows)
            {
                UC_ChuyenTauCard card = new UC_ChuyenTauCard();
                card.LoadData(row);

                card.CardSelected += (s, e2) =>
                {
                    foreach (UC_ChuyenTauCard c in flow_ChuyenTau.Controls)
                        c.DeselectCard();

                    var selected = (UC_ChuyenTauCard)s;
                    cardDuocChon = selected;
                    selected.SelectCard();

                    // 🧩 Tạo object chứa đầy đủ thông tin chuyến tàu
                    var thongTin = new ThongTinChuyenTau
                    {
                        MaChuyen = cardDuocChon.MaChuyen,
                        Tuyen = cardDuocChon.lblTuyen.Text,
                        Ngay = cardDuocChon.lblNgay.Text,
                        Gio = cardDuocChon.lblGio.Text,
                        Gia = cardDuocChon.lblGia.Text
                    };

                    // 🔹 Gửi object về control cha (MuaVe)
                    ChuyenTauDaChon?.Invoke(thongTin);
                };

                flow_ChuyenTau.Controls.Add(card);
            }
        }


        public DataTable LayDanhSachChuyenTau()
        {
            string query = @"
            SELECT * 
            FROM CHUYENTAU
            WHERE 
                DATEADD(
                    SECOND, 
                    DATEDIFF(SECOND, 0, GioDi), 
                    CAST(NgayDi AS DATETIME)
                ) >= GETDATE()
            ORDER BY NgayDi, GioDi ASC";

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

    }
}
