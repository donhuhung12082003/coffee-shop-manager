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
using System.Windows.Media.Animation;
//
namespace QuanLyQuanCafe.GUI.GUI_ADMIN
{
    /// <summary>
    /// Interaction logic for ThongKeTheoDoUong.xaml
    /// </summary>
    public partial class ThongKeTheoDoUong : Window
    {
        QuanLyQuanCafeContext db=new QuanLyQuanCafeContext();
        ManHinhChinh manhinhchinh = null;
        public ThongKeTheoDoUong( ManHinhChinh manhinhchinh)
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
        private List<DG_DSHoaDonTheoDoUong> laydulieu()
        {
            var tungay = DateOnly.FromDateTime(dp_tungay.SelectedDate.Value);
            var denngay = DateOnly.FromDateTime(dp_denngay.SelectedDate.Value);
            return (from du in db.DoUongs
                    join cthd in db.ChiTietHoaDons on du.MaDoUong equals cthd.MaDoUong
                    join hd in db.HoaDons on cthd.MaHoaDon equals hd.MaHoaDon
                    where hd.NgayLap >= tungay && hd.NgayLap <=denngay
                    group cthd by du.MaDoUong into g
                    select new DG_DSHoaDonTheoDoUong
                    {
                        MaDoUong=g.Key,
                        TenDoUong=db.DoUongs.FirstOrDefault(du=>du.MaDoUong==g.Key).TenDoUong,
                        SoLuongDaBan=g.Sum(ct=>(int)ct.SoLuong),
                        TongTien=g.Sum(ct=>(int)ct.SoLuong*(int)ct.DonGia)
                    }).ToList<DG_DSHoaDonTheoDoUong>();
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
            DG_DSHoaDonTheoDoUong.stt = 0;
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
                var worksheet = package.Workbook.Worksheets.Add("Danh sách hóa đơn");

                // Tạo tiêu đề cột
                worksheet.Cells[1, 1].Value = "Số thứ tự";
                worksheet.Cells[1, 2].Value = "Mã đồ uống";
                worksheet.Cells[1, 3].Value = "Tên đồ uống";
                worksheet.Cells[1, 4].Value = "Số lượng đồ uống đã bán";
                worksheet.Cells[1, 5].Value = "Tổng tiền";

                // Định dạng tiêu đề (in đậm, căn giữa)
                using (var range = worksheet.Cells[1, 1, 1, 5])
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
                    worksheet.Cells[row, 2].Value = hd.MaDoUong;
                    worksheet.Cells[row, 3].Value = hd.TenDoUong;
                    worksheet.Cells[row, 4].Value = hd.SoLuongDaBan;
                    worksheet.Cells[row, 5].Value = hd.TongTien;
                    row++;
                }

                // Tự động co giãn cột cho đẹp
                worksheet.Cells.AutoFitColumns();

                // Chọn nơi lưu
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.FileName = "DanhSachHoaDonTheoDoUong.xlsx";

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
