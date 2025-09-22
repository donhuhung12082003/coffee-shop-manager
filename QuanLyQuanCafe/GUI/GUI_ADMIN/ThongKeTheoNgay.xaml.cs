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
using QuanLyQuanCafe.Class_hien_thi_datagridview;
//thu vien xuat file excel
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using Microsoft.Win32;
//
namespace QuanLyQuanCafe.GUI.GUI_ADMIN
{
    /// <summary>
    /// Interaction logic for ThongKeTheoNgay.xaml
    /// </summary>
    public partial class ThongKeTheoNgay : Window
    {
        QuanLyQuanCafeContext db = new QuanLyQuanCafeContext();
        ManHinhChinh manHinhChinh = null;
        
        public ThongKeTheoNgay(ManHinhChinh mhc)
        {
            InitializeComponent();
            manHinhChinh = mhc;
        }
        private bool check_nhap()
        {
           if(string.IsNullOrEmpty(dp_tungay.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin ngày bắt đầu ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                dp_tungay.Focus();
                return false;
            }
           if(string.IsNullOrEmpty(dp_denngay.Text.Trim()))
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            if (manHinhChinh != null)
            {
                manHinhChinh.Show();
            }
        }
        private List<DG_DSHoaDonTheoNgay> laydulieu()
        {
            var tungay = DateOnly.FromDateTime(dp_tungay.SelectedDate.Value);
            var denngay = DateOnly.FromDateTime(dp_denngay.SelectedDate.Value);
            return (from hd in db.HoaDons
                    join cthd in db.ChiTietHoaDons on hd.MaHoaDon equals cthd.MaHoaDon
                    where hd.NgayLap >= tungay && hd.NgayLap <= denngay
                    group cthd by new { hd.NgayLap, hd.MaHoaDon } into g
                    select new DG_DSHoaDonTheoNgay
                    {
                        NgayThanhToan = g.Key.NgayLap.Value.ToString("dd/MM/yyyy"),
                        SoLuongDoUongDaBan = g.Sum(ct => (int)ct.SoLuong),
                        TongTien = g.Sum(ct => (int)ct.SoLuong * (int)ct.DonGia)
                    }).ToList<DG_DSHoaDonTheoNgay>();
        }
        private void btn_locdulieu_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap())
            {
                return;
            }
            if (db.HoaDons.Count() == 0)
            {
                MessageBox.Show("Không có dữ liệu hóa đơn để thống kê.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            DG_DSHoaDonTheoNgay.stt = 0;
            dg_dshoadon.ItemsSource = laydulieu();
            int tongtien=laydulieu().Sum(hd => hd.TongTien);
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
                var worksheet = package.Workbook.Worksheets.Add("Danh sách hóa đơn");

                // Tạo tiêu đề cột
                worksheet.Cells[1, 1].Value = "Số thứ tự";
                worksheet.Cells[1, 2].Value = "Ngày thanh toán";
                worksheet.Cells[1, 3].Value = "Số lượng đồ uống đã bán";
                worksheet.Cells[1, 4].Value = "Tổng tiền";
                
                // Định dạng tiêu đề (in đậm, căn giữa)
                using (var range = worksheet.Cells[1, 1, 1, 4])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Thêm dữ liệu từng dòng
                int row = 2;
                foreach (var hd in danhSach)
                {
                    worksheet.Cells[row, 1].Value = hd.SoThuTu;
                    worksheet.Cells[row, 2].Value = hd.NgayThanhToan;
                    worksheet.Cells[row, 3].Value = hd.SoLuongDoUongDaBan;
                    worksheet.Cells[row, 4].Value = hd.TongTien;
                    row++;
                }

                // Tự động co giãn cột cho đẹp
                worksheet.Cells.AutoFitColumns();

                // Chọn nơi lưu
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.FileName = "DanhSachHoaDonTheoNgay.xlsx";

                if (saveDialog.ShowDialog() == true)
                {
                    var filePath = saveDialog.FileName;
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                    MessageBox.Show("Xuất Excel thành công!");
                }
            }
        }
    }
}
