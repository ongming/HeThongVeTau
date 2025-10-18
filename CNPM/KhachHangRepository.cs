using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CNPM
{
    public class KhachHangRepository
    {
        //kiểm tra đăng nhập
        public ThongTinKhachHang CheckLogin(string username, string password)
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"
                SELECT kh.MaKhachHang, kh.HoTen, kh.Gmail, kh.SoDienThoai, kh.DiaChi, kh.CCCD
                FROM TAIKHOAN t
                JOIN KHACHHANG kh ON t.MaLienKet = kh.MaKhachHang
                WHERE t.TenDangNhap = @user 
                  AND t.MatKhau = @pass 
                  AND t.VaiTro = 'KhachHang'
                  AND t.TrangThai = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@pass", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new ThongTinKhachHang
                                {
                                    MaKhachHang = reader.GetInt32(0),
                                    HoTen = reader.GetString(1),
                                    Gmail = reader.GetString(2),
                                    CCCD = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    DienThoai = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    DiaChi = reader.IsDBNull(4) ? "" : reader.GetString(4)
                                };
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu!\nChi tiết: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi đăng nhập!\nChi tiết: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // 🔹 Nếu không có kết quả nào, trả về null
            return null;
        }


        // đặt lại mật khẩu theo gmail
        public static void DatLaiMatKhau(string gmail)
        {
            try
            {
                using (SqlConnection conn = DatabaseConnection.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                UPDATE TAIKHOAN SET MatKhau = '123'
                WHERE MaTaiKhoan IN (
                    SELECT t.MaTaiKhoan FROM TAIKHOAN t
                    LEFT JOIN KHACHHANG kh ON t.MaLienKet = kh.MaKhachHang AND t.VaiTro = 'KhachHang'
                    LEFT JOIN NHANVIEN nv ON t.MaLienKet = nv.MaNhanVien AND t.VaiTro = 'NhanVien'
                    LEFT JOIN QUANLY ql ON t.MaLienKet = ql.MaQuanLy AND t.VaiTro = 'QuanLy'
                    WHERE kh.Gmail = @gmail OR nv.Gmail = @gmail OR ql.Gmail = @gmail
                )";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@gmail", gmail);
                        int rows = cmd.ExecuteNonQuery();

                        MessageBox.Show(rows > 0 ? "✅ Đặt lại mật khẩu thành công!" : "❌ Không tìm thấy Gmail!");
                    }
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("❌ Lỗi khi kết nối hoặc cập nhật dữ liệu!");
            }
        }

        // hàm đặt vé khi nhấn nút hoàn tất
        public static bool DatVe(
             int maKhachHang,
             int maChuyen,
             List<int> gheList,
             List<NguoiSuDungVe> nguoiSuDungList,
             decimal tongTien,
             string phuongThucThanhToan,
             decimal GheCung,
             decimal GheMem
            )
        {
            if (gheList.Count != nguoiSuDungList.Count)
            {
                MessageBox.Show("Số ghế và số người sử dụng không khớp!");
                return false;
            }

            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    // 🔹 1. Thêm vào LICHSUGIAODICH (tổng giao dịch)
                    string sqlGD = @"
                INSERT INTO LICHSUGIAODICH (MaKhachHang, ThoiGianDat, PhuongThucThanhToan, TongTien)
                OUTPUT INSERTED.MaGiaoDich
                VALUES (@makh, GETDATE(), @pttt, @tong)";

                    int maGiaoDich;
                    using (SqlCommand cmd = new SqlCommand(sqlGD, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@makh", maKhachHang);
                        cmd.Parameters.AddWithValue("@pttt", phuongThucThanhToan);
                        cmd.Parameters.AddWithValue("@tong", tongTien);
                        maGiaoDich = (int)cmd.ExecuteScalar();
                    }

                    // 🔹 2. Duyệt từng ghế để thêm vào CHITIETGIAODICH và VE
                    string sqlCT = @"
                INSERT INTO CHITIETGIAODICH (MaGiaoDich, SoGhe, LoaiGhe, GiaTien)
                VALUES (@magd, @soghe, @loaighe, @gia)";

                    string sqlVe = @"
                INSERT INTO VE (MaGiaoDich, MaChuyen, MaKhachHang, SoGhe, LoaiGhe, GiaTien, TenNguoiSoHuu, SoDienThoai, CCCD, NgayDat)
                VALUES (@magd, @machuyen, @makh, @soghe, @loaighe, @gia, @ten, @sdt, @cccd, GETDATE())";

                    for (int i = 0; i < gheList.Count; i++)
                    {
                        int ghe = gheList[i];
                        var nguoi = nguoiSuDungList[i];

                        string loaiGhe = ghe > 20 ? "GheCung" : "GheMem";
                        decimal giaTien = ghe > 20 ? GheCung : GheMem;

                        // 🔸 Thêm chi tiết giao dịch
                        using (SqlCommand cmdCT = new SqlCommand(sqlCT, conn, trans))
                        {
                            cmdCT.Parameters.AddWithValue("@magd", maGiaoDich);
                            cmdCT.Parameters.AddWithValue("@soghe", ghe);
                            cmdCT.Parameters.AddWithValue("@loaighe", loaiGhe);
                            cmdCT.Parameters.AddWithValue("@gia", giaTien);
                            cmdCT.ExecuteNonQuery();
                        }

                        // 🔸 Thêm vé tương ứng
                        using (SqlCommand cmdVe = new SqlCommand(sqlVe, conn, trans))
                        {
                            cmdVe.Parameters.AddWithValue("@machuyen", maChuyen);
                            cmdVe.Parameters.AddWithValue("@makh", maKhachHang);
                            cmdVe.Parameters.AddWithValue("@soghe", ghe);
                            cmdVe.Parameters.AddWithValue("@magd", maGiaoDich);
                            cmdVe.Parameters.AddWithValue("@loaighe", loaiGhe);
                            cmdVe.Parameters.AddWithValue("@gia", giaTien);
                            cmdVe.Parameters.AddWithValue("@ten", nguoi.TenNguoiSuDung);
                            cmdVe.Parameters.AddWithValue("@sdt", nguoi.SoDienThoai);
                            cmdVe.Parameters.AddWithValue("@cccd", nguoi.CCCD);
                            cmdVe.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("❌ Lỗi khi lưu giao dịch: " + ex.Message);
                    return false;
                }
            }
        }
        public static bool GuiYKien(int maKhachHang, int danhGia, string noiDung)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                conn.Open();

                try
                {
                    // 🔹 1. Kiểm tra xem hôm nay KH đã gửi chưa
                    string sqlCheck = @"
                    SELECT COUNT(*) 
                    FROM YKIENPHANHOI 
                    WHERE MaKhachHang = @makh 
                      AND CAST(NgayGopY AS DATE) = CAST(GETDATE() AS DATE)";

                    using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@makh", maKhachHang);
                        int count = (int)cmdCheck.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("❌ Bạn đã gửi ý kiến hôm nay rồi!\nVui lòng quay lại vào ngày mai.",
                                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    // 🔹 2. Thêm ý kiến mới
                    string sqlInsert = @"
                    INSERT INTO YKIENPHANHOI (MaKhachHang, NgayGopY, DanhGia, NoiDung)
                    VALUES (@makh, GETDATE(), @danhgia, @noidung)";

                    using (SqlCommand cmd = new SqlCommand(sqlInsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@makh", maKhachHang);
                        cmd.Parameters.AddWithValue("@danhgia", danhGia);
                        cmd.Parameters.AddWithValue("@noidung", noiDung);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("✅ Cảm ơn bạn đã gửi ý kiến phản hồi!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Lỗi khi gửi ý kiến: " + ex.Message);
                    return false;
                }
            }
        }
        public static DataTable LayLichSuTheoKhach(int maKhachHang)
        {
            using (SqlConnection conn = DatabaseConnection.GetConnection())
            {
                string query = @"
                SELECT 
                    MaGiaoDich,
                    PhuongThucThanhToan,
                    TongTien,
                    ThoiGianDat
                FROM LICHSUGIAODICH
                WHERE MaKhachHang = @makh
                ORDER BY ThoiGianDat DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@makh", maKhachHang);

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

    }
}
