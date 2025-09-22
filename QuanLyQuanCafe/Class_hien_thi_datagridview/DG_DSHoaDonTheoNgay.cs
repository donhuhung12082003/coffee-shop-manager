using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.Class_hien_thi_datagridview
{
    public class DG_DSHoaDonTheoNgay
    {
        public static int stt = 0;
        public int SoThuTu { get; set; } 
        public string NgayThanhToan { get; set; }
        public int SoLuongDoUongDaBan { get; set; }
        public int TongTien { get; set; }
        public DG_DSHoaDonTheoNgay()
        {
            SoThuTu = ++stt;
           
        }

    }
}
