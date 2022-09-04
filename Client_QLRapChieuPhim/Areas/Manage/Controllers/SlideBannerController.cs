using Client_QLRapChieuPhim.Common;
using Client_QLRapChieuPhim.Models;
using Grpc.Net.Client;
using gRPC_QLRapChieuPhim;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLRapChieuPhim.Areas.Manage.Models.Authorization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Client_QLRapChieuPhim.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize("QuanTri")]
    public class SlideBannerController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SlideBannerController(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
        }

        public IActionResult Index()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5002/");
            var client = new RapChieuPhim.RapChieuPhimClient(channel);

            var response = client.DanhSachSlideBanner(new Input.Types.Empty());
            var banners = response.Items.ToList();
            List<SlideBannerModel.Output.ThongTinSlideBanner> dsBanner
                    = banners.Select(x => new SlideBannerModel.Output.ThongTinSlideBanner
                    {
                        Id = x.Id,
                        Ten = x.Ten,
                        Hinh = x.Hinh,
                        LienKet = x.LienKet,
                        KichHoat = x.KichHoat
                    }).ToList();
            return View(dsBanner);
        }

        public IActionResult ThemSlideBanner(string ten, IFormFile hinh, string lienket, bool kichhoat)
        {
            SlideBannerModel.SlideBannerBase slideBanner = new();
            if (hinh != null)
            {
                var filename = hinh.FileName;
                var filepath = Path.Combine(_hostingEnvironment.WebRootPath, "images\\slides\\banner", filename);
                var imagepath = "/images/slides/banner/" + filename;
                var banner = new Input.Types.SlideBanner
                {
                    Hinh = imagepath,
                    Ten = ten,
                    LienKet = lienket == null ? "" : lienket,
                    KichHoat = kichhoat
                };
                var channel = GrpcChannel.ForAddress("https://localhost:5002/");
                var client = new RapChieuPhim.RapChieuPhimClient(channel);

                var tb = client.ThemSlideBannerMoi(banner);
                if (tb.Maso > 0)
                {
                    ViewData["ThongBao"] = tb.NoiDung;
                    PropertyCopier<Input.Types.SlideBanner, SlideBannerModel.SlideBannerBase>.Copy(banner, slideBanner);
                }
                else
                {
                    hinh.CopyTo(new FileStream(filepath, FileMode.Create));
                    return RedirectToAction("Index");
                }
            }
            return View(slideBanner);
        }

        public IActionResult CapNhatSlideBanner(int id)
        {
            if (id > 0)
            {
                var channel = GrpcChannel.ForAddress("https://localhost:5002/");
                var client = new RapChieuPhim.RapChieuPhimClient(channel);
                Output.Types.SlideBanner banner = client.XemThongTinSlideBanner(new Input.Types.SlideBanner { Id = id });

                if (banner != null && banner.Id > 0)
                {
                    SlideBannerModel.Output.ThongTinSlideBanner thongTinBanner = new()
                    {
                        Id = banner.Id,
                        Ten = banner.Ten,
                        Hinh = banner.Hinh,
                        LienKet = banner.LienKet,
                        KichHoat = banner.KichHoat
                    };
                    return View(thongTinBanner);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CapNhatSlideBanner(IFormFile hinh, int id, string lienket, bool kichhoat)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5002/");
            var client = new RapChieuPhim.RapChieuPhimClient(channel);
            Output.Types.SlideBanner banner = client.XemThongTinSlideBanner(new Input.Types.SlideBanner { Id = id });
            SlideBannerModel.SlideBannerBase slideBanner = new();
            if (banner != null && banner.Id > 0)
            {
                Input.Types.SlideBanner bannerCapNhat = new()
                {
                    Id = banner.Id,
                    Ten = banner.Ten,
                    Hinh = banner.Hinh,
                    LienKet = lienket == null ? "" : lienket,
                    KichHoat = kichhoat
                };
                var filepath = "";
                if (hinh != null)
                {
                    var filename = hinh.FileName;
                    filepath = Path.Combine(_hostingEnvironment.WebRootPath, "images\\slides\\banner", filename);
                    var imagepath = "/images/slides/banner/" + filename;
                    bannerCapNhat.Hinh = imagepath;
                }
                var tb = client.CapNhatSlideBanner(bannerCapNhat);
                if (tb.Maso > 0)
                {
                    ViewData["ThongBao"] = tb.NoiDung;
                    PropertyCopier<Input.Types.SlideBanner, SlideBannerModel.SlideBannerBase>.Copy(bannerCapNhat, slideBanner);
                }
                else
                {
                    if (filepath != "") hinh.CopyTo(new FileStream(filepath, FileMode.Create));
                    return RedirectToAction("Index");
                }
            }
            return View(slideBanner);
        }

        public IActionResult XoaSlideBanner(int id)
        {
            if (id > 0)
            {
                var channel = GrpcChannel.ForAddress("https://localhost:5002/");
                var client = new RapChieuPhim.RapChieuPhimClient(channel);
                var bannerXoa = client.XemThongTinSlideBanner(new Input.Types.SlideBanner { Id = id });
                var tb = client.XoaSlideBanner(new Input.Types.SlideBanner { Id = id });
                if (tb.Maso > 0)
                {
                    ViewData["ThongBao"] = tb.NoiDung;
                }
                else
                {
                    //var filepath = Path.Combine(_hostingEnvironment.WebRootPath, bannerXoa.Hinh);
                    var filepath = _hostingEnvironment.WebRootPath + bannerXoa.Hinh.Replace("/", "\\");
                    if (filepath != "") System.IO.File.Delete(filepath);
                }
            }
            return RedirectToAction("Index");
        }
    }
}