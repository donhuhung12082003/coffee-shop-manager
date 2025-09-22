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
//kiem tra bieu thuc chinh quy
using System.Text.RegularExpressions;
//
using System.Xml;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
//thư viện mở file explorer
using Microsoft.Win32;
//thu vien xuat file excel
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
//
namespace QuanLyQuanCafe.GUI.GUI_ADMIN
{
    /// <summary>
    /// Interaction logic for QuanLyNhanVien.xaml
    /// </summary>
    public partial class QuanLyNhanVien : Window
    {
        ManHinhChinh ManHinhChinh = null;
        TaiKhoan taiKhoan = null;
        QuanLyQuanCafeContext db = new QuanLyQuanCafeContext();
        public QuanLyNhanVien(ManHinhChinh manHinhChinh, TaiKhoan taiKhoan)
        {
            InitializeComponent();
            ManHinhChinh = manHinhChinh;
            this.taiKhoan = taiKhoan;
        }
        private bool check_nhap()
        {
            if (string.IsNullOrEmpty(txt_manhanvien.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập mã nhân viên");
                txt_manhanvien.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_tennhanvien.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập tên nhân viên");
                txt_tennhanvien.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_sodienthoainhanvien.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại nhân viên");
                txt_sodienthoainhanvien.Focus();
                return false;
            }
            if(txt_sodienthoainhanvien.Text.Length < 10)
            {
                MessageBox.Show("Số điện thoại phải có ít nhất 10 chữ số");
                txt_sodienthoainhanvien.Focus();
                return false;
            }
            string soDienThoai = txt_sodienthoainhanvien.Text.Trim();

            if (!Regex.IsMatch(soDienThoai, @"^\d{10,}$"))
            {
                MessageBox.Show("Số điện thoại phải có ít nhất 10 chữ số và chỉ chứa số!");
                txt_sodienthoainhanvien.Focus();
                return false;
            }
            var check_sdt_trung=db.NhanViens.FirstOrDefault(nv => nv.SoDienThoai.Trim() == soDienThoai);
            if (check_sdt_trung != null )
            {
                MessageBox.Show("Số điện thoại đã tồn tại, vui lòng nhập số khác");
                txt_sodienthoainhanvien.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_diachinhanvien.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ nhân viên");
                txt_diachinhanvien.Focus();
                return false;
            }
            if(string.IsNullOrEmpty(txt_tuoinhanvien.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập tuổi nhân viên");
                txt_tuoinhanvien.Focus();
                return false;
            }
            if (!int.TryParse(txt_tuoinhanvien.Text, out int tuoi) || tuoi <= 0)
            {
                MessageBox.Show("Tuổi nhân viên phải là một số nguyên dương");
                txt_tuoinhanvien.Focus();
                return false;
            }
            var ngayvaolam = dp_ngayvaolamnhanvien.SelectedDate;
            if (ngayvaolam == null)
            {
                MessageBox.Show("Vui lòng chọn ngày vào làm của nhân viên");
                dp_ngayvaolamnhanvien.Focus();
                return false;
            }
            if (ngayvaolam > DateTime.Now)
            {
                MessageBox.Show("Ngày vào làm không thể lớn hơn ngày hiện tại");
                dp_ngayvaolamnhanvien.Focus();
                return false;
            }
            return true;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void txt_tuoinhanvien_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btn_themnhanvien_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap())
            {
                return;
            }
            var nhanvien_them = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == txt_manhanvien.Text.Trim());
            if (nhanvien_them != null)
            {
                MessageBox.Show("Mã nhân viên đã tồn tại, vui lòng nhập mã khác");
                txt_manhanvien.Focus();
                return;
            }
            NhanVien nhanVien = new NhanVien();
            nhanVien.MaNhanVien = txt_manhanvien.Text.Trim();
            nhanVien.TenNhanVien = txt_tennhanvien.Text.Trim();
            nhanVien.SoDienThoai = txt_sodienthoainhanvien.Text.Trim();
            nhanVien.DiaChi = txt_diachinhanvien.Text.Trim();
            nhanVien.GioiTinh = radio_nam.IsChecked == true ? "Nam" : "Nữ";
            nhanVien.Tuoi = int.Parse(txt_tuoinhanvien.Text.Trim());
            DateTime ngayvaolam_dt = dp_ngayvaolamnhanvien.SelectedDate.Value;
            nhanVien.NgayVaoLam = DateOnly.FromDateTime(ngayvaolam_dt);
            // Tìm entity cũ đang bị track (nếu có)
            var trackedEntity = db.ChangeTracker.Entries<NhanVien>()
                                  .FirstOrDefault(e => e.Entity.MaNhanVien == nhanVien.MaNhanVien);

            if (trackedEntity != null)
            {
                db.Entry(trackedEntity.Entity).State = EntityState.Detached;
            }

            db.NhanViens.Add(nhanVien);
            db.SaveChanges();
            MessageBox.Show("Thêm nhân viên thành công");
            reset_nhap();
            hienthi_dgdsnhanvien();
        }
        private void reset_nhap()
        {
            txt_manhanvien.Clear();
            txt_tennhanvien.Clear();
            txt_sodienthoainhanvien.Clear();
            txt_diachinhanvien.Clear();
            txt_tuoinhanvien.Clear();
            dp_ngayvaolamnhanvien.SelectedDate = null;
            radio_nam.IsChecked = true;
            txt_timnhanvientheoten.Text = "";
        }

