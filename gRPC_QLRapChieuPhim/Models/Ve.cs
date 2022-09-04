using System;
using System.Collections.Generic;

#nullable disable

namespace gRPC_QLRapChieuPhim.Models
{
    public partial class Ve
    {
        public int Id { get; set; }
        public DateTime NgayBanVe { get; set; }
        public string SoGhe { get; set; }
        public int GiaVe { get; set; }
        public int LichChieuId { get; set; }
        public int NhanVienId { get; set; }
        public int TinhTrang { get; set; }

        public virtual LichChieu LichChieu { get; set; }
        public virtual NhanVien NhanVien { get; set; }
    }
}
