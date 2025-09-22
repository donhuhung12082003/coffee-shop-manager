using System;
using System.Collections.Generic;

namespace QuanLyQuanCafe.Models;

public partial class NhanVien
{
    public string MaNhanVien { get; set; } = null!;

    public string? TenNhanVien { get; set; }

    public string? DiaChi { get; set; }

    public DateOnly? NgayVaoLam { get; set; }

    public int? Tuoi { get; set; }

    public string? SoDienThoai { get; set; }

    public string? GioiTinh { get; set; }
}
