using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QuanLyQuanCafe.Models;
//thư viện mở file explorer
using Microsoft.Win32;
//thu vien xuat file excel
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using QuanLyQuanCafe.Class_hien_thi_datagridview;
//
namespace QuanLyQuanCafe.GUI.GUI_ADMIN
{
    /// <summary>
    /// Interaction logic for ThongKeLichSuHoaDon.xaml
    /// </summary>
    public partial class ThongKeLichSuHoaDon : Window
    {
        QuanLyQuanCafeContext db=new QuanLyQuanCafeContext();
        ManHinhChinh manhinhchinh = null;
        public ThongKeLichSuHoaDon( ManHinhChinh manhinhchinh)
        {
            InitializeComponent();
            this.manhinhchinh = manhinhchinh;
        }
        private bool check_nhap()
        {
            if (string.IsNullOrEmpty(dp_tungay.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin ngày bắt đầu ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                dp_tungay.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(dp_denngay.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin ngày kết thúc ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                dp_denngay.Focus();
                return false;
            }
            var tungay = dp_tungay.SelectedDate.Value.Date;
            var denngay = dp_denngay.SelectedDate.Value.Date;
            if (tungay > denngay)
            {

                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc. Vui lòng nhập lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        private List<DG_DSLichSuHoaDon> laydulieu()
        {
            var tungay = DateOnly.FromDateTime(dp_tungay.SelectedDate.Value);
            var denngay = DateOnly.FromDateTime(dp_denngay.SelectedDate.Value);
            return (from hd in db.HoaDons
                    join cthd in db.ChiTietHoaDons on hd.MaHoaDon equals cthd.MaHoaDon
                    join nv in db.NhanViens on hd.MaNhanVien equals nv.MaNhanVien
                    join ban in db.Bans on hd.MaBan equals ban.MaBan
                    join tang in db.Tangs on ban.MaTang equals tang.MaTang
                    where hd.NgayLap >= tungay && hd.NgayLap <=denngay
                    group cthd by hd.MaHoaDon into g
                    select new DG_DSLichSuHoaDon
                    {
                        MaHoaDon=g.Key,
                        TenNhanVien=db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien==(db.HoaDons.FirstOrDefault(hd => hd.MaHoaDon==g.Key).MaNhanVien)).TenNhanVien,
                        ViTri=db.Bans.FirstOrDefault(b => b.MaBan==(db.HoaDons.FirstOrDefault(hd => hd.MaHoaDon==g.Key).MaBan)).TenBan + " Tầng "+db.Tangs.FirstOrDefault(t => t.MaTang==(db.Bans.FirstOrDefault(b => b.MaBan == (db.HoaDons.FirstOrDefault(hd => hd.MaHoaDon==g.Key).MaBan)).MaTang)).TenTang,
                        NgayThanhToan=db.HoaDons.FirstOrDefault(hd=>hd.MaHoaDon==g.Key).NgayLap.Value.ToString("dd/MM/yy"),
                        TongTien=g.Sum(ct=>(int)ct.SoLuong*(int)ct.DonGia)  

                    }).ToList<DG_DSLichSuHoaDon>();
        }
        private void btn_locdulieu_Click(object sender, RoutedEventArgs e)
        {
            if (!check_nhap())
            {
                return;
            }
            if (db.HoaDons.Count() == 0)
            {
                MessageBox.Show("Không có dữ liệu hóa đơn để thống kê.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            DG_DSLichSuHoaDon.stt = 0;
            dg_dshoadon.ItemsSource = laydulieu();
            int tongtien = laydulieu().Sum(hd => hd.TongTien);
            tb_tongtien.Text = tongtien.ToString("N0") + " VNĐ";
        }

        private void btn_xuatexcel_Click(object sender, RoutedEventArgs e)
        {
            if (!check_nhap())
            {
                return;
            }
            if (db.HoaDons.Count() == 0)
            {
                MessageBox.Show("Không có dữ liệu hóa đơn để thống kê.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


            // Lấy dữ liệu từ EF Core
            var danhSach = laydulieu();
            
            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Danh sách lịch sử hóa đơn");

                // Tạo tiêu đề cột
                worksheet.Cells[1, 1].Value = "Số thứ tự";
                worksheet.Cells[1, 2].Value = "Mã hóa đơn";
                worksheet.Cells[1, 3].Value = "Nhân viên";
                worksheet.Cells[1, 4].Value = "Vị trí";
                worksheet.Cells[1, 5].Value = "Ngày";
                worksheet.Cells[1, 6].Value = "Tổng tiền";
                // Định dạng tiêu đề (in đậm, căn giữa)
                using (var range = worksheet.Cells[1, 1, 1, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Thêm dữ liệu từng dòng
                int row = 2;
                int stt = 1;
                foreach (var hd in danhSach)
                {
                    worksheet.Cells[row, 1].Value = stt++;
                    worksheet.Cells[row, 2].Value = hd.MaHoaDon;
                    worksheet.Cells[row, 3].Value = hd.TenNhanVien;
                    worksheet.Cells[row, 4].Value = hd.ViTri;
                    worksheet.Cells[row, 5].Value = hd.NgayThanhToan;
                    worksheet.Cells[row, 6].Value = hd.TongTien;
                    row++;
                }

                // Tự động co giãn cột cho đẹp
                worksheet.Cells.AutoFitColumns();

                // Chọn nơi lưu
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.FileName = "DanhSachLichSuHoaDon.xlsx";

                if (saveDialog.ShowDialog() == true)
                {
                    var filePath = saveDialog.FileName;
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                    MessageBox.Show("Xuất Excel thành công!");
                }
            }
        }
        
        private void btn_thoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            if (manhinhchinh != null)
            {
                manhinhchinh.Show();
            }
        }

    }
}
