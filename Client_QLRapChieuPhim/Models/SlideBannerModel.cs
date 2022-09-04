using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Client_QLRapChieuPhim.Models
{
    public class SlideBannerModel
    {
        public class SlideBannerBase
        {
            public int Id { get; set; }

            [Display(Name = "Tên banner")]
            [Required(ErrorMessage = "Tên phải khác rỗng")]
            public string Ten { get; set; }

            [Display(Name = "Hình banner")]
            [Required(ErrorMessage = "Hình phải khác rỗng")]
            public string Hinh { get; set; }

            [Display(Name = "Liên kết (URL)")]
            public string LienKet { get; set; }

            [Display(Name = "Kích hoạt")]
            public bool KichHoat { get; set; }
        }

        public class Input
        { }

        public class Output
        {
            public class ThongTinSlideBanner : SlideBannerBase
            { }
        }
    }
}