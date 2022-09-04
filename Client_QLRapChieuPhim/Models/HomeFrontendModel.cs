using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client_QLRapChieuPhim.Models
{
    public class HomeFrontendModel
    {
        public List<SlideBannerModel.SlideBannerBase> DanhSachBanner { get; set; }

        public HomeFrontendModel()
        {
            DanhSachBanner = new List<SlideBannerModel.SlideBannerBase>();
        }
    }
}