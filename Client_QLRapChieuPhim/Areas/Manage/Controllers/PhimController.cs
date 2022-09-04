using Client_QLRapChieuPhim.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using gRPC_QLRapChieuPhim;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Client_QLRapChieuPhim.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class PhimController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DanhSachPhim(int TheLoaiId, int CurrentPage, int PageSize)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5002");
            var client = new RapChieuPhim.RapChieuPhimClient(channel);

            Output.Types.Phims DSPhim = client.DanhSachPhimTheoTheLoai(new Input.Types.PhimTheoTheLoai()
            {
                IdTheLoai = TheLoaiId,
                CurrentPage = CurrentPage,
                PageSize = PageSize
            });
            PhimModel.Output.PhimTheoTheLoai model = new PhimModel.Output.PhimTheoTheLoai();
            if (DSPhim != null && DSPhim.Items.Count > 0)
            {
                var theLoais = client.DanhSachTheLoai(new Input.Types.Empty());
                var xepHangPhims = client.DanhSachXepHangPhim(new Input.Types.Empty());
                var dsTheLoai = theLoais.Items.ToList();
                foreach (var phim in DSPhim.Items)
                {
                    var thongtin = new PhimModel.Output.ThongTinPhim
                    {
                        Id = phim.Id,
                        TenPhim = phim.TenPhim,
                        TenGoc = phim.TenGoc,
                        DienVien = phim.DienVien,
                        DaoDien = phim.DaoDien,
                        NoiDung = phim.NoiDung,
                        NgayKhoiChieu = phim.NgayKhoiChieu.ToDateTime(),
                        NgonNgu = phim.NgonNgu,
                        NhaSanXuat = phim.NhaSanXuat,
                        NuocSanXuat = phim.NuocSanXuat,
                        Poster = phim.Poster,
                        ThoiLuong = phim.ThoiLuong,
                        Trailer = phim.Trailer,
                        DanhSachTheLoaiId = phim.DanhSachTheLoaiId,
                        XepHangPhimId = phim.XepHangPhimId
                    };
                    var dsIdTheLoai = phim.DanhSachTheLoaiId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    var theloai_phim = dsTheLoai.Where(x => dsIdTheLoai.Contains(x.Id.ToString())).ToList();
                    if (theloai_phim != null)
                        foreach (var tl in theloai_phim)
                        {
                            thongtin.DanhSachTheLoai.Add(new TheLoaiPhimModel.TheLoaiPhimBase
                            {
                                Id = tl.Id,
                                Ten = tl.Ten
                            });
                        }
                    var xepHang = xepHangPhims.Items.FirstOrDefault(x => x.Id.Equals(phim.XepHangPhimId));
                    if (xepHang != null)
                    {
                        thongtin.XepHangPhim = new XepHangPhimModel.XepHangPhimBase
                        {
                            Id = xepHang.Id,
                            Ten = xepHang.Ten,
                            KyHieu = xepHang.KyHieu
                        };
                    }
                    model.DanhSachPhim.Add(thongtin);
                }
                if (TheLoaiId != 0)
                {
                    var tl = dsTheLoai.FirstOrDefault(x => x.Id.Equals(TheLoaiId));
                    model.TheLoaiHienHanh = new TheLoaiPhimModel.TheLoaiPhimBase
                    {
                        Id = TheLoaiId,
                        Ten = tl.Ten
                    };
                }
                model.DanhSachTheLoai = dsTheLoai.Select(x => new TheLoaiPhimModel.TheLoaiPhimBase
                {
                    Id = x.Id,
                    Ten = x.Ten
                }).ToList();
                model.CurrentPage = CurrentPage;
                model.PageCount = DSPhim.PageCount;
            }

            return View("DanhSachPhim", model);
        }

        public IActionResult ThongTinPhim(int id)
        {
            if (id > 0)
            {
                var channel = GrpcChannel.ForAddress("https://localhost:5002");
                var client = new RapChieuPhim.RapChieuPhimClient(channel);

                Output.Types.Phim phim = client.XemThongTinPhim(new Input.Types.Phim { Id = id });
                var theLoais = client.DanhSachTheLoai(new Input.Types.Empty());
                var xepHangPhims = client.DanhSachXepHangPhim(new Input.Types.Empty());
                var dsTheLoai = theLoais.Items.ToList();

                if (phim != null && phim.Id > 0)
                {
                    var thongtin = new PhimModel.Output.ThongTinPhim
                    {
                        Id = phim.Id,
                        TenPhim = phim.TenPhim,
                        TenGoc = phim.TenGoc,
                        DienVien = phim.DienVien,
                        DaoDien = phim.DaoDien,
                        NoiDung = phim.NoiDung,
                        NgayKhoiChieu = phim.NgayKhoiChieu.ToDateTime(),
                        NgonNgu = phim.NgonNgu,
                        NhaSanXuat = phim.NhaSanXuat,
                        NuocSanXuat = phim.NuocSanXuat,
                        Poster = phim.Poster,
                        ThoiLuong = phim.ThoiLuong,
                        Trailer = phim.Trailer,
                        DanhSachTheLoaiId = phim.DanhSachTheLoaiId,
                        XepHangPhimId = phim.XepHangPhimId
                    };
                    var dsIdTheLoai = phim.DanhSachTheLoaiId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    var theloai_phim = dsTheLoai.Where(x => dsIdTheLoai.Contains(x.Id.ToString())).ToList();
                    if (theloai_phim != null)
                    {
                        foreach (var tl in theloai_phim)
                        {
                            thongtin.DanhSachTheLoai.Add(new TheLoaiPhimModel.TheLoaiPhimBase
                            {
                                Id = tl.Id,
                                Ten = tl.Ten
                            });
                        }
                        var xepHang = xepHangPhims.Items.FirstOrDefault(x => x.Id.Equals(phim.XepHangPhimId));
                        if (xepHang != null)
                        {
                            thongtin.XepHangPhim = new XepHangPhimModel.XepHangPhimBase
                            {
                                Id = xepHang.Id,
                                KyHieu = xepHang.KyHieu,
                                Ten = xepHang.Ten
                            };
                        }
                    }
                    return View(thongtin);
                }
            }
            RedirectToAction("DanhSachPhim");
            return View();
        }

        public IActionResult ThemPhimMoi()
        {
            PhimModel.Output.ThemPhimMoi model = new PhimModel.Output.ThemPhimMoi();
            var channel = GrpcChannel.ForAddress("https://localhost:5002");
            var client = new RapChieuPhim.RapChieuPhimClient(channel);
            var theLoais = client.DanhSachTheLoai(new Input.Types.Empty());
            var xepHangPhims = client.DanhSachXepHangPhim(new Input.Types.Empty());
            var DSTheLoai = theLoais.Items.ToList();
            var DSXepHang = xepHangPhims.Items.ToList();
            model.DanhSachXepHangPhim = DSXepHang.Select(x => new XepHangPhimModel.XepHangPhimBase
            {
                Id = x.Id,
                Ten = x.Ten,
                KyHieu = x.KyHieu
            }).ToList();
            model.DanhSachTheLoai = DSTheLoai.Select(x => new TheLoaiPhimModel.TheLoaiPhimBase { Id = x.Id, Ten = x.Ten }).ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult ThemPhimMoi(IFormCollection form)
        {
            PhimModel.Output.ThemPhimMoi model = new PhimModel.Output.ThemPhimMoi();
            var channel = GrpcChannel.ForAddress("https://localhost:5002");
            var client = new RapChieuPhim.RapChieuPhimClient(channel);

            try
            {
                if (string.IsNullOrEmpty(form["TenPhim"]))
                    model.ThongBao = "<p>- Tên phim phải khác rỗng</p>";
                if (string.IsNullOrEmpty(form["ThoiLuong"]) || int.Parse(form["ThoiLuong"]) <= 0)
                    model.ThongBao += "<p>- Thời lượng phim phải > 0</p>";
                if (string.IsNullOrEmpty(model.ThongBao))
                {
                    var phimThemMoi = new Input.Types.Phim
                    {
                        TenPhim = form["TenPhim"].ToString(),
                        TenGoc = form["TenGoc"].ToString(),
                        DienVien = form["DienVien"].ToString(),
                        DaoDien = form["DaoDien"].ToString(),
                        NgonNgu = form["NgonNgu"].ToString(),
                        NgayKhoiChieu = string.IsNullOrEmpty(form["NgayKhoiChieu"]) ? Timestamp.FromDateTimeOffset(new DateTime(1900, 1, 1)) : Timestamp.FromDateTimeOffset(DateTime.Parse(form["NgayKhoiChieu"])),
                        NhaSanXuat = form["NhaSanXuat"].ToString(),
                        NuocSanXuat = form["NuocSanXuat"].ToString(),
                        NoiDung = form["NoiDung"].ToString(),
                        Poster = form["Poster"].ToString(),
                        ThoiLuong = int.Parse(form["ThoiLuong"].ToString()),
                        Trailer = form["Trailer"].ToString(),
                        XepHangPhimId = int.Parse(form["XepHangPhimId"].ToString()),
                        DanhSachTheLoaiId = form["DanhSachTheLoaiId"].ToString()
                    };
                    var tb = client.ThemPhimMoi(phimThemMoi);
                    model.ThongBao = tb.NoiDung;
                }
            }
            catch (Exception ex)
            {
                model.ThongBao = "Có lỗi xảy ra: " + ex.Message;
            }
            var theLoais = client.DanhSachTheLoai(new Input.Types.Empty());
            var xepHangPhims = client.DanhSachXepHangPhim(new Input.Types.Empty());
            var DSTheLoai = theLoais.Items.ToList();
            var DSXepHang = xepHangPhims.Items.ToList();
            model.DanhSachXepHangPhim = DSXepHang.Select(x => new XepHangPhimModel.XepHangPhimBase
            {
                Id = x.Id,
                Ten = x.Ten,
                KyHieu = x.KyHieu
            }).ToList();
            model.DanhSachTheLoai = DSTheLoai.Select(x => new TheLoaiPhimModel.TheLoaiPhimBase { Id = x.Id, Ten = x.Ten }).ToList();
            return View(model);
        }

        public IActionResult CapNhatPhim(int id)
        {
            if (id > 0)
            {
                var channel = GrpcChannel.ForAddress("https://localhost:5002");
                var client = new RapChieuPhim.RapChieuPhimClient(channel);
                Output.Types.Phim phim = client.XemThongTinPhim(new Input.Types.Phim { Id = id });
                var theLoais = client.DanhSachTheLoai(new Input.Types.Empty());
                var xepHangPhims = client.DanhSachXepHangPhim(new Input.Types.Empty());

                if (phim != null && phim.Id > 0)
                {
                    PhimModel.Output.CapNhatPhim thongTinPhim = new PhimModel.Output.CapNhatPhim
                    {
                        Id = phim.Id,
                        TenPhim = phim.TenPhim,
                        TenGoc = phim.TenGoc,
                        DienVien = phim.DienVien,
                        DaoDien = phim.DaoDien,
                        NoiDung = phim.NoiDung,
                        NgayKhoiChieu = phim.NgayKhoiChieu.ToDateTime(),
                        NgonNgu = phim.NgonNgu,
                        NhaSanXuat = phim.NhaSanXuat,
                        NuocSanXuat = phim.NuocSanXuat,
                        Poster = phim.Poster,
                        ThoiLuong = phim.ThoiLuong,
                        Trailer = phim.Trailer,
                        DanhSachTheLoaiId = phim.DanhSachTheLoaiId,
                        XepHangPhimId = phim.XepHangPhimId
                    };
                    var DSTheLoai = theLoais.Items.ToList();
                    var DSXepHang = xepHangPhims.Items.ToList();
                    thongTinPhim.DanhSachXepHangPhim = DSXepHang.Select(x => new XepHangPhimModel.XepHangPhimBase
                    {
                        Id = x.Id,
                        Ten = x.Ten,
                        KyHieu = x.KyHieu
                    }).ToList();
                    thongTinPhim.DanhSachTheLoai = DSTheLoai.Select(x => new TheLoaiPhimModel.TheLoaiPhimBase { Id = x.Id, Ten = x.Ten }).ToList();
                    return View(thongTinPhim);
                }
            }
            RedirectToAction("DanhSachPhim");
            return View();
        }

        [HttpPost]
        public IActionResult CapNhatPhim(IFormCollection form)
        {
            PhimModel.Output.CapNhatPhim model = new PhimModel.Output.CapNhatPhim();
            var channel = GrpcChannel.ForAddress("https://localhost:5002");
            var client = new RapChieuPhim.RapChieuPhimClient(channel);

            try
            {
                if (string.IsNullOrEmpty(form["TenPhim"]))
                    model.ThongBao = "<p>- Tên phim phải khác rỗng</p>";
                if (string.IsNullOrEmpty(form["ThoiLuong"]) || int.Parse(form["ThoiLuong"]) <= 0)
                    model.ThongBao += "<p>- Thời lượng phim phải > 0</p>";
                if (string.IsNullOrEmpty(form["DanhSachTheLoaiId"]))
                    model.ThongBao += "<p>- Phim phải thuộc ít nhất một thể loại</p>";
                if (string.IsNullOrEmpty(model.ThongBao))
                {
                    var phimCapNhat = new Input.Types.Phim
                    {
                        Id = int.Parse(form["Id"].ToString()),
                        TenPhim = form["TenPhim"].ToString(),
                        TenGoc = form["TenGoc"].ToString(),
                        DienVien = form["DienVien"].ToString(),
                        DaoDien = form["DaoDien"].ToString(),
                        NgonNgu = form["NgonNgu"].ToString(),
                        NgayKhoiChieu = string.IsNullOrEmpty(form["NgayKhoiChieu"]) ? Timestamp.FromDateTimeOffset(new DateTime(1900, 1, 1)) : Timestamp.FromDateTimeOffset(DateTime.Parse(form["NgayKhoiChieu"])),
                        NhaSanXuat = form["NhaSanXuat"].ToString(),
                        NuocSanXuat = form["NuocSanXuat"].ToString(),
                        NoiDung = form["NoiDung"].ToString(),
                        Poster = form["Poster"].ToString(),
                        ThoiLuong = int.Parse(form["ThoiLuong"].ToString()),
                        Trailer = form["Trailer"].ToString(),
                        XepHangPhimId = int.Parse(form["XepHangPhimId"].ToString()),
                        DanhSachTheLoaiId = "," + form["DanhSachTheLoaiId"].ToString() + ","
                    };
                    var tb = client.CapNhatPhim(phimCapNhat);
                    model.ThongBao = tb.NoiDung;
                }
            }
            catch (Exception ex)
            {
                model.ThongBao = "Có lỗi xảy ra: " + ex.Message;
            }

            model.Id = int.Parse(form["Id"].ToString());
            model.TenPhim = form["TenPhim"].ToString();
            model.TenGoc = form["TenGoc"].ToString();
            model.DienVien = form["DienVien"].ToString();
            model.DaoDien = form["DaoDien"].ToString();
            model.NgonNgu = form["NgonNgu"].ToString();
            model.NgayKhoiChieu = string.IsNullOrEmpty(form["NgayKhoiChieu"]) ? new DateTime(1900, 1, 1) : DateTime.Parse(form["NgayKhoiChieu"]);
            model.NhaSanXuat = form["NhaSanXuat"].ToString();
            model.NuocSanXuat = form["NuocSanXuat"].ToString();
            model.NoiDung = form["NoiDung"].ToString();
            model.Poster = form["Poster"].ToString();
            model.ThoiLuong = int.Parse(form["ThoiLuong"].ToString());
            model.Trailer = form["Trailer"].ToString();
            model.XepHangPhimId = int.Parse(form["XepHangPhimId"].ToString());
            model.DanhSachTheLoaiId = !string.IsNullOrEmpty(form["DanhSachTheLoaiId"]) ? "," + form["DanhSachTheLoaiId"].ToString() + "," : "";

            var theLoais = client.DanhSachTheLoai(new Input.Types.Empty());
            var xepHangPhims = client.DanhSachXepHangPhim(new Input.Types.Empty());
            var DSTheLoai = theLoais.Items.ToList();
            var DSXepHang = xepHangPhims.Items.ToList();
            model.DanhSachXepHangPhim = DSXepHang.Select(x => new XepHangPhimModel.XepHangPhimBase
            {
                Id = x.Id,
                Ten = x.Ten,
                KyHieu = x.KyHieu
            }).ToList();
            model.DanhSachTheLoai = DSTheLoai.Select(x => new TheLoaiPhimModel.TheLoaiPhimBase { Id = x.Id, Ten = x.Ten }).ToList();
            return View(model);
        }
    }
}