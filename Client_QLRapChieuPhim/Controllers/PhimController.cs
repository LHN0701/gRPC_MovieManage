using Client_QLRapChieuPhim.Models;
using Grpc.Net.Client;
using gRPC_QLRapChieuPhim;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Client_QLRapChieuPhim.Models.PhimModel.Output;

namespace Client_QLRapChieuPhim.Controllers
{
    [Route("phim")]
    public class PhimController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("phim-dang-chieu")]
        public IActionResult PhimDangChieu()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5002");
            var client = new RapChieuPhim.RapChieuPhimClient(channel);

            Output.Types.Phims phimDangChieus = client.DanhSachPhimDangChieu(new Input.Types.Empty());
            var DSPhimDangChieu = phimDangChieus.Items.ToList();
            var theLoais = client.DanhSachTheLoai(new Input.Types.Empty());
            var xepHangPhims = client.DanhSachXepHangPhim(new Input.Types.Empty());
            var dsTheLoai = theLoais.Items.ToList();
            List<ThongTinPhim> model = new List<ThongTinPhim>();

            foreach (var phim in DSPhimDangChieu)
            {
                var thongTin = new ThongTinPhim
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
                        thongTin.DanhSachTheLoai.Add(new TheLoaiPhimModel.TheLoaiPhimBase
                        {
                            Id = tl.Id,
                            Ten = tl.Ten
                        });
                    }
                    var xepHang = xepHangPhims.Items.FirstOrDefault(x => x.Id.Equals(phim.XepHangPhimId));
                    if (xepHang != null)
                    {
                        thongTin.XepHangPhim = new XepHangPhimModel.XepHangPhimBase
                        {
                            Id = xepHang.Id,
                            Ten = xepHang.Ten,
                            KyHieu = xepHang.KyHieu
                        };
                    }
                }
                model.Add(thongTin);
            }
            ViewData["loaiPhim"] = "PhimDangChieu";

            return View("DanhSachPhim", model);
        }

        [Route("phim-sap-chieu")]
        public IActionResult PhimSapChieu()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5002");
            var client = new RapChieuPhim.RapChieuPhimClient(channel);

            Output.Types.Phims phimSapChieu = client.DanhSachPhimSapChieu(new Input.Types.Empty());
            var DSPhimSapChieu = phimSapChieu.Items.ToList();
            var theLoais = client.DanhSachTheLoai(new Input.Types.Empty());
            var xepHangPhims = client.DanhSachXepHangPhim(new Input.Types.Empty());
            var dsTheLoai = theLoais.Items.ToList();
            var model = new List<ThongTinPhim>();

            foreach (var phim in DSPhimSapChieu)
            {
                var thongtin = new ThongTinPhim
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
                model.Add(thongtin);
            }

            ViewData["loaiPhim"] = "PhimSapChieu";

            return View("DanhSachPhim", model);
        }

        [Route("thong-tin-phim")]
        public IActionResult ThongTinPhim(int id)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5002");
            var client = new RapChieuPhim.RapChieuPhimClient(channel);

            Output.Types.Phim phim = client.XemThongTinPhim(new Input.Types.Phim { Id = id });
            var theLoais = client.DanhSachTheLoai(new Input.Types.Empty());
            var xepHangPhims = client.DanhSachXepHangPhim(new Input.Types.Empty());
            var dsTheLoai = theLoais.Items.ToList();

            if (phim != null && phim.Id > 0)
            {
                var thongtin = new ThongTinPhim
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

                    if (Count.count == null)
                    {
                        Count.count = 1;
                        Response.Cookies.Append($"tenphim{Count.count}", thongtin.TenPhim);
                    }
                    else
                    {
                        Count.count++;
                        Response.Cookies.Append($"tenphim{Count.count}", thongtin.TenPhim);
                    }

                    return View(thongtin);
                }
            }
            return View();
        }

        [Route("phim-da-xem")]
        public IActionResult PhimDaXem()
        {
            for (int i = 1; i <= Count.count; i++)
            {
                if (Request.Cookies.ContainsKey($"tenphim{i}"))
                {
                    CookieOptions cookieOptions = new CookieOptions();
                    cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddSeconds(20));
                    //Response.Cookies.Append($"tenphim{i}", DateTime.Now.ToString(), cookieOptions);
                }
                ViewData[$"tenphim{i}"] = Request.Cookies[$"tenphim{i}"];
            }
            return View();
        }
    }
}