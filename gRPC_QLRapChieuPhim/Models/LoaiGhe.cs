using System;
using System.Collections.Generic;

#nullable disable

namespace gRPC_QLRapChieuPhim.Models
{
    public partial class LoaiGhe
    {
        public LoaiGhe()
        {
            Ghes = new HashSet<Ghe>();
        }

        public int Id { get; set; }
        public string TenLoaiGhe { get; set; }

        public virtual ICollection<Ghe> Ghes { get; set; }
    }
}
