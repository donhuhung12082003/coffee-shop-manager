using System;
using System.Collections.Generic;

namespace QuanLyQuanCafe.Models;

public partial class Ban
{
    public string MaBan { get; set; } = null!;

    public string MaTang { get; set; } = null!;

    public string? TenBan { get; set; }

    public bool? TrangThai { get; set; }
}
