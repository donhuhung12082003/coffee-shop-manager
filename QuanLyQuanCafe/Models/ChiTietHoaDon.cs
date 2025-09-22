using System;
using System.Collections.Generic;

namespace QuanLyQuanCafe.Models;

public partial class ChiTietHoaDon
{
    public string MaHoaDon { get; set; } = null!;

    public string MaDoUong { get; set; } = null!;

    public int? SoLuong { get; set; }

    public int? DonGia { get; set; }

    public int? ThanhTien { get; set; }
}
