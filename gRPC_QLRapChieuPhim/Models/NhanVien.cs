using System;
using System.Collections.Generic;

#nullable disable

namespace gRPC_QLRapChieuPhim.Models
{
    public partial class NhanVien
    {
        public NhanVien()
        {
            Ves = new HashSet<Ve>();
        }

        public int Id { get; set; }
        public string HoTen { get; set; }
        public bool GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string Cmnd { get; set; }
        public string MatKhau { get; set; }
        public int RapId { get; set; }
        public string QuyenHan { get; set; }

        public virtual ICollection<Ve> Ves { get; set; }
    }
}
