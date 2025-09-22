using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
//thu vien xuat file excel
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuanLyQuanCafe.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
//
namespace QuanLyQuanCafe.GUI.GUI_ADMIN
{
    /// <summary>
    /// Interaction logic for QuanLyNguyenLieu.xaml
    /// </summary>
    public partial class QuanLyNguyenLieu : Window
    {
        QuanLyQuanCafeContext db = new QuanLyQuanCafeContext();
        ManHinhChinh manHinhChinh = null;
        public QuanLyNguyenLieu( ManHinhChinh manHinhChinh)
        {
            InitializeComponent();
            this.manHinhChinh = manHinhChinh;
        }
        private void hienthi_dgdsnguyenlieu()
        {
            dg_dsnguyenlieu.ItemsSource = db.NguyenLieus.ToList();
        }
        private bool check_nhap()
        {
            if (string.IsNullOrEmpty(txt_ma.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập mã nguyên liệu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_ma.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_ten.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập tên nguyên liệu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_ten.Focus();
                return false;
            }
            return true;
        }
        private void reset_nhap()
        {
            txt_ma.Text = "";
            txt_ten.Text = "";
            txt_timtheoten.Text = "";
        }
        private void quanlynguyenlieu_Loaded(object sender, RoutedEventArgs e)
        {
            hienthi_dgdsnguyenlieu();
        }

        private void btn_them_Click(object sender, RoutedEventArgs e)
        {
            if (!check_nhap()) return;
            var nl_them=db.NguyenLieus.FirstOrDefault(nl => nl.MaNguyenLieu == txt_ma.Text.Trim());
            if (nl_them != null)
            {
                MessageBox.Show("Mã nguyên liệu đã tồn tại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_ma.Focus();
                return;
            }
            NguyenLieu nguyenLieu = new NguyenLieu
            {
                MaNguyenLieu = txt_ma.Text.Trim(),
                TenNguyenLieu = txt_ten.Text.Trim()
            };

            // Tìm entity cũ đang bị track (nếu có)
            var trackedEntity = db.ChangeTracker.Entries<NguyenLieu>()
                                  .FirstOrDefault(e => e.Entity.MaNguyenLieu == nguyenLieu.MaNguyenLieu);

            if (trackedEntity != null)
            {
                db.Entry(trackedEntity.Entity).State = EntityState.Detached;
            }
            db.NguyenLieus.Add(nguyenLieu);
            db.SaveChanges();
            MessageBox.Show("Thêm nguyên liệu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            hienthi_dgdsnguyenlieu();
            reset_nhap();
        }

        private void btn_sua_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap()) return;
            var nl_sua = db.NguyenLieus.FirstOrDefault(nl => nl.MaNguyenLieu == txt_ma.Text.Trim());
            if (nl_sua == null)
            {
                MessageBox.Show("Nguyên liệu không tồn tại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_ma.Focus();
                return;
            }
            nl_sua.TenNguyenLieu = txt_ten.Text.Trim();
            db.SaveChanges();
            MessageBox.Show("Sửa nguyên liệu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            hienthi_dgdsnguyenlieu();
            reset_nhap();
        }

        private void btn_xoa_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txt_ma.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập mã nguyên liệu cần xóa", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_ma.Focus();
                return;
            }
            var nl_xoa = db.NguyenLieus.FirstOrDefault(nl => nl.MaNguyenLieu == txt_ma.Text.Trim());
            if (nl_xoa == null)
            {
                MessageBox.Show("Nguyên liệu không tồn tại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_ma.Focus();
                return;
            }
            var dsdouongdungnguyenlieu = db.CongThucPhaChes.Where(ct => ct.MaNguyenLieu == nl_xoa.MaNguyenLieu).ToList();
            if (dsdouongdungnguyenlieu.Count > 0)
            {
                if (MessageBox.Show("Nguyên liệu này đang được dùng trong công thức pha chế, có chắc muốn xóa?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
                db.CongThucPhaChes.RemoveRange(dsdouongdungnguyenlieu);
                db.NguyenLieus.Remove(nl_xoa);
                var ds_congthucphachedungnguyenlieu = db.CongThucPhaChes.Where((ct) => ct.MaNguyenLieu == nl_xoa.MaNguyenLieu).ToList();
                db.CongThucPhaChes.RemoveRange(ds_congthucphachedungnguyenlieu);
                db.SaveChanges();
                MessageBox.Show("Xóa nguyên liệu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                hienthi_dgdsnguyenlieu();
                reset_nhap();
            }
            else
            {
                if (MessageBox.Show("Bạn có chắc muốn xóa nguyên liệu này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    db.NguyenLieus.Remove(nl_xoa);
                    db.SaveChanges();
                    MessageBox.Show("Xóa nguyên liệu thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    reset_nhap();
                    hienthi_dgdsnguyenlieu();
                }
            }
            
        }

        private void btn_nhaplai_Click(object sender, RoutedEventArgs e)
        {
            reset_nhap();
        }

        private void dg_dsnguyenlieu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_dsnguyenlieu.SelectedItem != null)
            {
                NguyenLieu nguyenLieu = dg_dsnguyenlieu.SelectedItem as NguyenLieu;
                txt_ma.Text = nguyenLieu.MaNguyenLieu;
                txt_ten.Text = nguyenLieu.TenNguyenLieu;
            }
        }

        private void btn_thoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            manHinhChinh.Show();
        }

        private void btn_xuatexcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


            // Lấy dữ liệu từ EF Core
            var danhSach = db.NguyenLieus.ToList();

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Danh sách nguyên liệu");

                // Tạo tiêu đề cột
                worksheet.Cells[1, 1].Value = "Mã nguyên liệu";
                worksheet.Cells[1, 2].Value = "Tên nguyên liệu";
                
                // Định dạng tiêu đề (in đậm, căn giữa)
                using (var range = worksheet.Cells[1, 1, 1, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Thêm dữ liệu từng dòng
                int row = 2;
                foreach (var nl in danhSach)
                {
                    worksheet.Cells[row, 1].Value = nl.MaNguyenLieu;
                    worksheet.Cells[row, 2].Value = nl.TenNguyenLieu;
                   
                    row++;
                }

                // Tự động co giãn cột cho đẹp
                worksheet.Cells.AutoFitColumns();

                // Chọn nơi lưu
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.FileName = "DanhSachNguyenLieu.xlsx";

                if (saveDialog.ShowDialog() == true)
                {
                    var filePath = saveDialog.FileName;
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                    MessageBox.Show("Xuất Excel thành công!");
                }
            }
        }

        private void btn_timtheoten_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txt_timtheoten.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập tên nguyên liệu cần tìm", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_timtheoten.Focus();
                return;
            }
            var kq=db.NguyenLieus.Where(nl => nl.TenNguyenLieu.ToLower().Trim().Contains(txt_timtheoten.Text.Trim().ToLower())).ToList();
            if (kq.Count == 0)
            {
                MessageBox.Show("Không tìm thấy nguyên liệu nào với tên này", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                hienthi_dgdsnguyenlieu();
            }
            else
            {
                dg_dsnguyenlieu.ItemsSource = kq;
            }
        }
    }
}
