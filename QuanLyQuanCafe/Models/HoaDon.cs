using System;
using System.Collections.Generic;

namespace QuanLyQuanCafe.Models;

public partial class HoaDon
{
    public string MaHoaDon { get; set; } = null!;

    public string MaNhanVien { get; set; } = null!;

    public string MaBan { get; set; } = null!;

    public DateOnly? NgayLap { get; set; }

    public int? TongTien { get; set; }

    public bool? TrangThai { get; set; }
}
