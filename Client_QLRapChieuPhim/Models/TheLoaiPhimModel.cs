using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client_QLRapChieuPhim.Models
{
    public class TheLoaiPhimModel
    {
        public class TheLoaiPhimBase
        {
            public int Id { get; set; }
            public string Ten { get; set; }
        }

        public class Input
        { }

        public class Output
        {
            public class ThongTinTheLoaiPhim : TheLoaiPhimBase
            {
                public List<PhimModel.PhimBase> DanhSachPhim { get; set; }

                public ThongTinTheLoaiPhim()
                {
                    DanhSachPhim = new List<PhimModel.PhimBase>();
                }
            }
        }
    }
}