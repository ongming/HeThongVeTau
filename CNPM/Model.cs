using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNPM
{
    
        public class ThongTinChuyenTau
        {
            public int MaChuyen { get; set; }
            public string Tuyen { get; set; }     // VD: "TPHCM - Cần Thơ"
            public string Ngay { get; set; }      // "23/07/2024"
            public string Gio { get; set; }       // "07:30 - 10:00"
            public string Gia { get; set; }       // "180000 VND"
        }

        public class ThongTinKhachHang
    {
            public int MaKhachHang { get; set; }
            public string HoTen { get; set; }
            public string CCCD { get; set; }
            public string Gmail { get; set; }
            public string DienThoai { get; set; }
            public string DiaChi { get; set; }
        }
        public class NguoiSuDungVe
        {
            public string TenNguoiSuDung { get; set; }
            public string SoDienThoai { get; set; }
            public string CCCD { get; set; }
        }

        public class ThongTinNhanVien
        {
            public int MaNhanVien { get; set; }
            public string HoTen { get; set; }
            public string CCCD { get; set; }
            public string Gmail { get; set; }
            public string DienThoai { get; set; }
            public string DiaChi { get; set; }
            public string VaiTro { get; set; }   // "NhanVien" hoặc "QuanLy"
            public DateTime NgayTao { get; set; }
            public bool TrangThai { get; set; }  // true = hoạt động, false = bị khóa
        }

}
