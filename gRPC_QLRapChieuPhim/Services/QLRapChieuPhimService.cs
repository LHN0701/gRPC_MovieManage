using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gRPC_QLRapChieuPhim.Common;
using gRPC_QLRapChieuPhim.Models;
using Microsoft.Extensions.Logging;

namespace gRPC_QLRapChieuPhim.Services
{
    public class QLRapChieuPhimService : RapChieuPhim.RapChieuPhimBase
    {
        private readonly QLRapChieuPhimContext _context;
        private readonly ILogger<QLRapChieuPhimService> _logger;

        public QLRapChieuPhimService(QLRapChieuPhimContext context, ILogger<QLRapChieuPhimService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override Task<Output.Types.TheLoaiPhims> DanhSachTheLoai(Input.Types.Empty request, ServerCallContext context)
        {
            var dsTheLoai = _context.TheLoaiPhims.Select(x => new Output.Types.TheLoaiPhim
            {
                Id = x.Id,
                Ten = x.Ten
            });
            Output.Types.TheLoaiPhims responseData = new Output.Types.TheLoaiPhims();
            responseData.Items.AddRange(dsTheLoai);
            try
            {
                return Task.FromResult(responseData);
            }
            catch (Exception ex)
            {
                throw new RpcException(Status.DefaultCancelled, ex.Message);
            }
        }

        public override Task<Output.Types.Phims> DanhSachPhimDangChieu(Input.Types.Empty request, ServerCallContext context)
        {
            int SoNgayChieu = Utilities.NumberOfWeekShow * 7;
            var dsPhimDangChieu = _context.Phims.Where(x => x.NgayKhoiChieu != null && (x.NgayKhoiChieu <= DateTime.Today && x.NgayKhoiChieu.Value.AddDays(SoNgayChieu) >= new DateTime(2021, 1, 8))).ToList();
            Output.Types.Phims responseData = new Output.Types.Phims();
            if (dsPhimDangChieu.Count > 0)
            {
                var dsPhim = dsPhimDangChieu
                .Select(x => new Output.Types.Phim
                {
                    Id = x.Id,
                    TenPhim = x.TenPhim,
                    TenGoc = string.IsNullOrEmpty(x.TenGoc) ? "" : x.TenGoc,
                    NgayKhoiChieu = x.NgayKhoiChieu == null ? Timestamp.FromDateTimeOffset(new DateTime(1900, 1, 1)) : Timestamp.FromDateTimeOffset(x.NgayKhoiChieu.Value),
                    DanhSachTheLoaiId = x.DanhSachTheLoaiId,
                    DaoDien = x.DaoDien ?? "",
                    DienVien = x.DienVien ?? "",
                    XepHangPhimId = x.XepHangPhimId ?? 0,
                    NgonNgu = x.NgonNgu ?? "",
                    NhaSanXuat = x.NhaSanXuat ?? "",
                    NoiDung = x.NoiDung ?? "",
                    NuocSanXuat = x.NuocSanXuat ?? "",
                    Poster = x.Poster ?? "",
                    ThoiLuong = x.ThoiLuong,
                    Trailer = x.Trailer ?? ""
                });

                try
                {
                    responseData.Items.AddRange(dsPhim);
                }
                catch (Exception ex)
                {
                    responseData.ThongBao = "Lỗi: " + ex.Message;
                }
            }
            else
            {
                responseData.ThongBao = "Lỗi: Không có dữ liệu";
            }
            return Task.FromResult(responseData);
        }

        public override Task<Output.Types.Phims> DanhSachPhimTheoTheLoai(Input.Types.PhimTheoTheLoai request,
            ServerCallContext context)
        {
            //Xử lý trả về danh sách phim theo thể loại được chọn
            var theLoais = _context.TheLoaiPhims.ToList();
            Output.Types.TheLoaiPhim theLoaiHienHanh;
            List<Models.Phim> dsPhimTheoTheLoai = null;
            if (request.IdTheLoai > 0)
            {
                var theLoai = "," + request.IdTheLoai.ToString() + ",";
                var tl = theLoais.FirstOrDefault(t => t.Id.Equals(request.IdTheLoai));
                theLoaiHienHanh = new Output.Types.TheLoaiPhim { Id = tl.Id, Ten = tl.Ten };
                dsPhimTheoTheLoai = _context.Phims
                    .Where(x => x.DanhSachTheLoaiId.Contains(theLoai)).ToList();
            }
            else
            {
                theLoaiHienHanh = new Output.Types.TheLoaiPhim();
                dsPhimTheoTheLoai = _context.Phims.ToList();
            }

            if (request.PageSize <= 0)
                request.PageSize = 10;
            float numberpage = (float)dsPhimTheoTheLoai.Count() / request.PageSize;
            int pageCount = (int)Math.Ceiling(numberpage);

            int currentPage = request.CurrentPage;
            if (currentPage > pageCount) currentPage = pageCount;

            dsPhimTheoTheLoai = dsPhimTheoTheLoai
                .Skip((currentPage - 1) * request.PageSize)
                .Take(request.PageSize).ToList();
            if (dsPhimTheoTheLoai.Count > 0)
            {
                var dsPhim = dsPhimTheoTheLoai
                .Select(x => new Output.Types.Phim
                {
                    Id = x.Id,
                    TenPhim = x.TenPhim,
                    TenGoc = string.IsNullOrEmpty(x.TenGoc) ? "" : x.TenGoc,
                    NgayKhoiChieu = x.NgayKhoiChieu == null ?
                        Timestamp.FromDateTimeOffset(new DateTime(1900, 1, 1)) :
                        Timestamp.FromDateTimeOffset(x.NgayKhoiChieu.Value),
                    DanhSachTheLoaiId = x.DanhSachTheLoaiId,
                    DaoDien = x.DaoDien ?? "",
                    DienVien = x.DienVien ?? "",
                    XepHangPhimId = x.XepHangPhimId ?? 0,
                    NgonNgu = x.NgonNgu ?? "",
                    NhaSanXuat = x.NhaSanXuat ?? "",
                    NoiDung = x.NoiDung ?? "",
                    NuocSanXuat = x.NuocSanXuat ?? "",
                    Poster = x.Poster ?? "",
                    ThoiLuong = x.ThoiLuong,
                    Trailer = x.Trailer ?? ""
                });

                Output.Types.Phims responseData = new Output.Types.Phims();
                responseData.Items.AddRange(dsPhim);
                responseData.PageCount = pageCount;
                responseData.TheLoaiHienHanh = theLoaiHienHanh;
                try
                {
                    return Task.FromResult(responseData);
                }
                catch (Exception ex)
                {
                    throw new RpcException(Status.DefaultCancelled, ex.Message);
                }
            }
            else
            {
                throw new RpcException(Status.DefaultCancelled, "Không có dữ liệu.");
            }
        }

        public override Task<Output.Types.XepHangPhims> DanhSachXepHangPhim(Input.Types.Empty request,
                    ServerCallContext context)
        {
            //Xử lý trả về danh sách xếp hạng phim
            var dsXepHang = _context.XepHangPhims.Select(x => new Output.Types.XepHangPhim
            {
                Id = x.Id,
                Ten = x.Ten,
                KyHieu = x.KyHieu
            });
            Output.Types.XepHangPhims responseData = new Output.Types.XepHangPhims();
            responseData.Items.AddRange(dsXepHang);
            try
            {
                return Task.FromResult(responseData);
            }
            catch (Exception ex)
            {
                throw new RpcException(Status.DefaultCancelled, ex.Message);
            }
        }

        public override Task<Output.Types.Phims> DanhSachPhimSapChieu(Input.Types.Empty request, ServerCallContext context)
        {
            int SoNgayChieu = Utilities.NumberOfWeekShow * 21;
            var dsPhimSapChieu = _context.Phims.Where(x => x.NgayKhoiChieu != null && x.NgayKhoiChieu > DateTime.Today).ToList();
            Output.Types.Phims responseData = new Output.Types.Phims();
            if (dsPhimSapChieu.Count > 0)
            {
                var dsPhim = dsPhimSapChieu
                .Select(x => new Output.Types.Phim
                {
                    //Gán giá trị cho các thuộc tính của phim
                    Id = x.Id,
                    TenPhim = x.TenPhim,
                    TenGoc = string.IsNullOrEmpty(x.TenGoc) ? "" : x.TenGoc,
                    NgayKhoiChieu = x.NgayKhoiChieu == null ? Timestamp.FromDateTimeOffset(new DateTime(1900, 1, 1)) : Timestamp.FromDateTimeOffset(x.NgayKhoiChieu.Value),
                    DanhSachTheLoaiId = x.DanhSachTheLoaiId,
                    DaoDien = x.DaoDien ?? "",
                    DienVien = x.DienVien ?? "",
                    XepHangPhimId = x.XepHangPhimId ?? 0,
                    NgonNgu = x.NgonNgu ?? "",
                    NhaSanXuat = x.NhaSanXuat ?? "",
                    NoiDung = x.NoiDung ?? "",
                    NuocSanXuat = x.NuocSanXuat ?? "",
                    Poster = x.Poster ?? "",
                    ThoiLuong = x.ThoiLuong,
                    Trailer = x.Trailer ?? ""
                });

                try
                {
                    responseData.Items.AddRange(dsPhim);
                }
                catch (Exception ex)
                {
                    responseData.ThongBao = "Lỗi: " + ex.Message;
                }
            }
            else
            {
                responseData.ThongBao = "Lỗi: Không có dữ liệu";
            }
            return Task.FromResult(responseData);
        }

        public override Task<Output.Types.Phim> XemThongTinPhim(Input.Types.Phim request,
                ServerCallContext context)
        {
            if (request.Id > 0)
            {
                try
                {
                    var phim = _context.Phims.FirstOrDefault(x => x.Id.Equals(request.Id));
                    Output.Types.Phim thongtinPhim = null;
                    if (phim != null)
                    {
                        thongtinPhim = new Output.Types.Phim
                        {
                            Id = phim.Id,
                            TenPhim = phim.TenPhim,
                            TenGoc = string.IsNullOrEmpty(phim.TenGoc) ? "" : phim.TenGoc,
                            NgayKhoiChieu = phim.NgayKhoiChieu == null ? Timestamp.FromDateTimeOffset(new DateTime(1900, 1, 1)) : Timestamp.FromDateTimeOffset(phim.NgayKhoiChieu.Value),
                            DanhSachTheLoaiId = phim.DanhSachTheLoaiId,
                            DaoDien = phim.DaoDien ?? "",
                            DienVien = phim.DienVien ?? "",
                            XepHangPhimId = phim.XepHangPhimId ?? 0,
                            NgonNgu = phim.NgonNgu ?? "",
                            NhaSanXuat = phim.NhaSanXuat ?? "",
                            NoiDung = phim.NoiDung ?? "",
                            NuocSanXuat = phim.NuocSanXuat ?? "",
                            Poster = phim.Poster ?? "",
                            ThoiLuong = phim.ThoiLuong,
                            Trailer = phim.Trailer ?? ""
                        };
                    }
                    else
                    {
                        thongtinPhim = new Output.Types.Phim();
                    }
                    return Task.FromResult(thongtinPhim);
                }
                catch (Exception ex)
                {
                    throw new RpcException(Status.DefaultCancelled, ex.Message);
                }
            }
            else
            {
                throw new RpcException(Status.DefaultCancelled, "Không có dữ liệu.");
            }
        }

        public override Task<Output.Types.ThongBao> ThemPhimMoi(Input.Types.Phim request, ServerCallContext context)
        {
            Output.Types.ThongBao tb = new Output.Types.ThongBao { Maso = 1 };
            try
            {
                var phimMoi = new Models.Phim
                {
                    //Gán thông tin từ request vào phimMoi
                    Id = request.Id,
                    TenPhim = request.TenPhim,
                    TenGoc = string.IsNullOrEmpty(request.TenGoc) ? "" : request.TenGoc,
                    NgayKhoiChieu = request.NgayKhoiChieu == null ? null : request.NgayKhoiChieu.ToDateTime(),
                    DanhSachTheLoaiId = request.DanhSachTheLoaiId,
                    DaoDien = request.DaoDien,
                    DienVien = request.DienVien,
                    XepHangPhimId = request.XepHangPhimId,
                    NgonNgu = request.NgonNgu,
                    NhaSanXuat = request.NhaSanXuat,
                    NoiDung = request.NoiDung,
                    NuocSanXuat = request.NuocSanXuat,
                    Poster = request.Poster,
                    ThoiLuong = request.ThoiLuong,
                    Trailer = request.Trailer
                };
                var chuoiTB = "";
                if (string.IsNullOrEmpty(phimMoi.TenPhim))
                    chuoiTB = "Tên phim phải khác rỗng";
                if (phimMoi.ThoiLuong <= 0)
                    chuoiTB += "Thời lượng phim phải > 0";

                if (string.IsNullOrEmpty(chuoiTB))
                {
                    _context.Phims.Add(phimMoi);
                    int kq = _context.SaveChanges();
                    if (kq > 0)
                    {
                        tb.Maso = 0;
                        chuoiTB = "Lưu thông tin phim mới thành công";
                    }
                    else
                        chuoiTB = "Lưu thông tin phim mới không thành công";
                }

                tb.NoiDung = chuoiTB;

                return Task.FromResult(tb);
            }
            catch (Exception ex)
            {
                throw new RpcException(Status.DefaultCancelled, ex.Message);
            }
        }

        public override Task<Output.Types.ThongBao> CapNhatPhim(Input.Types.Phim request, ServerCallContext context)
        {
            Output.Types.ThongBao tb = new Output.Types.ThongBao { Maso = 1 };
            try
            {
                var phimCapNhat = _context.Phims.FirstOrDefault(p => p.Id.Equals(request.Id));
                if (phimCapNhat != null)
                {
                    //Gán thông tin từ request vào phimCapNhat
                    phimCapNhat.TenPhim = request.TenPhim;
                    phimCapNhat.TenGoc = string.IsNullOrEmpty(request.TenGoc) ? "" : request.TenGoc;
                    phimCapNhat.NgayKhoiChieu = request.NgayKhoiChieu == null ? null : request.NgayKhoiChieu.ToDateTime();
                    phimCapNhat.DanhSachTheLoaiId = request.DanhSachTheLoaiId;
                    phimCapNhat.DaoDien = request.DaoDien;
                    phimCapNhat.DienVien = request.DienVien;
                    phimCapNhat.XepHangPhimId = request.XepHangPhimId;
                    phimCapNhat.NgonNgu = request.NgonNgu;
                    phimCapNhat.NhaSanXuat = request.NhaSanXuat;
                    phimCapNhat.NoiDung = request.NoiDung;
                    phimCapNhat.NuocSanXuat = request.NuocSanXuat;
                    phimCapNhat.Poster = request.Poster;
                    phimCapNhat.ThoiLuong = request.ThoiLuong;
                    phimCapNhat.Trailer = request.Trailer;

                    var chuoiTB = "";
                    if (string.IsNullOrEmpty(phimCapNhat.TenPhim))
                        chuoiTB = "Tên phim phải khác rỗng";
                    if (phimCapNhat.ThoiLuong <= 0)
                        chuoiTB += "Thời lượng phim phải > 0";

                    if (string.IsNullOrEmpty(chuoiTB))
                    {
                        int kq = _context.SaveChanges();
                        if (kq > 0)
                        {
                            tb.Maso = 0;
                            chuoiTB = "Lưu thông tin phim mới thành công";
                        }
                        else
                            chuoiTB = "Lưu thông tin phim mới không thành công";
                    }
                    tb.NoiDung = chuoiTB;
                }
                return Task.FromResult(tb);
            }
            catch (Exception ex)
            {
                throw new RpcException(Status.DefaultCancelled, ex.Message);
            }
        }

        public override Task<Output.Types.Phims> TimPhim(Input.Types.TimPhim request, ServerCallContext context)
        {
            var theLoais = _context.TheLoaiPhims.ToList();
            List<Models.Phim> dsPhimTimKiem = new List<Models.Phim>();
            if (!string.IsNullOrEmpty(request.KeyWord))
            {
                var tukhoas = request.KeyWord.Split(new string[] { "," },
                                    StringSplitOptions.RemoveEmptyEntries);
                if (tukhoas.Count() > 0)
                {
                    foreach (var tk in tukhoas)
                    {
                        var phimTimKiem = _context.Phims.Where(x => x.TenPhim.Contains(tk)
                            || x.TenGoc.Contains(tk) || x.DaoDien.Contains(tk)
                            || x.DienVien.Contains(tk)).ToList();
                        if (phimTimKiem != null && phimTimKiem.Count() > 0)
                            dsPhimTimKiem.AddRange(phimTimKiem);
                    }
                }
            }

            float numberpage = (float)dsPhimTimKiem.Count() / request.PageSize;
            int pageCount = (int)Math.Ceiling(numberpage);
            int currentPage = request.CurrentPage;
            if (currentPage > pageCount) currentPage = pageCount;

            //var xepHangs = _context.XepHangPhims.ToList();
            dsPhimTimKiem = dsPhimTimKiem.Skip((currentPage - 1) * request.PageSize)
                                         .Take(request.PageSize).ToList();
            if (dsPhimTimKiem.Count > 0)
            {
                var dsPhim = dsPhimTimKiem
                .Select(x => new Output.Types.Phim
                {
                    //Gán giá trị cho các thuộc tính của phim
                    Id = x.Id,
                    TenPhim = x.TenPhim,
                    TenGoc = string.IsNullOrEmpty(x.TenGoc) ? "" : x.TenGoc,
                    NgayKhoiChieu = x.NgayKhoiChieu == null ? Timestamp.FromDateTimeOffset(new DateTime(1900, 1, 1)) : Timestamp.FromDateTimeOffset(x.NgayKhoiChieu.Value),
                    DanhSachTheLoaiId = x.DanhSachTheLoaiId,
                    DaoDien = x.DaoDien ?? "",
                    DienVien = x.DienVien ?? "",
                    XepHangPhimId = x.XepHangPhimId ?? 0,
                    NgonNgu = x.NgonNgu ?? "",
                    NhaSanXuat = x.NhaSanXuat ?? "",
                    NoiDung = x.NoiDung ?? "",
                    NuocSanXuat = x.NuocSanXuat ?? "",
                    Poster = x.Poster ?? "",
                    ThoiLuong = x.ThoiLuong,
                    Trailer = x.Trailer ?? ""
                });

                Output.Types.Phims responseData = new Output.Types.Phims();
                responseData.Items.AddRange(dsPhim);
                responseData.PageCount = pageCount;
                try
                {
                    return Task.FromResult(responseData);
                }
                catch (Exception ex)
                {
                    throw new RpcException(Status.DefaultCancelled, ex.Message);
                }
            }
            else
            {
                throw new RpcException(Status.DefaultCancelled, "Không có dữ liệu.");
            }
        }

        public override Task<Output.Types.ThanhVien> DangNhap(Input.Types.ThongTinDangNhap request,
                                                                         ServerCallContext context)
        {
            Output.Types.ThanhVien thongtinThanhVien = new Output.Types.ThanhVien();
            if (!string.IsNullOrEmpty(request.TenDangNhap) && !string.IsNullOrEmpty(request.MatKhau))
            {
                try
                {
                    var thanh_vien = _context.ThanhViens.FirstOrDefault(x => x.Email.Equals(request.TenDangNhap)
                                        && x.MatKhau.Equals(request.MatKhau));
                    if (thanh_vien != null)
                    {
                        thongtinThanhVien = new Output.Types.ThanhVien
                        {
                            Id = thanh_vien.Id,
                            HoTen = thanh_vien.HoTen,
                            GioiTinh = thanh_vien.GioiTinh,
                            NgaySinh = Timestamp.FromDateTimeOffset(thanh_vien.NgaySinh),
                            Email = thanh_vien.Email,
                            DienThoai = thanh_vien.DienThoai,
                            MatKhau = thanh_vien.MatKhau,
                            KichHoat = thanh_vien.KichHoat
                        };
                    }
                    else
                    {
                        thongtinThanhVien.ThongBao = "Tên đăng nhập hoặc Mật khẩu không hợp lệ";
                    }
                }
                catch (Exception ex)
                {
                    thongtinThanhVien.ThongBao = "Lỗi đăng nhập: " + ex.Message;
                }
            }
            else
            {
                thongtinThanhVien.ThongBao = "Phải cung cấp Tên đăng nhập và Mật khẩu.";
            }
            return Task.FromResult(thongtinThanhVien);
        }

        public override Task<Output.Types.ThongBao> DangKyThanhVien(Input.Types.ThongTinDangKy request,
                                                                                ServerCallContext context)
        {
            Output.Types.ThongBao tb = new Output.Types.ThongBao { Maso = 1 };
            try
            {
                var thanh_vien_moi = new Models.ThanhVien
                {
                    Email = request.Email,
                    MatKhau = request.MatKhau,
                    HoTen = request.HoTen,
                    GioiTinh = request.GioiTinh,
                    NgaySinh = request.NgaySinh.ToDateTime(),
                    DienThoai = request.DienThoai,
                    KichHoat = false
                };
                var chuoiTB = "";
                if (string.IsNullOrEmpty(thanh_vien_moi.Email))
                    chuoiTB = "Email phải khác rỗng";
                else
                {
                    var thanhvien = _context.ThanhViens.FirstOrDefault(x => x.Email.Equals(thanh_vien_moi.Email));
                    if (thanhvien != null)
                        chuoiTB = "Email này đã được sử dụng rồi.";
                }
                if (string.IsNullOrEmpty(thanh_vien_moi.HoTen))
                    chuoiTB = "Họ tên phải khác rỗng";
                if (string.IsNullOrEmpty(thanh_vien_moi.DienThoai))
                    chuoiTB = "Điện thoại phải khác rỗng";
                if (string.IsNullOrEmpty(chuoiTB))
                {
                    var thanh_vien = _context.ThanhViens.Add(thanh_vien_moi);
                    int kq = _context.SaveChanges();
                    if (kq > 0)
                    {
                        tb.Maso = 0;
                        chuoiTB = thanh_vien.Entity.Id.ToString();
                    }
                    else
                        chuoiTB = "Lưu thông tin thành viên mới không thành công";
                }
                tb.NoiDung = chuoiTB;
            }
            catch (Exception ex)
            {
                tb.NoiDung = "Lỗi: " + ex.Message;
            }
            return Task.FromResult(tb);
        }

        public override Task<Output.Types.ThongBao> KichHoatTaiKhoan(Input.Types.ThongTinDangKy request,
                                                                                ServerCallContext context)
        {
            Output.Types.ThongBao tb = new Output.Types.ThongBao { Maso = 1 };
            try
            {
                var thanhVienCapNhat = _context.ThanhViens.FirstOrDefault(p => p.Email.Equals(request.Email));
                if (thanhVienCapNhat != null)
                {
                    thanhVienCapNhat.KichHoat = true;
                    int kq = _context.SaveChanges();
                    if (kq > 0)
                    {
                        tb.Maso = 0;
                        tb.NoiDung = "Kích hoạt tài khoản thành công";
                    }
                    else
                        tb.NoiDung = "Kích hoạt tài khoản không thành công";
                }
            }
            catch (Exception ex)
            {
                tb.NoiDung = "Lỗi: " + ex.Message;
            }
            return Task.FromResult(tb);
        }

        public override Task<Output.Types.ThongBao> ThayDoiMatKhau(Input.Types.ThongTinThayDoiMatKhau request,
                                                                                        ServerCallContext context)
        {
            Output.Types.ThongBao tb = new Output.Types.ThongBao { Maso = 1 };
            try
            {
                var thanhVienCapNhat = _context.ThanhViens.FirstOrDefault(p => p.Id.Equals(request.Id) &&
                                                                                    p.Email.Equals(request.Email));
                if (thanhVienCapNhat != null)
                {
                    if (thanhVienCapNhat.MatKhau == request.MatKhauCu)
                    {
                        thanhVienCapNhat.MatKhau = request.MatKhauMoi;
                        int kq = _context.SaveChanges();
                        if (kq > 0)
                        {
                            tb.Maso = 0;
                            tb.NoiDung = "Thay đổi mật khẩu thành công";
                        }
                        else
                            tb.NoiDung = "Thay đổi mật khẩu không thành công";
                    }
                    else
                        tb.NoiDung = "Mật khẩu cũ không đúng.";
                }
            }
            catch (Exception ex)
            {
                tb.NoiDung = "Lỗi: " + ex.Message;
            }
            return Task.FromResult(tb);
        }

        public override Task<Output.Types.SlideBanners> DanhSachSlideBanner(Input.Types.Empty request, ServerCallContext context)
        {
            var dsSuatChieu = _context.SlideBanners
                .Select(x => new Output.Types.SlideBanner
                {
                    Id = x.Id,
                    Ten = x.Ten,
                    Hinh = x.Hinh,
                    LienKet = x.LienKet,
                    KichHoat = x.KichHoat
                });
            Output.Types.SlideBanners responseData = new Output.Types.SlideBanners();
            responseData.Items.AddRange(dsSuatChieu);
            return Task.FromResult(responseData);
        }

        public override Task<Output.Types.SlideBanner> XemThongTinSlideBanner(Input.Types.SlideBanner request, ServerCallContext context)
        {
            Output.Types.SlideBanner thongtinSlideBanner = new Output.Types.SlideBanner();
            if (request.Id > 0)
            {
                try
                {
                    var slideBanner = _context.SlideBanners.FirstOrDefault(x => x.Id.Equals(request.Id));
                    if (slideBanner != null)
                    {
                        thongtinSlideBanner = new Output.Types.SlideBanner
                        {
                            Id = slideBanner.Id,
                            Ten = slideBanner.Ten,
                            Hinh = slideBanner.Hinh,
                            LienKet = slideBanner.LienKet,
                            KichHoat = slideBanner.KichHoat
                        };
                    }
                    else
                        thongtinSlideBanner.ThongBao = "Lỗi: Không tìm thấy thông tin slide banner";
                }
                catch (Exception ex)
                {
                    thongtinSlideBanner.ThongBao = "Lỗi: " + ex.Message;
                }
            }
            else
            {
                thongtinSlideBanner.ThongBao = "Lỗi: Id Slide Banner không hợp lệ";
            }
            return Task.FromResult(thongtinSlideBanner);
        }

        public override Task<Output.Types.ThongBao> ThemSlideBannerMoi(Input.Types.SlideBanner request, ServerCallContext context)
        {
            Output.Types.ThongBao tb = new Output.Types.ThongBao { Maso = 1 };
            try
            {
                var bannerMoi = new Models.SlideBanner
                {
                    Id = request.Id,
                    Ten = request.Ten,
                    Hinh = request.Hinh,
                    LienKet = request.LienKet,
                    KichHoat = request.KichHoat
                };
                var chuoiTB = "";
                if (string.IsNullOrEmpty(bannerMoi.Hinh))
                    chuoiTB = "Hình phải khác rỗng";

                if (string.IsNullOrEmpty(chuoiTB))
                {
                    _context.SlideBanners.Add(bannerMoi);
                    int kq = _context.SaveChanges();
                    if (kq > 0)
                    {
                        tb.Maso = 0;
                        chuoiTB = "Lưu thông tin slide banner mới thành công";
                    }
                    else
                        chuoiTB = "Lưu thông tin slide banner mới không thành công";
                }

                tb.NoiDung = chuoiTB;
            }
            catch (Exception ex)
            {
                tb.NoiDung = "Lỗi: " + ex.Message;
            }
            return Task.FromResult(tb);
        }

        public override Task<Output.Types.ThongBao> CapNhatSlideBanner(Input.Types.SlideBanner request, ServerCallContext context)
        {
            Output.Types.ThongBao tb = new Output.Types.ThongBao { Maso = 1 };
            try
            {
                var bannerCapNhat = _context.SlideBanners.FirstOrDefault(p => p.Id.Equals(request.Id));
                if (bannerCapNhat != null)
                {
                    //Gán thông tin từ request vào phimCapNhat
                    bannerCapNhat.Hinh = request.Hinh;
                    bannerCapNhat.Ten = request.Ten;
                    bannerCapNhat.LienKet = request.LienKet;
                    bannerCapNhat.KichHoat = request.KichHoat;

                    var chuoiTB = "";
                    if (string.IsNullOrEmpty(bannerCapNhat.Hinh))
                        chuoiTB = "Hình phải khác rỗng";

                    if (string.IsNullOrEmpty(chuoiTB))
                    {
                        int kq = _context.SaveChanges();
                        if (kq > 0)
                        {
                            tb.Maso = 0;
                            chuoiTB = "Lưu thông tin slide banner thành công";
                        }
                        else
                            chuoiTB = "Lưu thông tin slide banner không thành công";
                    }
                    tb.NoiDung = chuoiTB;
                }
            }
            catch (Exception ex)
            {
                tb.NoiDung = "Lỗi: " + ex.Message;
            }
            return Task.FromResult(tb);
        }

        public override Task<Output.Types.ThongBao> XoaSlideBanner(Input.Types.SlideBanner request, ServerCallContext context)
        {
            Output.Types.ThongBao tb = new Output.Types.ThongBao { Maso = 1 };
            try
            {
                var bannerXoa = _context.SlideBanners.FirstOrDefault(p => p.Id.Equals(request.Id));
                if (bannerXoa != null)
                {
                    _context.SlideBanners.Remove(bannerXoa);
                    int kq = _context.SaveChanges();
                    var chuoiTB = "";
                    if (kq > 0)
                    {
                        tb.Maso = 0;
                        chuoiTB = "Xóa slide banner thành công";
                    }
                    else
                        chuoiTB = "Xóa slide banner không thành công";

                    tb.NoiDung = chuoiTB;
                }
            }
            catch (Exception ex)
            {
                tb.NoiDung = "Lỗi: " + ex.Message;
            }
            return Task.FromResult(tb);
        }

        public override Task<Output.Types.NhanVien> NhanVienDangNhap(Input.Types.ThongTinDangNhap request, ServerCallContext context)
        {
            Output.Types.NhanVien thongtinNhanVien = new Output.Types.NhanVien();
            if (!string.IsNullOrEmpty(request.TenDangNhap) && !string.IsNullOrEmpty(request.MatKhau))
            {
                try
                {
                    var nhanvien = _context.NhanViens.FirstOrDefault(x => x.Cmnd.Equals(request.TenDangNhap)
                                        && x.MatKhau.Equals(request.MatKhau));
                    if (nhanvien != null)
                    {
                        thongtinNhanVien = new Output.Types.NhanVien
                        {
                            Id = nhanvien.Id,
                            HoTen = nhanvien.HoTen,
                            GioiTinh = nhanvien.GioiTinh,
                            NgaySinh = Timestamp.FromDateTimeOffset(nhanvien.NgaySinh),
                            Cmnd = nhanvien.Cmnd,
                            DiaChi = nhanvien.DiaChi,
                            MatKhau = nhanvien.MatKhau,
                            RapId = nhanvien.RapId,
                            QuyenHan = nhanvien.QuyenHan
                        };
                    }
                    else
                    {
                        thongtinNhanVien.ThongBao = "Tên đăng nhập hoặc Mật khẩu không hợp lệ";
                    }
                }
                catch (Exception ex)
                {
                    thongtinNhanVien.ThongBao = "Lỗi đăng nhập: " + ex.Message;
                }
            }
            else
            {
                thongtinNhanVien.ThongBao = "Phải cung cấp Tên đăng nhập và Mật khẩu.";
            }
            return Task.FromResult(thongtinNhanVien);
        }

        public override Task<Output.Types.ThongBao> NhanVienThayDoiMatKhau(Input.Types.ThongTinThayDoiMatKhau request, ServerCallContext context)
        {
            Output.Types.ThongBao tb = new Output.Types.ThongBao { Maso = 1 };
            try
            {
                var nhanVienCapNhat = _context.NhanViens.FirstOrDefault(p => p.Id.Equals(request.Id) &&
                                                                                    p.Cmnd.Equals(request.Email));
                if (nhanVienCapNhat != null)
                {
                    if (nhanVienCapNhat.MatKhau == request.MatKhauCu)
                    {
                        nhanVienCapNhat.MatKhau = request.MatKhauMoi;
                        int kq = _context.SaveChanges();
                        if (kq > 0)
                        {
                            tb.Maso = 0;
                            tb.NoiDung = "Thay đổi mật khẩu thành công";
                        }
                        else
                            tb.NoiDung = "Thay đổi mật khẩu không thành công";
                    }
                    else
                        tb.NoiDung = "Mật khẩu cũ không đúng.";
                }
            }
            catch (Exception ex)
            {
                tb.NoiDung = "Lỗi: " + ex.Message;
            }
            return Task.FromResult(tb);
        }
    }
}