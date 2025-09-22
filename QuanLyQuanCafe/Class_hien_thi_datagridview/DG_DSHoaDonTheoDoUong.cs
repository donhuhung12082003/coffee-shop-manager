using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.Class_hien_thi_datagridview
{
    public class DG_DSHoaDonTheoDoUong
    {
        public static int stt = 0;
        public int SoThuTu {  get; set; }
        public string MaDoUong { get; set; }    
        public string TenDoUong { get; set; }  
        public int SoLuongDaBan { get; set; }
        public int TongTien {  get; set; }
        public DG_DSHoaDonTheoDoUong()
        {
            stt++; SoThuTu = stt;
        }
    }
}
