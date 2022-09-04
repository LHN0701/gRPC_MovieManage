using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client_QLRapChieuPhim.Models
{
    public class XepHangPhimModel
    {
        public class XepHangPhimBase
        {
            public int Id { get; set; }
            public string KyHieu { get; set; }
            public string Ten { get; set; }
        }

        public class Input
        { }

        public class Output
        {
            public class ThongTinXepHangPhim : XepHangPhimBase
            {
                public List<PhimModel.PhimBase> DanhSachPhim { get; set; }

                public ThongTinXepHangPhim()
                {
                    DanhSachPhim = new List<PhimModel.PhimBase>();
                }
            }
        }
    }
}