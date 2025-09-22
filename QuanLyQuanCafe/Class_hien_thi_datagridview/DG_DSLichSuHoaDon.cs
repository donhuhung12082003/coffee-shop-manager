using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.Class_hien_thi_datagridview
{
    public class DG_DSLichSuHoaDon
    {
        public static int stt = 0;
        public int SoThuTu {  get; set; }
        public string MaHoaDon { get; set; }
        public string TenNhanVien { get; set; }
        public string ViTri {  get; set; }
        public string NgayThanhToan {  get; set; }
        public int TongTien { get; set; }
        public DG_DSLichSuHoaDon()
        {
            stt++; SoThuTu = stt;
        }
    }
}
