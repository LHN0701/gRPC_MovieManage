using System;
using System.Collections.Generic;

#nullable disable

namespace gRPC_QLRapChieuPhim.Models
{
    public partial class Ghe
    {
        public int Id { get; set; }
        public string SoGhe { get; set; }
        public int LoaiGheId { get; set; }
        public int PhongId { get; set; }
        public string Day { get; set; }
        public int Hang { get; set; }

        public virtual LoaiGhe LoaiGhe { get; set; }
        public virtual Phong Phong { get; set; }
    }
}
