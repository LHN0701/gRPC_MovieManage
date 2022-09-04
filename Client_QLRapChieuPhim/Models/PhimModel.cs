using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Client_QLRapChieuPhim.Models
{
    public class PhimModel
    {
        public class PhimBase
        {
            [ScaffoldColumn(false)]
            public int Id { get; set; }

            [Required(ErrorMessage = "Tên phim phải khác rỗng")]
            [MaxLength(100, ErrorMessage = "Tên phim tối đa 100 ký tự")]
            [Display(Name = "Tên phim")]
            public string TenPhim { get; set; }

            [Display(Name = "Tên gốc")]
            public string TenGoc { get; set; }

            [Required(ErrorMessage = "Thời lượng phải > 0")]
            [RegularExpression("([0-9]+)", ErrorMessage = "Thời lượng phải là số nguyên")]
            [Range(1, int.MaxValue, ErrorMessage = "Thời lượng phải > 0")]
            [Display(Name = "Thời lượng")]
            public int ThoiLuong { get; set; }

            [Required(ErrorMessage = "Đạo diễn phải khác rỗng")]
            [Display(Name = "Đạo diễn")]
            public string DaoDien { get; set; }

            [Required(ErrorMessage = "Diễn viên phải khác rỗng")]
            [Display(Name = "Diễn viên")]
            public string DienVien { get; set; }

            [Display(Name = "Ngày khởi chiếu")]
            public DateTime NgayKhoiChieu { get; set; }

            [Display(Name = "Nội dung phim")]
            public string NoiDung { get; set; }

            [Display(Name = "Nước sản xuất")]
            public string NuocSanXuat { get; set; }

            [Display(Name = "Nhà sản xuất")]
            public string NhaSanXuat { get; set; }

            [Display(Name = "Poster phim")]
            public string Poster { get; set; }

            [Required(ErrorMessage = "Phim phải thuộc ít nhất một thể loại")]
            [Display(Name = "Danh sách thể loại")]
            public string DanhSachTheLoaiId { get; set; }

            [Display(Name = "Ngôn ngữ")]
            public string NgonNgu { get; set; }

            [Required(ErrorMessage = "Phim phải được xếp hạng")]
            [Display(Name = "Xếp hạng")]
            public int XepHangPhimId { get; set; }

            [Display(Name = "Link Trailer")]
            public string Trailer { get; set; }
        }

        public class Input
        {
            public class ThongTinPhim
            {
                public int PhimId { get; set; }
            }

            public class TimThongTinPhim
            {
                public string TuKhoa { get; set; }
                public int PageSize { get; set; }
                public int CurrentPage { get; set; }
            }

            public class PhimTheoTheLoai
            {
                public int TheLoaiId { get; set; }
                public int PageSize { get; set; }
                public int CurrentPage { get; set; }
            }
        }

        public class Output
        {
            public class ThongTinPhim : PhimBase
            {
                public XepHangPhimModel.XepHangPhimBase XepHangPhim { get; set; }
                public List<TheLoaiPhimModel.TheLoaiPhimBase> DanhSachTheLoai { get; set; }

                public ThongTinPhim()
                {
                    XepHangPhim = new XepHangPhimModel.XepHangPhimBase();
                    DanhSachTheLoai = new List<TheLoaiPhimModel.TheLoaiPhimBase>();
                }
            }

            public class PhimTheoTheLoai
            {
                public TheLoaiPhimModel.TheLoaiPhimBase TheLoaiHienHanh { get; set; }
                public List<TheLoaiPhimModel.TheLoaiPhimBase> DanhSachTheLoai { get; set; }
                public List<ThongTinPhim> DanhSachPhim { get; set; }
                public int CurrentPage { get; set; }
                public int PageCount { get; set; }

                public PhimTheoTheLoai()
                {
                    TheLoaiHienHanh = new TheLoaiPhimModel.TheLoaiPhimBase();
                    DanhSachPhim = new List<ThongTinPhim>();
                    DanhSachTheLoai = new List<TheLoaiPhimModel.TheLoaiPhimBase>();
                }
            }

            public class ThemPhimMoi : PhimBase
            {
                public List<XepHangPhimModel.XepHangPhimBase> DanhSachXepHangPhim { get; set; }
                public List<TheLoaiPhimModel.TheLoaiPhimBase> DanhSachTheLoai { get; set; }
                public string ThongBao { get; set; }

                public ThemPhimMoi()
                {
                    DanhSachTheLoai = new List<TheLoaiPhimModel.TheLoaiPhimBase>();
                    DanhSachXepHangPhim = new List<XepHangPhimModel.XepHangPhimBase>();
                }
            }

            public class CapNhatPhim : PhimBase
            {
                public List<XepHangPhimModel.XepHangPhimBase> DanhSachXepHangPhim { get; set; }
                public List<TheLoaiPhimModel.TheLoaiPhimBase> DanhSachTheLoai { get; set; }
                public string ThongBao { get; set; }

                public CapNhatPhim()
                {
                    DanhSachTheLoai = new List<TheLoaiPhimModel.TheLoaiPhimBase>();
                    DanhSachXepHangPhim = new List<XepHangPhimModel.XepHangPhimBase>();
                }
            }
        }
    }
}