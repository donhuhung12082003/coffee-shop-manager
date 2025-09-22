using System;
using System.Collections.Generic;

namespace QuanLyQuanCafe.Models;

public partial class CongThucPhaChe
{
    public string MaCongThuc { get; set; } = null!;

    public string? MaDoUong { get; set; }

    public string? MaNguyenLieu { get; set; }

    public string? MoTaSoLuongNguyenLieuCanDung { get; set; }
}
