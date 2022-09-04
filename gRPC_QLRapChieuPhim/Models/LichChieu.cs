using System;
using System.Collections.Generic;

#nullable disable

namespace gRPC_QLRapChieuPhim.Models
{
    public partial class LichChieu
    {
        public LichChieu()
        {
            Ves = new HashSet<Ve>();
        }

        public int Id { get; set; }
        public int SuatChieuId { get; set; }
        public int PhongId { get; set; }
        public int PhimId { get; set; }
        public DateTime NgayChieu { get; set; }

        public virtual Phim Phim { get; set; }
        public virtual Phong Phong { get; set; }
        public virtual SuatChieu SuatChieu { get; set; }
        public virtual ICollection<Ve> Ves { get; set; }
    }
}
