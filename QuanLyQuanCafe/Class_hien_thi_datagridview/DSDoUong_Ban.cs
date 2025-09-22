using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLyQuanCafe.Models;
namespace QuanLyQuanCafe.Class_hien_thi_datagridview
{
    public class DSDoUong_Ban
    {
        public Ban ban {  get; set; }
        public List<DG_DSDoUongGoi> dsDoUong = new List<DG_DSDoUongGoi>();
    }
}
