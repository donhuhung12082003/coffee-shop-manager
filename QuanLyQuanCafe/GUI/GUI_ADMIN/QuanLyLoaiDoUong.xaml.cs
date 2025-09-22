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
//thu vien xuat file excel
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using Microsoft.Win32;
namespace QuanLyQuanCafe.GUI.GUI_ADMIN
{
    /// <summary>
    /// Interaction logic for QuanLyLoaiDoUong.xaml
    /// </summary>
    public partial class QuanLyLoaiDoUong : Window
    {
        QuanLyQuanCafeContext db=new QuanLyQuanCafeContext();
        ManHinhChinh manhinhchinh = null;
        public QuanLyLoaiDoUong(ManHinhChinh mhc)
        {
            InitializeComponent();
            manhinhchinh = mhc;
        }
        private void hienthi_dgdsloaidouong()
        {
            dg_dsloaidouong.ItemsSource = db.LoaiDoUongs.ToList();
        }
        private void quanlydouong_Loaded(object sender, RoutedEventArgs e)
        {
            hienthi_dgdsloaidouong();
        }
        private bool check_nhap()
        {
            if (string.IsNullOrEmpty(txt_maloaidouong.Text.Trim()))
            {
                MessageBox.Show("chưa nhập mã loại đồ uống");
                txt_maloaidouong.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_tenloaidouong.Text.Trim()))
            {
                MessageBox.Show("chưa nhập tên loại đồ uống");
                txt_tenloaidouong.Focus(); return false;
            }
            return true;
        }
        private void btn_themloaidouong_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap()) return;
            var ldu_them = db.LoaiDoUongs.FirstOrDefault(l => l.MaLoai == txt_maloaidouong.Text);
            if (ldu_them != null)
            {
                MessageBox.Show("đã tồn tại mã đồ uống");
                txt_maloaidouong.Focus();
                return;
            }
            var ldu = new LoaiDoUong();
            ldu.MaLoai=txt_maloaidouong.Text;
            ldu.TenLoai=txt_tenloaidouong.Text;
            db.LoaiDoUongs.Add(ldu);
            db.SaveChanges();
            MessageBox.Show("thêm thành công");
            reset_nhap();
            hienthi_dgdsloaidouong();
        }
        private void reset_nhap()
        {
            txt_maloaidouong.Text= string.Empty;
            txt_tenloaidouong.Text= string.Empty;
            txt_timtheotenloaidouong.Text = string.Empty;
        }
        private void btn_sualoaidouong_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap()) return;
            var ldu_sua = db.LoaiDoUongs.FirstOrDefault(l => l.MaLoai == txt_maloaidouong.Text);
            if (ldu_sua == null)
            {
                MessageBox.Show("không tồn tại mã loại đồ uống này");
                txt_maloaidouong.Focus();
                return;
            }
            ldu_sua.TenLoai= txt_tenloaidouong.Text;
            db.SaveChanges();
            MessageBox.Show("sửa thành công");
            reset_nhap();
            hienthi_dgdsloaidouong() ;
        }
        private void btn_xoaloaidouong_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_maloaidouong.Text))
            {
                MessageBox.Show("chưa nhập mã loại đồ uống");
                txt_maloaidouong.Focus();
                return;
            }
            var ldu= db.LoaiDoUongs.FirstOrDefault(l=>l.MaLoai==txt_maloaidouong.Text);
            if(ldu == null)
            {
                MessageBox.Show("không tồn tại mã loại đồ uống này");
                txt_maloaidouong.Focus();
                return;
            }
            if (MessageBox.Show("xác nhận xóa", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            db.LoaiDoUongs.Remove(ldu);
            var ds_duuongthuocloaixoa=db.DoUongs.Where(du=>du.MaLoai==ldu.MaLoai).ToList();
            db.DoUongs.RemoveRange(ds_duuongthuocloaixoa);
            db.SaveChanges();
            MessageBox.Show("xóa thành công");
            reset_nhap();
            hienthi_dgdsloaidouong();
        }

        private void btn_nhaplai_Click(object sender, RoutedEventArgs e)
        {
            reset_nhap();
        }

        private void btn_timtheotenloaidouong_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_timtheotenloaidouong.Text))
            {
                MessageBox.Show("chưa nhập tên loại cần tìm");
                txt_timtheotenloaidouong.Focus();
                return;
            }
            var kq = db.LoaiDoUongs.Where(l => l.TenLoai.Trim().ToLower().Contains(txt_timtheotenloaidouong.Text.Trim().ToLower())).ToList();
            if (kq.Count == 0)
            {
                MessageBox.Show("không tìm thấy tên loại đồ uống này");
                txt_maloaidouong.Focus();
                hienthi_dgdsloaidouong();
                return;
            }
            dg_dsloaidouong.ItemsSource= kq;    
        }

        private void btn_thoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            manhinhchinh.hienthi_dgdsdouong();
            manhinhchinh.Show();
        }

        private void btn_xuatexceldanhsachloaidouong_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


            // Lấy dữ liệu từ EF Core
            var danhSach = db.LoaiDoUongs.ToList();

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Danh sách loại đồ uống");

                // Tạo tiêu đề cột
                worksheet.Cells[1, 1].Value = "Mã loại đồ uống";
                worksheet.Cells[1, 2].Value = "Tên loại đồ uống";
                
                // Định dạng tiêu đề (in đậm, căn giữa)
                using (var range = worksheet.Cells[1, 1, 1, 2])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Thêm dữ liệu từng dòng
                int row = 2;
                foreach (var ldu in danhSach)
                {
                    worksheet.Cells[row, 1].Value = ldu.MaLoai;
                    worksheet.Cells[row, 2].Value = ldu.TenLoai;
                    row++;
                }

                // Tự động co giãn cột cho đẹp
                worksheet.Cells.AutoFitColumns();

                // Chọn nơi lưu
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.FileName = "DanhSachLoaiDoUong.xlsx";

                if (saveDialog.ShowDialog() == true)
                {
                    var filePath = saveDialog.FileName;
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                    MessageBox.Show("Xuất Excel thành công!");
                }
            }
        }

        private void dg_dsloaidouong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dg_dsloaidouong.SelectedItem != null)
            {
                LoaiDoUong ldu=dg_dsloaidouong.SelectedItem as LoaiDoUong;
                txt_maloaidouong.Text = ldu.MaLoai;
                txt_tenloaidouong.Text=ldu.TenLoai;
            }
        }
    }
}
