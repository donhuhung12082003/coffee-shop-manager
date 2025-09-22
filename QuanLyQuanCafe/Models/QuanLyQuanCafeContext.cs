using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyQuanCafe.Models;

public partial class QuanLyQuanCafeContext : DbContext
{
    public QuanLyQuanCafeContext()
    {
    }

    public QuanLyQuanCafeContext(DbContextOptions<QuanLyQuanCafeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ban> Bans { get; set; }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<CongThucPhaChe> CongThucPhaChes { get; set; }

    public virtual DbSet<DoUong> DoUongs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<LoaiDoUong> LoaiDoUongs { get; set; }

    public virtual DbSet<NguyenLieu> NguyenLieus { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<Tang> Tangs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-QIS0RT7P\\SQLEXPRESS;Initial Catalog=QuanLyQuanCafe;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ban>(entity =>
        {
            entity.HasKey(e => e.MaBan);

            entity.ToTable("Ban");

            entity.Property(e => e.MaBan)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaTang)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenBan).HasMaxLength(20);
        });

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => new { e.MaHoaDon, e.MaDoUong });

            entity.ToTable("ChiTietHoaDon");

            entity.Property(e => e.MaHoaDon)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaDoUong)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<CongThucPhaChe>(entity =>
        {
            entity.HasKey(e => e.MaCongThuc);

            entity.ToTable("CongThucPhaChe");

            entity.Property(e => e.MaCongThuc)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaDoUong)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaNguyenLieu)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MoTaSoLuongNguyenLieuCanDung).HasColumnType("text");
        });

        modelBuilder.Entity<DoUong>(entity =>
        {
            entity.HasKey(e => e.MaDoUong);

            entity.ToTable("DoUong");

            entity.Property(e => e.MaDoUong)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.MaLoai)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenDoUong).HasMaxLength(255);
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon);

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHoaDon)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaBan)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<LoaiDoUong>(entity =>
        {
            entity.HasKey(e => e.MaLoai);

            entity.ToTable("LoaiDoUong");

            entity.Property(e => e.MaLoai)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenLoai).HasMaxLength(255);
        });

        modelBuilder.Entity<NguyenLieu>(entity =>
        {
            entity.HasKey(e => e.MaNguyenLieu);

            entity.ToTable("NguyenLieu");

            entity.Property(e => e.MaNguyenLieu)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenNguyenLieu).HasMaxLength(255);
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien);

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.GioiTinh).HasMaxLength(20);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenNhanVien).HasMaxLength(255);
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan);

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.MaTaiKhoan)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(255)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Tang>(entity =>
        {
            entity.HasKey(e => e.MaTang);

            entity.ToTable("Tang");

            entity.Property(e => e.MaTang)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenTang).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
