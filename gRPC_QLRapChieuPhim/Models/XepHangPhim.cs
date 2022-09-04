using System;
using System.Collections.Generic;

#nullable disable

namespace gRPC_QLRapChieuPhim.Models
{
    public partial class XepHangPhim
    {
        public XepHangPhim()
        {
            Phims = new HashSet<Phim>();
        }

        public int Id { get; set; }
        public string KyHieu { get; set; }
        public string Ten { get; set; }

        public virtual ICollection<Phim> Phims { get; set; }
    }
}
