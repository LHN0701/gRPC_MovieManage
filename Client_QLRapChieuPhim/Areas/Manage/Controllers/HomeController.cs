using Microsoft.AspNetCore.Mvc;
using QLRapChieuPhim.Areas.Manage.Models.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client_QLRapChieuPhim.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize("QuanTri", "NhanVien")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("DanhSachPhim", "Phim");
        }
    }
}