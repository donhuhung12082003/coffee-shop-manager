using System;
using System.Collections.Generic;

namespace QuanLyQuanCafe.Models;

public partial class TaiKhoan
{
    public string MaTaiKhoan { get; set; } = null!;

    public string? TenDangNhap { get; set; }

    public string? MatKhau { get; set; }

    public string? MaNhanVien { get; set; }

    public bool? Quyen { get; set; }
}
