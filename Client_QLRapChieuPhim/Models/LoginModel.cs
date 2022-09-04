using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Client_QLRapChieuPhim.Models
{
    public class LoginModel
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Please enter your username.")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Please enter your password.")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool Remember { get; set; }
    }
}