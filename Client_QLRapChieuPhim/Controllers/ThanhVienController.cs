using Client_QLRapChieuPhim.Common;
using Client_QLRapChieuPhim.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using gRPC_QLRapChieuPhim;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Client_QLRapChieuPhim.Controllers
{
    public class ThanhVienController : Controller
    {
        public IActionResult Login()
        {
            TempData["ThongBaoLogin"] = " ";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DangNhap(string username, string password, bool remember = false)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5002/");
            var client = new RapChieuPhim.RapChieuPhimClient(channel);
            //Gọi phương thức DangNhap từ gRPC
            if (username == null) username = "";
            if (password == null) password = "";
            Output.Types.ThanhVien thanh_vien = client.DangNhap(new Input.Types.ThongTinDangNhap
            {
                TenDangNhap = username,
                MatKhau = password
            }
            );
            HttpContext.Session.Remove("ThanhVien");
            if (thanh_vien != null)
            {
                if (thanh_vien.Id > 0)
                {
                    string userToken = LoginUser(thanh_vien, remember).Result;
                    HttpContext.Session.SetString("ThanhVien", userToken);
                }
                else
                    TempData["ThongBaoLogin"] = thanh_vien.ThongBao;
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return Redirect("~/Home/Index");
        }

        public IActionResult Register()
        {
            ThanhVienModel.Output.ThongTinThanhVien thanhvien = new();
            return View(thanhvien);
        }

        [HttpPost]
        public IActionResult Register(ThanhVienModel.Output.ThongTinThanhVien thanhvien)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5002/");
            var client = new RapChieuPhim.RapChieuPhimClient(channel);
            if (ModelState.IsValid)
            {
                var thanhviendangky = new Input.Types.ThongTinDangKy();
                PropertyCopier<ThanhVienModel.Output.ThongTinThanhVien,
                                         Input.Types.ThongTinDangKy>.Copy(thanhvien, thanhviendangky);
                if (thanhvien.NgaySinh.Year < 1900)
                    thanhviendangky.NgaySinh = Timestamp.FromDateTimeOffset(new DateTime(1900, 1, 1));
                else
                    thanhviendangky.NgaySinh = Timestamp.FromDateTimeOffset(thanhvien.NgaySinh);
                var tb = client.DangKyThanhVien(thanhviendangky);
                if (tb.Maso == 0)
                {
                    thanhvien.Id = int.Parse(tb.NoiDung);
                    TempData["EmailDangKy"] = thanhviendangky.Email;
                    var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(thanhvien.Email));
                    //Tạo link xác nhận kích hoạt tài khoản thành viên
                    var callbackUrl = Url.ActionLink("ConfirmEmail", "ThanhVien",
                                                    new { area = "", code = code }, Request.Scheme);
                    //Tạo nội dung Email
                    var message = @"<div style='margin:4px 0;font-family:Arial,Helvetica,sans-serif;font-size:14px;color:#444;line-height:18px;font-weight:normal'>Chào <b>" + thanhvien.HoTen + "</b>,<br/><br/></div>" +
                                "<div>Cám ơn bạn đã đăng ký thành viên tại CSC Cinema.<br/><br/> </div>" +
                                "<div>Để hoàn tất đăng ký, bạn vui lòng <b><a style='text-decoration: none;' href='" + callbackUrl + "'><span style='background-color: #ff6600 !important; color: #ffffff; padding: 5px 10px; border-radius: 3px;'>Nhấn vào đây</span></a></b><br/><br/></div>";
                    Utilities.SendMail("Xác nhận đăng ký thành viên", message, thanhvien.Email);
                    return RedirectToAction("RegisterSuccess", "ThanhVien");
                }
                else
                    ViewData["ThongBaoDangKy"] = tb.NoiDung;
            }
            return View(thanhvien);
        }

        public IActionResult RegisterSuccess()
        {
            return View();
        }

        public IActionResult ConfirmEmail(string code)
        {
            var email = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            //Kết nối đến dịch vụ gRPC để truy xuất dữ liệu
            var channel = GrpcChannel.ForAddress("https://localhost:5002/");
            var client = new RapChieuPhim.RapChieuPhimClient(channel);
            //Gọi phương thức KichHoat từ gRPC
            Output.Types.ThongBao thong_bao =
                client.KichHoatTaiKhoan(new Input.Types.ThongTinDangKy { Email = email });
            if (thong_bao.Maso == 0) // kích hoạt thành công
            {
                ViewData["ThongBaoKichHoat"] = "thanhcong";
            }
            else // kích hoạt bị lỗi
            {
                ViewData["ThongBaoKichHoat"] = "thatbai";
            }
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
                var email = User.Claims.FirstOrDefault(x => x.Type == "EMAIL").Value;
                var id = User.Claims.FirstOrDefault(x => x.Type == "THANHVIENID").Value;
                var channel = GrpcChannel.ForAddress("https://localhost:5002/");
                var client = new RapChieuPhim.RapChieuPhimClient(channel);
                Output.Types.ThongBao thong_bao = client.ThayDoiMatKhau(
                    new Input.Types.ThongTinThayDoiMatKhau
                    {
                        Id = int.Parse(id),
                        Email = email,
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

        private async Task<string> LoginUser(Output.Types.ThanhVien thanhvien, bool remember)
        {
            try
            {
                var userClaims = new List<Claim>() {
                    new Claim("THANHVIENID", thanhvien.Id.ToString()),
                    new Claim("USERNAME", thanhvien.Email),
                    new Claim("QUUYENHAN", "ThanhVien"),
                    new Claim("HOTEN", thanhvien.HoTen),
                    new Claim("EMAIL", thanhvien.Email)
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