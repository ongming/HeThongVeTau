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
    public partial class ChiTietVe : Form
    {
        int mave, makh;
        ThongTinNhanVien nv;
        string tenkh,sodt,cccd,soghe,loaighe,giatien,ngaydat;

        private void btn_LuuThongTIn_Click(object sender, EventArgs e)
        {
            // 🔹 Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(txt_HovaTen.Text))
            {
                MessageBox.Show("⚠️ Họ và tên không được để trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_HovaTen.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_SDT.Text))
            {
                MessageBox.Show("⚠️ Số điện thoại không được để trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_SDT.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_CCCD.Text))
            {
                MessageBox.Show("⚠️ CCCD không được để trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_CCCD.Focus();
                return;
            }
            bool thanhCong = NhanVienRepository.CapNhatThongTinKhachHang(mave, txt_HovaTen.Text, txt_SDT.Text, txt_CCCD.Text);
            if (thanhCong)
            {
                // 1️⃣ Ghi thông báo cho khách hàng
                string noiDung = $"Vé của bạn đã được nhân viên {nv.HoTen} cập nhật thông tin.";
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    string queryThongBao = @"
                    INSERT INTO THONGBAO_KH (NoiDung, MaKhachHang, ThoiGian, DaXem)
                    VALUES (@NoiDung, @MaKhachHang, GETDATE(), 0)";

                    SqlCommand cmdTB = new SqlCommand(queryThongBao, conn);
                    cmdTB.Parameters.AddWithValue("@NoiDung", noiDung);
                    cmdTB.Parameters.AddWithValue("@MaKhachHang", makh);

                    conn.Open();
                    cmdTB.ExecuteNonQuery();
                    conn.Close();
                }

                // 2️⃣ Ghi nhật ký hoạt động
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    string queryNhatKy = @"
                    INSERT INTO NHATKY_HOATDONG (MaNhanVien, HanhDong, ThoiGian)
                    VALUES (@MaNhanVien, @HanhDong, GETDATE())";

                    string hanhDong = $"Nhân viên {nv.HoTen} đã sửa vé mã {mave} của khách hàng mã {makh}.";

                    SqlCommand cmdNK = new SqlCommand(queryNhatKy, conn);
                    cmdNK.Parameters.AddWithValue("@MaNhanVien", nv.MaNhanVien);
                    cmdNK.Parameters.AddWithValue("@HanhDong", hanhDong);

                    conn.Open();
                    cmdNK.ExecuteNonQuery();
                    conn.Close();
                }

                MessageBox.Show("✅ Cập nhật thông tin khách hàng thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
                MessageBox.Show("❌ Không thể cập nhật thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txt_HovaTen.ReadOnly = true;
            txt_HovaTen.BorderThickness = 0;
            txt_SDT.ReadOnly = true;
            txt_SDT.BorderThickness = 0;
            txt_CCCD.ReadOnly = true;
            txt_CCCD.BorderThickness = 0;
            btn_LuuThongTIn.Visible = false;
            btn_ChinhSuaChuyenTau.Visible = true;
        }

        private void btn_ChinhSuaChuyenTau_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show(
       "Bạn có chắc muốn chỉnh sửa thông tin khách hàng không?",
       "Xác nhận chỉnh sửa",
       MessageBoxButtons.YesNo,
       MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                txt_HovaTen.ReadOnly = false;
                txt_HovaTen.BackColor = Color.Gainsboro;
                txt_SDT.ReadOnly = false;
                txt_SDT.BackColor = Color.Gainsboro;
                txt_CCCD.ReadOnly = false;
                txt_CCCD.BackColor = Color.Gainsboro;
                btn_LuuThongTIn.Visible = true;

                MessageBox.Show("Đã mở khóa, bạn có thể chỉnh sửa thông tin.",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                btn_ChinhSuaChuyenTau.Visible = false;
            }
        }

        public ChiTietVe(ThongTinNhanVien  nv, string maVe, int MaKhachHang, string tenKH, string soDT, string cccd, string SoGhe, string loaiGhe, string giaTien, string ngayDat)
        {
            this.nv = nv;
            this.makh = MaKhachHang;
            this.mave = Convert.ToInt32(maVe);
            this.tenkh = tenKH;
            this.sodt = soDT;
            this.cccd = cccd;
            this.soghe = SoGhe;
            this.loaighe = loaiGhe;
            this.giatien = giaTien;
            this.ngaydat = ngayDat;
            InitializeComponent();
            
            LoadThongTinVe(mave);
        }

        private void LoadThongTinVe(int maVe)
        {
            DataRow ve = NhanVienRepository.LayThongTinVe(maVe);
            if (ve == null)
            {
                MessageBox.Show("Không tìm thấy thông tin vé.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 🔹 Thông tin khách hàng
            txt_HovaTen.Text = tenkh;
            txt_SDT.Text = sodt;
            txt_CCCD.Text = cccd;
            lb_MaVe.Text = mave.ToString();
            lb_NgayDat.Text = ngaydat;

            // 🔹 Thông tin chuyến tàu
            lb_MaChuyen.Text = ve["MaChuyen"].ToString();
            lb_Tuyen.Text = ve["NoiDi"] + " → " + ve["NoiDen"];
            lb_LoaiGhe.Text = loaighe;
            lb_NgayDi.Text = Convert.ToDateTime(ve["NgayDi"]).ToString("dd/MM/yyyy");
            lb_Gio.Text = ve["GioDi"].ToString() + " - " + ve["GioDen"].ToString();
            lb_SoGhe.Text = soghe;

            // 🔹 Giá vé và thanh toán
            lb_GiaVe.Text = giatien;
            lb_ThanhToan.Text = ve["PhuongThucThanhToan"].ToString();
        }
        private void lb_Closed_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
