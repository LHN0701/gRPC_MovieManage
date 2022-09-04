using System;
using System.Collections.Generic;

#nullable disable

namespace gRPC_QLRapChieuPhim.Models
{
    public partial class SlideBanner
    {
        public int Id { get; set; }
        public string Ten { get; set; }
        public string Hinh { get; set; }
        public string LienKet { get; set; }
        public bool KichHoat { get; set; }
    }
}
