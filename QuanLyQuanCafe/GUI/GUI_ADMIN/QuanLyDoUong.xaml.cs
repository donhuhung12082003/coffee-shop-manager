using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Microsoft.Win32;
//thu vien xuat file excel
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using QuanLyQuanCafe.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
namespace QuanLyQuanCafe.GUI.GUI_ADMIN
{
    /// <summary>
    /// Interaction logic for QuanLyDoUong.xaml
    /// </summary>
    public partial class QuanLyDoUong : Window
    {
        QuanLyQuanCafeContext db= new QuanLyQuanCafeContext();
        ManHinhChinh manhinhchinh;
        public QuanLyDoUong(ManHinhChinh chinhChinh)
        {
            InitializeComponent();
            this.manhinhchinh = chinhChinh;
        }

        private void btn_chonanh_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            string duongDanAnh;
            if (openFileDialog.ShowDialog() == true)
            {
                duongDanAnh = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(duongDanAnh);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                img_anhdouong.Source = bitmap;
            }
        }

        private void btn_xoaanh_Click(object sender, RoutedEventArgs e)
        {
            img_anhdouong.Source = null;

        }
        private void hienthi_cboloaidouong()
        {
            cbo_loaidouong.ItemsSource=db.LoaiDoUongs.ToList();
            cbo_loaidouong.DisplayMemberPath = "TenLoai";
            cbo_loaidouong.SelectedValuePath = "MaLoai";
            cbo_loaidouong.SelectedIndex = 0;
        }
        private void hienthi_dgdsdouong()
        {
            dg_dsdouong.ItemsSource=db.DoUongs.ToList();
        }
        private void quanlydouong_Loaded(object sender, RoutedEventArgs e)
        {
            hienthi_cboloaidouong();
            hienthi_dgdsdouong();
        }
        private void reset_nhap()
        {
            txt_madouong.Text = string.Empty;
            txt_tendouong.Text = string.Empty;
            txt_giadouong.Text = string.Empty;
            txt_timtheoten.Text = string.Empty;
            cbo_loaidouong.SelectedIndex=0;
            img_anhdouong.Source = null;
        }
        private bool check_nhap()
        {
            if (string.IsNullOrEmpty(txt_madouong.Text.Trim()))
            {
                MessageBox.Show("chưa nhập mã đồ uống");
                txt_madouong.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_tendouong.Text.Trim()))
            {
                MessageBox.Show("chưa nhập tên đồ uống");
                txt_tendouong.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_giadouong.Text.Trim()))
            {
                MessageBox.Show("chưa nhập giá đồ uống");
                txt_giadouong.Focus();
                return false;
            }
            if(!int.TryParse(txt_giadouong.Text.Trim(), out int gia))
            {
                MessageBox.Show("giá đồ uống phải là số nguyên dương");
                txt_giadouong.Focus();
                return false;
            }
            if (gia <= 0)
            {
                MessageBox.Show("giá đồ uống phải là số nguyên dương lớn hơn 0");
                txt_giadouong.Focus();
                return false;
            }
            return true;    
        }
        private void btn_themdouong_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap()) return;
            var du_them = db.DoUongs.FirstOrDefault(du => du.MaDoUong == txt_madouong.Text.Trim());
            if (du_them != null)
            {
                MessageBox.Show("mã đồ uống đã tồn tại");
                txt_madouong.Focus();
                return;
            }
            var du = new DoUong();
            du.MaDoUong = txt_madouong.Text.Trim();
            du.TenDoUong=txt_tendouong.Text.Trim(); 
            du.DonGia=int.Parse(txt_giadouong.Text.Trim());
            du.MaLoai = cbo_loaidouong.SelectedValue.ToString();
            du.HinhAnh = img_anhdouong.Source == null ? null: img_anhdouong.Source.ToString();

            // Tìm entity cũ đang bị track (nếu có)
            var trackedEntity = db.ChangeTracker.Entries<DoUong>()
                                  .FirstOrDefault(e => e.Entity.MaDoUong == du.MaDoUong);

            if (trackedEntity != null)
            {
                db.Entry(trackedEntity.Entity).State = EntityState.Detached;
            }
            db.DoUongs.Add(du);
        
            db.SaveChanges();
            MessageBox.Show("Thêm thành công");
            hienthi_dgdsdouong();
            reset_nhap();
        }

        private void btn_suadouong_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap()) return;
            var du_sua=db.DoUongs.FirstOrDefault(du=>du.MaDoUong==txt_madouong.Text.Trim());
            if (du_sua == null)
            {
                MessageBox.Show("Không tồn tại mã đồ uống này");
                txt_madouong.Focus();  
                return;
            }
            du_sua.DonGia=int.Parse(txt_giadouong.Text.Trim());
            du_sua.TenDoUong= txt_tendouong.Text.Trim();
            du_sua.MaLoai = cbo_loaidouong.SelectedValue.ToString();
            du_sua.HinhAnh = img_anhdouong.Source == null ? null : img_anhdouong.Source.ToString();
            db.SaveChanges();
            MessageBox.Show("sửa thành công");
            hienthi_dgdsdouong();
            reset_nhap();
        }

        private void btn_xoadouong_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_madouong.Text.Trim()))
            {
                MessageBox.Show("chưa nhập mã đồ uống");
                txt_madouong.Focus();
                return;
            }
            var du_xoa=db.DoUongs.FirstOrDefault(du=>du.MaDoUong==txt_madouong.Text.Trim());
            if (du_xoa == null)
            {
                MessageBox.Show("mã đồ uống không tồn tại");
                txt_madouong.Focus();
                return;
            }
            if (MessageBox.Show("xác nhận xóa", "xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            db.DoUongs.Remove(du_xoa);
            var ds_congthucphachedouongxoa = db.CongThucPhaChes.Where(ct => ct.MaDoUong == du_xoa.MaDoUong).ToList();
            db.CongThucPhaChes.RemoveRange(ds_congthucphachedouongxoa);
            db.SaveChanges();
            MessageBox.Show("xoá thành công");
            hienthi_dgdsdouong();
            reset_nhap();
        }

        private void btn_nhaplai_Click(object sender, RoutedEventArgs e)
        {
            reset_nhap();
        }

        private void btn_xuatexceldanhsachdouong_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            // Lấy danh sách đồ uống (hoặc từ DataGrid nếu không dùng EF)
            var danhSach = db.DoUongs.ToList();

            using (var package = new ExcelPackage())
            {
                
                var worksheet = package.Workbook.Worksheets.Add("Danh sách đồ uống");

                // Tiêu đề cột
                worksheet.Cells[1, 1].Value = "Mã đồ uống";
                worksheet.Cells[1, 2].Value = "Tên đồ uống";
                worksheet.Cells[1, 3].Value = "Mã loại";
                worksheet.Cells[1, 4].Value = "Giá";
                worksheet.Cells[1, 5].Value = "Hình ảnh";

                // Định dạng tiêu đề
                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                //mở khóa tất cả các ô 
                worksheet.Cells.Style.Locked = false;
                int row = 2;
                foreach (var douong in danhSach)
                {
                    worksheet.Cells[row, 1, row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 1, row, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[row, 1].Value = douong.MaDoUong;
                    worksheet.Cells[row, 2].Value = douong.TenDoUong;
                    worksheet.Cells[row, 3].Value = douong.MaLoai;
                    worksheet.Cells[row, 4].Value = douong.DonGia;

                    // chèn ảnh
                    string imgPath=null;
                    if (!string.IsNullOrEmpty(douong.HinhAnh)) {
                        imgPath = new Uri(douong.HinhAnh).LocalPath;
                    }

                    int imageCol = 5; // ví dụ cột ảnh là E (index = 5)

                    
                    // 2. Set chiều cao dòng và chiều rộng cột ảnh
                    worksheet.Row(row).Height = 60;
                    worksheet.Column(imageCol).Width = 18;

                    if (File.Exists(imgPath))
                    {
                        
                        var fileInfo = new FileInfo(imgPath);
                        var picture = worksheet.Drawings.AddPicture($"img{row}", fileInfo);

                        picture.SetPosition(row - 1, 5, imageCol - 1, 5);
                        picture.SetSize(60, 60);

                        picture.EditAs = eEditAs.OneCell;
                        picture.Locked = true;
                        picture.LockAspectRatio = true;

                        // 3. Lock lại ô chứa ảnh
                        worksheet.Cells[row, imageCol].Style.Locked = true;

                        // 4. Khóa Sheet để áp dụng việc khóa ảnh
                        worksheet.Protection.IsProtected = true;
                      //  worksheet.Protection.SetPassword("123");
                        worksheet.Protection.AllowEditObject = false; // quan trọng để ảnh bị khóa
                    }

                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                // Chọn nơi lưu
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.xlsx";
                saveDialog.FileName = "DanhSachDoUong.xlsx";

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
            manhinhchinh.hienthi_dgdsdouong();
            manhinhchinh.Show();
        }

        private void btn_timtheoten_Click(object sender, object e)
        {
            if (string.IsNullOrEmpty(txt_timtheoten.Text))
            {
                MessageBox.Show("chưa nhập tên đồ uống cần tìm");
                txt_timtheoten.Focus();
                return;
            }
            var kq= db.DoUongs.Where(du=>du.TenDoUong.ToLower().Contains(txt_timtheoten.Text.Trim().ToLower())).ToList();
            if (kq.Count == 0)
            {
                MessageBox.Show("không tìm thấy");
                txt_timtheoten.Focus();
                hienthi_dgdsdouong();
                return;
            }
            dg_dsdouong.ItemsSource = kq;
        }

        private void dg_dsdouong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_dsdouong.SelectedItem != null)
            {
                var du= dg_dsdouong.SelectedItem as DoUong;
                txt_madouong.Text = du.MaDoUong;
                txt_tendouong.Text = du.TenDoUong;
                txt_giadouong.Text=du.DonGia.ToString();
                cbo_loaidouong.SelectedValue = du.MaLoai;
                if (string.IsNullOrEmpty(du.HinhAnh) == false)
                {
                    try
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(du.HinhAnh);

                        bitmap.EndInit();
                        img_anhdouong.Source = bitmap;
                    }
                    catch(Exception ex)
                    {

                    }
                }else img_anhdouong.Source = null;
            }
        }
    }
}
