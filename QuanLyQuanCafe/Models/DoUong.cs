using System;
using System.Collections.Generic;

namespace QuanLyQuanCafe.Models;

public partial class DoUong
{
    public string MaDoUong { get; set; } = null!;

    public string MaLoai { get; set; } = null!;

    public string? TenDoUong { get; set; }

    public int? DonGia { get; set; }

    public string? HinhAnh { get; set; }
}
