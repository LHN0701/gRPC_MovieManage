using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Client_QLRapChieuPhim.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Mật khẩu cũ phải khác rỗng.")]
        [Display(Name = "Mật khẩu cũ")]
        [DataType(DataType.Password)]
        public string MatKhauCu { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới phải khác rỗng.")]
        [Display(Name = "Mật khẩu mới")]
        [DataType(DataType.Password)]
        public string MatKhauMoi { get; set; }

        [Required(ErrorMessage = "Nhập lại Mật khẩu mới phải khác rỗng.")]
        [Display(Name = "Nhập lại Mật khẩu mới")]
        [DataType(DataType.Password)]
        [Compare("MatKhauMoi", ErrorMessage = "Nhập lại Mật khẩu mới không chính xác.")]
        public string NhapLaiMatKhauMoi { get; set; }
    }
}