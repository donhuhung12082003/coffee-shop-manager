//thư viện để làm việc với file word
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
//thư viện mở file explorer
using Microsoft.Win32;
using QuanLyQuanCafe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace QuanLyQuanCafe.GUI.GUI_ADMIN
{
    /// <summary>
    /// Interaction logic for QuanLyCongThucPhaChe.xaml
    /// </summary>
    public partial class QuanLyCongThucPhaChe : Window
    {
        QuanLyQuanCafeContext db = new QuanLyQuanCafeContext();
        ManHinhChinh ManHinhChinh = null;
        public QuanLyCongThucPhaChe(ManHinhChinh manHinhChinh)
        {
            InitializeComponent();

            ManHinhChinh = manHinhChinh;
        }
        private void hienthi_cboDoUong()
        {
            var dsdoUong = db.DoUongs.OrderBy(d => d.TenDoUong).ToList();
            cbo_douong.ItemsSource = dsdoUong;
            cbo_douong.DisplayMemberPath = "TenDoUong";
            cbo_douong.SelectedValuePath = "MaDoUong";
            cbo_douong.SelectedIndex = 0;
        }
        private void hienthi_cboNguyenLieu()
        {
            var dsnguyenlieu = db.NguyenLieus.OrderBy(n => n.TenNguyenLieu).ToList();
            cbo_nguyenlieu.ItemsSource = dsnguyenlieu;
            cbo_nguyenlieu.DisplayMemberPath = "TenNguyenLieu";
            cbo_nguyenlieu.SelectedValuePath = "MaNguyenLieu";
            cbo_nguyenlieu.SelectedIndex = 0;
        }
        private void hienthi_cbodonvitinh()
        {
            List<string> dsDonViTinh = new List<string>
            {
                "g", "ml"
            };
            cbo_donvitinh.ItemsSource = dsDonViTinh;
            cbo_donvitinh.SelectedIndex = 0;
        }
        private void hienthi_dgdscongthuc()
        {
            dg_dscongthuc.ItemsSource = db.CongThucPhaChes.ToList();
        }
        private void hienthi_cbotimdouongtheoten()
        {
            var dsdoUong = db.DoUongs.OrderBy(d => d.TenDoUong).ToList();
            cbo_timdouongtheoten.ItemsSource = dsdoUong;
            cbo_timdouongtheoten.DisplayMemberPath = "TenDoUong";
            cbo_timdouongtheoten.SelectedValuePath = "MaDoUong";
            cbo_timdouongtheoten.SelectedIndex = 0;
        }
        private void quanlycongthucphache_Loaded(object sender, RoutedEventArgs e)
        {
            hienthi_cbodonvitinh();
            hienthi_cboDoUong();
            hienthi_cboNguyenLieu();
            hienthi_dgdscongthuc();
            hienthi_cbotimdouongtheoten();
        }
        private bool check_nhap()
        {
            if (string.IsNullOrEmpty(txt_macongthuc.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập mã công thức pha chế", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_macongthuc.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_soluongnguyenlieu.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập số lượng nguyên liệu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_soluongnguyenlieu.Focus();
                return false;
            }
            if (!int.TryParse(txt_soluongnguyenlieu.Text.Trim(), out int soluong))
            {
                MessageBox.Show("Số lượng nguyên liệu phải là số nguyên", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_soluongnguyenlieu.Focus();
                return false;
            }
            if (soluong <= 0)
            {
                MessageBox.Show("Số lượng nguyên liệu phải lớn hơn 0", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_soluongnguyenlieu.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(cbo_douong.Text.Trim()))
            {
                MessageBox.Show("Vui lòng chọn đồ uống", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbo_douong.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(cbo_nguyenlieu.Text.Trim()))
            {
                MessageBox.Show("Vui lòng chọn nguyên liệu", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbo_nguyenlieu.Focus();
                return false;
            }

            if (cbo_douong.SelectedIndex == -1)
            {
                MessageBox.Show("Đồ uống không tồn tại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbo_douong.Focus();
                return false;
            }
            if (cbo_nguyenlieu.SelectedIndex == -1)
            {
                MessageBox.Show("Nguyên liệu không tồn tại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbo_nguyenlieu.Focus();
                return false;
            }
            return true;
        }
        private void reset_nhap()
        {
            txt_macongthuc.Text = "";
            txt_soluongnguyenlieu.Text = "";
            cbo_donvitinh.SelectedIndex = 0;
            cbo_douong.SelectedIndex = 0;
            cbo_nguyenlieu.SelectedIndex = 0;
        }
        private void btn_them_Click(object sender, RoutedEventArgs e)
        {
            if (!check_nhap())
            {

                return;
            }
            CongThucPhaChe congThuc = new CongThucPhaChe
            {
                MaCongThuc = txt_macongthuc.Text.Trim(),
                MaDoUong = cbo_douong.SelectedValue.ToString(),
                MaNguyenLieu = cbo_nguyenlieu.SelectedValue.ToString(),
                MoTaSoLuongNguyenLieuCanDung = txt_soluongnguyenlieu.Text.Trim() + " " + cbo_donvitinh.Text.Trim()
            };
            var existingCT = db.CongThucPhaChes.FirstOrDefault(ct => ct.MaCongThuc == txt_macongthuc.Text.Trim());
            var check_ctpc_tontai = db.CongThucPhaChes.FirstOrDefault(ct => ct.MaDoUong == congThuc.MaDoUong && ct.MaNguyenLieu == congThuc.MaNguyenLieu);

            if (existingCT != null || check_ctpc_tontai!=null)
            {
                MessageBox.Show("Công thức pha chế đã tồn tại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Tìm entity cũ đang bị track (nếu có)
            var trackedEntity = db.ChangeTracker.Entries<CongThucPhaChe>()
                                  .FirstOrDefault(e => e.Entity.MaCongThuc == congThuc.MaCongThuc);

            if (trackedEntity != null)
            {
                db.Entry(trackedEntity.Entity).State = EntityState.Detached;
            }

            db.CongThucPhaChes.Add(congThuc);
            db.SaveChanges();
            MessageBox.Show("Thêm công thức pha chế thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            hienthi_dgdscongthuc();
            reset_nhap();
        }

        private void btn_sua_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap())
            {
                return;
            }
            var congThucSua = db.CongThucPhaChes.FirstOrDefault(ct => ct.MaCongThuc == txt_macongthuc.Text.Trim());
            if (congThucSua == null)
            {
                MessageBox.Show("Công thức pha chế không tồn tại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_macongthuc.Focus();
                return;
            }
            congThucSua.MaDoUong = cbo_douong.SelectedValue.ToString();
            congThucSua.MaNguyenLieu = cbo_nguyenlieu.SelectedValue.ToString();
            congThucSua.MoTaSoLuongNguyenLieuCanDung = txt_soluongnguyenlieu.Text.Trim() + " " + cbo_donvitinh.Text.Trim();
            db.SaveChanges();
            MessageBox.Show("Sửa công thức pha chế thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            hienthi_dgdscongthuc();
            reset_nhap();
        }

        private void btn_xoa_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txt_macongthuc.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập mã công thức pha chế cần xóa", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_macongthuc.Focus();
                return;
            }
            var congThucXoa = db.CongThucPhaChes.FirstOrDefault(ct => ct.MaCongThuc == txt_macongthuc.Text.Trim());
            if (congThucXoa == null)
            {
                MessageBox.Show("Công thức pha chế không tồn tại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_macongthuc.Focus();
                return;
            }
            if (MessageBox.Show("Xác nhận xóa", "xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            db.CongThucPhaChes.Remove(congThucXoa);
            db.SaveChanges();
            MessageBox.Show("Xóa công thức pha chế thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            hienthi_dgdscongthuc();
            reset_nhap();
        }

        private void btn_nhaplai_Click(object sender, RoutedEventArgs e)
        {
            reset_nhap();
        }

        private void btn_thoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ManHinhChinh.Show();
        }

        private void btn_timtheoten_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(cbo_timdouongtheoten.Text.Trim()))
            {
                MessageBox.Show("Vui lòng chọn đồ uống để tìm công thức pha chế", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbo_timdouongtheoten.Focus();
                return;
            }
            if(cbo_timdouongtheoten.SelectedIndex == -1)
            {
                MessageBox.Show("Đồ uống không tồn tại", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbo_timdouongtheoten.Focus();
                hienthi_dgdscongthuc();
                reset_nhap();
                return;
            }
            var selectedDoUong = cbo_timdouongtheoten.SelectedValue.ToString();
            var dsCongThucTim = db.CongThucPhaChes
                .Where(ct => ct.MaDoUong == selectedDoUong)
                .ToList();
            if (dsCongThucTim.Count == 0)
            {
                MessageBox.Show("Không tìm thấy công thức pha chế cho đồ uống này", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                hienthi_dgdscongthuc();
                reset_nhap();
            }
            else
            {
                dg_dscongthuc.ItemsSource = dsCongThucTim;
            }
        }

        private void btn_xemcongthuc_Click(object sender, RoutedEventArgs e)
        {
            if (cbo_douong.SelectedIndex == -1)
            {
                MessageBox.Show("đồ uống không tồn tại");
                cbo_douong.Focus();
                return;
            }
            var dsnguyenLieu = db.CongThucPhaChes
                .Where(ct => ct.MaDoUong == cbo_douong.SelectedValue.ToString())
                .ToList();
            if (dsnguyenLieu.Count == 0)
            {
                MessageBox.Show("Không tìm thấy công thức pha chế cho đồ uống này", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            tb_congthuc.Text = "Công thức pha chế " + cbo_douong.Text + "\nDùng:\n";
            foreach (var ct in dsnguyenLieu)
            {
                var tennguyenLieu = db.NguyenLieus.FirstOrDefault(nl => nl.MaNguyenLieu == ct.MaNguyenLieu).TenNguyenLieu;
                tb_congthuc.Text += ct.MoTaSoLuongNguyenLieuCanDung + " "+tennguyenLieu+"\n";
            }
            
        }

        private void dg_dscongthuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_dscongthuc.SelectedItem != null)
            {
                CongThucPhaChe congThuc = dg_dscongthuc.SelectedItem as CongThucPhaChe;
                if (congThuc != null)
                {
                    txt_macongthuc.Text = congThuc.MaCongThuc;
                    cbo_douong.SelectedValue = congThuc.MaDoUong;
                    cbo_nguyenlieu.SelectedValue = congThuc.MaNguyenLieu;
                    txt_soluongnguyenlieu.Text = congThuc.MoTaSoLuongNguyenLieuCanDung.Split(' ')[0]; // Lấy phần số lượng
                    cbo_donvitinh.Text = congThuc.MoTaSoLuongNguyenLieuCanDung.Split(' ')[1]; // Lấy phần đơn vị tính
                }
            }
        }

        private void btn_xuatword_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Word Document (*.docx)|*.docx",
                Title = "Chọn nơi lưu file Word",
                FileName = "DanhSachCongThucPhaChe.docx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                string filePath = saveDialog.FileName;

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(filePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = new Body();

                    // Tiêu đề
                    body.Append(
                        new Paragraph(
                            new Run(
                                new Text("🍹 DANH SÁCH CÔNG THỨC PHA CHẾ")
                            )
                        )
                    );
                    var dsdouong = db.DoUongs.ToList();
                    var dscongthuc = db.CongThucPhaChes.ToList();
                    var dsnguyenlieu = db.NguyenLieus.ToList();
                    // Duyệt từng đồ uống
                    foreach (var douong in dsdouong)
                    {
                        var congThucList = dscongthuc
                            .Where(ct => ct.MaDoUong == douong.MaDoUong)
                            .ToList();

                        if (congThucList.Count > 0)
                        {
                            // Tên đồ uống
                            body.Append(new Paragraph(new Run(new Text($"\nCông thức pha chế cho đồ uống: {douong.TenDoUong}"))));
                            body.Append(new Paragraph(new Run(new Text("\tDùng:"))));

                            foreach (var ct in congThucList)
                            {
                                var tenNguyenLieu = dsnguyenlieu.FirstOrDefault(nl => nl.MaNguyenLieu == ct.MaNguyenLieu)?.TenNguyenLieu;
                                body.Append(new Paragraph(new Run(new Text($"{ct.MoTaSoLuongNguyenLieuCanDung} {tenNguyenLieu}"))));
                            }

                            body.Append(new Paragraph(new Run(new Text("")))); // khoảng trắng
                        }
                        else
                        {
                            body.Append(new Paragraph(new Run(new Text($"\nCông thức pha chế cho đồ uống: {douong.TenDoUong} đang cập nhật\n"))));
                        }
                    }

                    mainPart.Document.Append(body);
                    mainPart.Document.Save();
                }

                MessageBox.Show("✅ Xuất file Word thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
