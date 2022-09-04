using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Client_QLRapChieuPhim.Models
{
    public class ThanhVienModel
    {
        public class ThanhVienBase
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Họ tên phải khác rỗng")]
            [Display(Name = "Họ tên")]
            public string HoTen { get; set; }

            [Display(Name = "Giới tính")]
            public bool GioiTinh { get; set; }

            [Display(Name = "Ngày sinh")]
            [DataType(DataType.Date)]
            public DateTime NgaySinh { get; set; }

            [Required(ErrorMessage = "Email phải khác rỗng")]
            [Display(Name = "Email")]
            [DataType(DataType.EmailAddress)]
            [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Điện thoại phải khác rỗng")]
            [Display(Name = "Điện thoại")]
            public string DienThoai { get; set; }

            [Required(ErrorMessage = "Mật khẩu phải khác rỗng")]
            [Display(Name = "Mật khẩu")]
            [DataType(DataType.Password)]
            public string MatKhau { get; set; }

            public bool KichHoat { get; set; }
        }

        public class Input
        {
            public class ThongTinThanhVien : ThanhVienBase
            { }
        }

        public class Output
        {
            public class ThongTinThanhVien : ThanhVienBase
            {
                [Required(ErrorMessage = "Xác nhận Mật khẩu phải khác rỗng")]
                [Display(Name = "Xác nhận mật khẩu")]
                [Compare("MatKhau", ErrorMessage = "Xác nhận lại mật khẩu không đúng.")]
                [DataType(DataType.Password)]
                public string XacNhanMatKhau { get; set; }
            }
        }
    }
}