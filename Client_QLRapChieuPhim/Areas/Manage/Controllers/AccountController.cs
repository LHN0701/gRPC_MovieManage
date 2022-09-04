using Client_QLRapChieuPhim.Models;
using Grpc.Net.Client;
using gRPC_QLRapChieuPhim;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Client_QLRapChieuPhim.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            LoginModel model = new();
            return View(model);
        }

        public IActionResult DangNhap(string username, string password, bool remember = false, string returnUrl = null)
        {
            //Kết nối đến dịch vụ gRPC để truy xuất dữ liệu
            var channel = GrpcChannel.ForAddress("https://localhost:5002");
            var client = new RapChieuPhim.RapChieuPhimClient(channel);
            //Gọi phương thức DangNhap từ gRPC
            if (username == null) username = "";
            if (password == null) password = "";
            Output.Types.NhanVien nhanvien = client.NhanVienDangNhap(new Input.Types.ThongTinDangNhap
            {
                TenDangNhap = username,
                MatKhau = password
            }
            );
            HttpContext.Session.Remove("NhanVien");
            if (nhanvien != null)
            {
                if (nhanvien.Id > 0)
                {
                    string userToken = LoginUser(nhanvien, remember).Result;
                    HttpContext.Session.SetString("NhanVien", userToken);
                    if (!string.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                    TempData["ThongBaoLogin"] = nhanvien.ThongBao;
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ThongBaoLogin"] = null;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ChangePasswordModel model = new();
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult ChangePassword(string matkhaucu, string matkhaumoi)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ThongBaoLogin"] = null;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ChangePasswordModel model = new();
                var cmnd = User.Claims.FirstOrDefault(x => x.Type == "CMND").Value;
                var id = User.Claims.FirstOrDefault(x => x.Type == "NHANVIENID").Value;
                var channel = GrpcChannel.ForAddress("https://localhost:5002");
                var client = new RapChieuPhim.RapChieuPhimClient(channel);
                Output.Types.ThongBao thong_bao = client.NhanVienThayDoiMatKhau(
                    new Input.Types.ThongTinThayDoiMatKhau
                    {
                        Id = int.Parse(id),
                        Email = cmnd,
                        MatKhauCu = matkhaucu,
                        MatKhauMoi = matkhaumoi
                    });
                if (thong_bao.Maso == 0)
                    ViewData["ThongBao"] = $"<span style='color: #0000ff;'>{thong_bao.NoiDung}</span>";
                else
                    ViewData["ThongBao"] = $"<span style='color: #ff0000;'>{thong_bao.NoiDung}</span>";
                return View(model);
            }
        }

        private string GetToken(List<Claim> userClaims)
        {
            //Tạo key để mã hóa Token theo chuẩn Sha256
            //ví dụ: 123456-2222-123abc:456789-abcd-1234-4567-abc123abc
            var key = Encoding.ASCII.GetBytes("123456-2222-123abc:456789-abcd-1234-4567-abc123abc");
            //Tạo User Token
            var JWToken = new JwtSecurityToken(
                issuer: "http://localhost:45092/",
                audience: "http://localhost:45092/",
                claims: userClaims,
                notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                                                            SecurityAlgorithms.HmacSha256Signature)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(JWToken);
            return token;
        }

        private async Task<string> LoginUser(Output.Types.NhanVien nhanvien, bool remember)
        {
            try
            {
                var userClaims = new List<Claim>() {
                    new Claim("NHANVIENID", nhanvien.Id.ToString()),
                    new Claim("USERNAME", nhanvien.Cmnd),
                    new Claim("QUYENHAN", nhanvien.QuyenHan.ToUpper()),
                    new Claim("HOTEN", nhanvien.HoTen),
                    new Claim("CMND", nhanvien.Cmnd)
                };
                var claimsIdentity = new ClaimsIdentity(userClaims,
                                            CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(claimsIdentity);
                var properties = new AuthenticationProperties { IsPersistent = remember };
                await HttpContext.SignInAsync(principal, properties);

                var token = GetToken(userClaims);
                return token;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}