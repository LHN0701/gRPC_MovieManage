using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace gRPC_QLRapChieuPhim.Models
{
    public partial class QLRapChieuPhimContext : DbContext
    {
        public QLRapChieuPhimContext()
        {
        }

        public QLRapChieuPhimContext(DbContextOptions<QLRapChieuPhimContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ghe> Ghes { get; set; }
        public virtual DbSet<LichChieu> LichChieus { get; set; }
        public virtual DbSet<LoaiGhe> LoaiGhes { get; set; }
        public virtual DbSet<NhanVien> NhanViens { get; set; }
        public virtual DbSet<Phim> Phims { get; set; }
        public virtual DbSet<Phong> Phongs { get; set; }
        public virtual DbSet<Rap> Raps { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<SlideBanner> SlideBanners { get; set; }
        public virtual DbSet<SuatChieu> SuatChieus { get; set; }
        public virtual DbSet<ThanhVien> ThanhViens { get; set; }
        public virtual DbSet<TheLoaiPhim> TheLoaiPhims { get; set; }
        public virtual DbSet<Ve> Ves { get; set; }
        public virtual DbSet<XepHangPhim> XepHangPhims { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=QLRapChieuPhimContext");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Ghe>(entity =>
            {
                entity.ToTable("Ghe");

                entity.Property(e => e.Day)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsFixedLength(true);

                entity.Property(e => e.SoGhe)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.LoaiGhe)
                    .WithMany(p => p.Ghes)
                    .HasForeignKey(d => d.LoaiGheId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ghe_LoaiGhe");

                entity.HasOne(d => d.Phong)
                    .WithMany(p => p.Ghes)
                    .HasForeignKey(d => d.PhongId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ghe_Phong");
            });

            modelBuilder.Entity<LichChieu>(entity =>
            {
                entity.ToTable("LichChieu");

                entity.Property(e => e.NgayChieu).HasColumnType("date");

                entity.HasOne(d => d.Phim)
                    .WithMany(p => p.LichChieus)
                    .HasForeignKey(d => d.PhimId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LichChieu_Phim");

                entity.HasOne(d => d.Phong)
                    .WithMany(p => p.LichChieus)
                    .HasForeignKey(d => d.PhongId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LichChieu_Phong");

                entity.HasOne(d => d.SuatChieu)
                    .WithMany(p => p.LichChieus)
                    .HasForeignKey(d => d.SuatChieuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LichChieu_SuatChieu");
            });

            modelBuilder.Entity<LoaiGhe>(entity =>
            {
                entity.ToTable("LoaiGhe");

                entity.Property(e => e.TenLoaiGhe)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.ToTable("NhanVien");

                entity.Property(e => e.Cmnd)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.DiaChi)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.HoTen)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MatKhau)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NgaySinh).HasColumnType("datetime");

                entity.Property(e => e.QuyenHan)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Phim>(entity =>
            {
                entity.ToTable("Phim");

                entity.Property(e => e.DanhSachTheLoaiId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DaoDien).HasMaxLength(100);

                entity.Property(e => e.DienVien).HasMaxLength(200);

                entity.Property(e => e.NgayKhoiChieu).HasColumnType("date");

                entity.Property(e => e.NgonNgu).HasMaxLength(100);

                entity.Property(e => e.NhaSanXuat).HasMaxLength(200);

                entity.Property(e => e.NuocSanXuat).HasMaxLength(200);

                entity.Property(e => e.Poster).IsUnicode(false);

                entity.Property(e => e.TenGoc).HasMaxLength(200);

                entity.Property(e => e.TenPhim)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Trailer)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.XepHangPhim)
                    .WithMany(p => p.Phims)
                    .HasForeignKey(d => d.XepHangPhimId)
                    .HasConstraintName("FK_Phim_XepHangPhim");
            });

            modelBuilder.Entity<Phong>(entity =>
            {
                entity.ToTable("Phong");

                entity.Property(e => e.GhiChu).HasMaxLength(200);

                entity.Property(e => e.TenPhong)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Rap)
                    .WithMany(p => p.Phongs)
                    .HasForeignKey(d => d.RapId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Phong_Rap");
            });

            modelBuilder.Entity<Rap>(entity =>
            {
                entity.ToTable("Rap");

                entity.Property(e => e.DiaChi).HasMaxLength(200);

                entity.Property(e => e.DienThoai).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.TenRap)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("Rating");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Ip)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ngay).HasColumnType("datetime");
            });

            modelBuilder.Entity<SlideBanner>(entity =>
            {
                entity.ToTable("SlideBanner");

                entity.Property(e => e.Hinh)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.LienKet)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Ten)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<SuatChieu>(entity =>
            {
                entity.ToTable("SuatChieu");

                entity.Property(e => e.GioBatDau)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsFixedLength(true);

                entity.Property(e => e.GioKetThuc)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsFixedLength(true);

                entity.Property(e => e.TenSuatChieu)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<ThanhVien>(entity =>
            {
                entity.ToTable("ThanhVien");

                entity.Property(e => e.DienThoai)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.HoTen)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MatKhau)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NgaySinh).HasColumnType("datetime");
            });

            modelBuilder.Entity<TheLoaiPhim>(entity =>
            {
                entity.ToTable("TheLoaiPhim");

                entity.Property(e => e.Ten)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Ve>(entity =>
            {
                entity.ToTable("Ve");

                entity.Property(e => e.NgayBanVe).HasColumnType("datetime");

                entity.Property(e => e.SoGhe)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.TinhTrang).HasComment("0: Còn trống; 1: Đã bán; 2: Đặt chổ");

                entity.HasOne(d => d.LichChieu)
                    .WithMany(p => p.Ves)
                    .HasForeignKey(d => d.LichChieuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ve_LichChieu");

                entity.HasOne(d => d.NhanVien)
                    .WithMany(p => p.Ves)
                    .HasForeignKey(d => d.NhanVienId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ve_NhanVien");
            });

            modelBuilder.Entity<XepHangPhim>(entity =>
            {
                entity.ToTable("XepHangPhim");

                entity.Property(e => e.KyHieu)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Ten)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        private void OnModelCreatingPartial(ModelBuilder modelBuilder)
        { }
    }
}