        private void btn_suanhanvien_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap())
            {
                return;
            }
            var nhanvien_sua = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == txt_manhanvien.Text.Trim());
            if (nhanvien_sua == null)
            {
                MessageBox.Show("Mã nhân viên không tồn tại, vui lòng nhập mã khác");
                txt_manhanvien.Focus();
                return;
            }
            nhanvien_sua.TenNhanVien = txt_tennhanvien.Text.Trim();
            nhanvien_sua.SoDienThoai = txt_sodienthoainhanvien.Text.Trim();
            nhanvien_sua.DiaChi = txt_diachinhanvien.Text.Trim();
            nhanvien_sua.GioiTinh = radio_nam.IsChecked == true ? "Nam" : "Nữ";
            nhanvien_sua.Tuoi = int.Parse(txt_tuoinhanvien.Text.Trim());
            DateTime ngayvaolam_dt = dp_ngayvaolamnhanvien.SelectedDate.Value;
            nhanvien_sua.NgayVaoLam = DateOnly.FromDateTime(ngayvaolam_dt);
            db.SaveChanges();
            MessageBox.Show("Sửa thông tin nhân viên thành công");
            reset_nhap();
            hienthi_dgdsnhanvien();
        }

        private void btn_xoanhanvien_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txt_manhanvien.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập mã nhân viên cần xóa");
                txt_manhanvien.Focus();
                return;
            }
            var nhanvien_xoa = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == txt_manhanvien.Text.Trim());
            if (nhanvien_xoa == null)
            {
                MessageBox.Show("Mã nhân viên không tồn tại, vui lòng nhập mã khác");
                txt_manhanvien.Focus();
                return;
            }
            if(nhanvien_xoa.MaNhanVien == taiKhoan.MaNhanVien)
            {
                MessageBox.Show("Không thể xóa chính mình");
                return;
            }
            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmResult == MessageBoxResult.Yes)
            {
                db.NhanViens.Remove(nhanvien_xoa);
                // Xóa tài khoản liên quan nếu có
                var taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.MaNhanVien == nhanvien_xoa.MaNhanVien);
                if (taiKhoan != null)
                {
                    db.TaiKhoans.Remove(taiKhoan);
                }
                db.SaveChanges();
                MessageBox.Show("Xóa nhân viên thành công");
                reset_nhap();
                hienthi_dgdsnhanvien();
            }
        }

        private void btn_thoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ManHinhChinh.hienthi_thongtinhoadon();
            ManHinhChinh.Show();
        }

        private void btn_nhaplai_Click(object sender, RoutedEventArgs e)
        {
            reset_nhap();
        }
        private void hienthi_dgdsnhanvien()
        {
            dg_dsnhanvien.ItemsSource = db.NhanViens.ToList();
        }
        private void quanlynhanvien_Loaded(object sender, RoutedEventArgs e)
        {
            hienthi_dgdsnhanvien();
        }

        private void btn_timnhanvientheoten_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_timnhanvientheoten.Text.Trim())){
                MessageBox.Show("Vui lòng nhập tên nhân viên để tìm kiếm");
                txt_timnhanvientheoten.Focus();
                return;
            }
            string ten=txt_timnhanvientheoten.Text.Trim().ToLower();
            var dstimthay= db.NhanViens.Where(nv => nv.TenNhanVien.ToLower().Contains(ten)).ToList();
            if (dstimthay.Count == 0)
            {
                MessageBox.Show("Không tìm thấy nhân viên nào với tên đã nhập");
                txt_timnhanvientheoten.Focus();
                hienthi_dgdsnhanvien();
                return;
            }
            dg_dsnhanvien.ItemsSource = dstimthay;
        }
        private void XuatDanhSachNhanVienRaExcel()
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


            // Lấy dữ liệu từ EF Core
            var danhSach = db.NhanViens.ToList();

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Danh sách nhân viên");

                // Tạo tiêu đề cột
                worksheet.Cells[1, 1].Value = "Mã nhân viên";
                worksheet.Cells[1, 2].Value = "Tên nhân viên";
                worksheet.Cells[1, 3].Value = "Địa chỉ";
                worksheet.Cells[1, 4].Value = "Ngày vào làm";
                worksheet.Cells[1, 5].Value = "Tuổi";
                worksheet.Cells[1, 6].Value = "Số điện thoại";
                worksheet.Cells[1, 7].Value = "Giới tính";

                // Định dạng tiêu đề (in đậm, căn giữa)
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Thêm dữ liệu từng dòng
                int row = 2;
                foreach (var nv in danhSach)
                {
                    worksheet.Cells[row, 1].Value = nv.MaNhanVien;
                    worksheet.Cells[row, 2].Value = nv.TenNhanVien;
                    worksheet.Cells[row, 3].Value = nv.DiaChi;
                    worksheet.Cells[row, 4].Value = nv.NgayVaoLam.Value.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 5].Value = nv.Tuoi;
                    worksheet.Cells[row, 6].Value = nv.SoDienThoai;
                    worksheet.Cells[row, 7].Value = nv.GioiTinh;
                    row++;
                }

                // Tự động co giãn cột cho đẹp
                worksheet.Cells.AutoFitColumns();

                // Chọn nơi lưu
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.FileName = "DanhSachNhanVien.xlsx";

                if (saveDialog.ShowDialog() == true)
                {
                    var filePath = saveDialog.FileName;
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                    MessageBox.Show("Xuất Excel thành công!");
                }
            }
        }

        private void btn_xuatexceldanhsachnhanvien_Click(object sender, RoutedEventArgs e)
        {
            XuatDanhSachNhanVienRaExcel();
        }

        private void dg_dsnhanvien_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dg_dsnhanvien.SelectedItem != null)
            {
                NhanVien selectedNhanVien = dg_dsnhanvien.SelectedItem as NhanVien;
               
                txt_manhanvien.Text = selectedNhanVien.MaNhanVien;
                txt_tennhanvien.Text = selectedNhanVien.TenNhanVien;
                txt_sodienthoainhanvien.Text = selectedNhanVien.SoDienThoai;
                txt_diachinhanvien.Text = selectedNhanVien.DiaChi;
                txt_tuoinhanvien.Text = selectedNhanVien.Tuoi.ToString();
                dp_ngayvaolamnhanvien.SelectedDate = selectedNhanVien.NgayVaoLam?.ToDateTime(new TimeOnly(0, 0));
                radio_nam.IsChecked = selectedNhanVien.GioiTinh == "Nam";
                radio_nu.IsChecked = selectedNhanVien.GioiTinh == "Nữ";
                
            }
        }
    }
}
