using System;
using System.Collections.Generic;

#nullable disable

namespace gRPC_QLRapChieuPhim.Models
{
    public partial class Phong
    {
        public Phong()
        {
            Ghes = new HashSet<Ghe>();
            LichChieus = new HashSet<LichChieu>();
        }

        public int Id { get; set; }
        public string TenPhong { get; set; }
        public int SoLuongGhe { get; set; }
        public string GhiChu { get; set; }
        public int RapId { get; set; }

        public virtual Rap Rap { get; set; }
        public virtual ICollection<Ghe> Ghes { get; set; }
        public virtual ICollection<LichChieu> LichChieus { get; set; }
    }
}
