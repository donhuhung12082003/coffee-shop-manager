using Microsoft.EntityFrameworkCore;
using QuanLyQuanCafe.Class_hien_thi_datagridview;
using QuanLyQuanCafe.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace QuanLyQuanCafe.GUI.GUI_ADMIN
{
    /// <summary>
    /// Interaction logic for QuanLyTaiKhoan.xaml
    /// </summary>
    public partial class QuanLyTaiKhoan : Window
    {
        ManHinhChinh manhinhchinh;
        QuanLyQuanCafeContext db = new QuanLyQuanCafeContext();
        public QuanLyTaiKhoan(ManHinhChinh manhinhchinh)
        {
            InitializeComponent();
            this.manhinhchinh = manhinhchinh;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dg_dstaikhoan.SelectedItem != null)
            {
                DG_TaiKhoan tk = dg_dstaikhoan.SelectedItem as DG_TaiKhoan;
                txt_mataikhoan.Text = tk.MaTaiKhoan;
                txt_tendangnhap.Text=tk.TenDangNhap;
                txt_matkhau.Text = tk.MatKhau;
                txt_manhanviensohuutaikhoan.Text = tk.MaNhanVien;
                cbo_quyen.Text = tk.Quyen == "Admin" ? "Admin" : "Nhân viên";

            }
        }

        private void btn_themtaikhoan_Click(object sender, RoutedEventArgs e)
        {
          
                if (!check_nhap()) return;
                var tk_them = db.TaiKhoans.FirstOrDefault(tk => tk.MaTaiKhoan == txt_mataikhoan.Text.Trim());
                if (tk_them != null)
                {
                    MessageBox.Show("Mã tài khoản này đã tồn tại, vui lòng chọn mã khác");
                    txt_mataikhoan.Focus();
                    return;
                }
                var tk_moi = new TaiKhoan();
                tk_moi.MaTaiKhoan = txt_mataikhoan.Text.Trim();
                tk_moi.TenDangNhap = txt_tendangnhap.Text.Trim();
                tk_moi.MatKhau = txt_matkhau.Text.Trim();
                tk_moi.MaNhanVien = string.IsNullOrEmpty(txt_manhanviensohuutaikhoan.Text.Trim())?null: txt_manhanviensohuutaikhoan.Text.Trim();
                string quyen = cbo_quyen.Text;
                tk_moi.Quyen = quyen == "Nhân viên" ? true : false;

            // Tìm entity cũ đang bị track (nếu có)
            var trackedEntity = db.ChangeTracker.Entries<TaiKhoan>()
                                  .FirstOrDefault(e => e.Entity.MaTaiKhoan == tk_moi.MaTaiKhoan);

            if (trackedEntity != null)
            {
                db.Entry(trackedEntity.Entity).State = EntityState.Detached;
            }

            db.TaiKhoans.Add(tk_moi);
                db.SaveChanges();
                hienthi_dgtaikhoan();
                MessageBox.Show("thêm tài khoản thành công");
                reset_nhap();
            
          
        }
        private void reset_nhap()
        {
            txt_tendangnhap.Text = "";
            txt_matkhau.Text = "";
            txt_manhanviensohuutaikhoan.Text = "";
            txt_timtaikhoantheoten.Text = "";
            txt_mataikhoan.Text = "";
        }
        private void hienthi_cboquyen()
        {
            cbo_quyen.Items.Add("Admin");
            cbo_quyen.Items.Add("Nhân viên");
            cbo_quyen.SelectedIndex = 0;
            
        }
        private List<DG_TaiKhoan> layduLieu_dgdstaikhoan()
        {
            List<DG_TaiKhoan> dstk = db.TaiKhoans.Select(tk => new DG_TaiKhoan
            {
                MaTaiKhoan = tk.MaTaiKhoan,
                TenDangNhap = tk.TenDangNhap,
                MatKhau = tk.MatKhau,
                Quyen = tk.Quyen == false ? "Admin" : "Nhân viên",
                MaNhanVien = tk.MaNhanVien,
                TenNhanVien = string.IsNullOrEmpty(tk.MaNhanVien) ? "" : db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == tk.MaNhanVien).TenNhanVien
            }).ToList<DG_TaiKhoan>();
            return dstk;
        }
        public void hienthi_dgtaikhoan()
        { 
          dg_dstaikhoan.ItemsSource = layduLieu_dgdstaikhoan();
        }
        private void quanlytaikhoan_Loaded(object sender, RoutedEventArgs e)
        {
            hienthi_cboquyen();
            hienthi_dgtaikhoan();
        }
        public bool check_nhap()
        {
            if (string.IsNullOrEmpty(txt_mataikhoan.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập mã tài khoản");
                txt_mataikhoan.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_tendangnhap.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập");
                txt_tendangnhap.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_matkhau.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu");
                txt_matkhau.Focus();
                return false;
            }
            var tk_them=db.TaiKhoans.FirstOrDefault(tk => tk.MaTaiKhoan == txt_mataikhoan.Text.Trim());
            if (tk_them != null)
            {
                MessageBox.Show("Mã tài khoản này đã tồn tại, vui lòng chọn mã khác");
                txt_mataikhoan.Focus();
                return false;
            }
            tk_them = db.TaiKhoans.FirstOrDefault(tk => tk.TenDangNhap == txt_tendangnhap.Text.Trim());
            if (tk_them != null)
            {
                MessageBox.Show("Tên đăng nhập này đã tồn tại, vui lòng chọn tên khác");
                txt_tendangnhap.Focus();
                return false;
            }

            if (!string.IsNullOrEmpty(txt_manhanviensohuutaikhoan.Text.Trim()))
            {
                var check_nv_co_trong_bang_nv = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == txt_manhanviensohuutaikhoan.Text.Trim());
                if (check_nv_co_trong_bang_nv == null)
                {
                    MessageBox.Show("Mã nhân viên này không tồn tại, vui lòng kiểm tra lại");
                    txt_manhanviensohuutaikhoan.Focus();
                    return false;
                }
                var check_nv_co_2_tk = db.TaiKhoans.FirstOrDefault(tk => tk.MaNhanVien == txt_manhanviensohuutaikhoan.Text.Trim());
                if (check_nv_co_2_tk != null)
                {
                    MessageBox.Show("Mã nhân viên này đã có tài khoản, vui lòng chọn mã khác");
                    txt_manhanviensohuutaikhoan.Focus();
                    return false;
                }
            }
            return true;
        }

        private void btn_suataikhoan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_mataikhoan.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập mã tài khoản");
                txt_mataikhoan.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txt_tendangnhap.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập");
                txt_tendangnhap.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txt_matkhau.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu");
                txt_matkhau.Focus();
                return;
            }
            var tk_sua = db.TaiKhoans.FirstOrDefault(tk => tk.MaTaiKhoan == txt_mataikhoan.Text.Trim());
            if (tk_sua == null)
            {
                MessageBox.Show("Mã tài khoản này không tồn tại, vui lòng kiểm tra lại");
                txt_mataikhoan.Focus();
                return;
            }
            var tk_them = db.TaiKhoans.FirstOrDefault(tk => (tk.TenDangNhap == txt_tendangnhap.Text.Trim()) &&(tk.MaTaiKhoan!=txt_mataikhoan.Text.Trim()));
            if (tk_them != null)
            {
                MessageBox.Show("Tên đăng nhập này đã tồn tại, vui lòng chọn tên khác");
                txt_tendangnhap.Focus();
                return;
            }

            if (!string.IsNullOrEmpty(txt_manhanviensohuutaikhoan.Text.Trim()))
            {
                var check_nv_co_trong_bang_nv = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == txt_manhanviensohuutaikhoan.Text.Trim());
                if (check_nv_co_trong_bang_nv == null)
                {
                    MessageBox.Show("Mã nhân viên này không tồn tại, vui lòng kiểm tra lại");
                    txt_manhanviensohuutaikhoan.Focus();
                    return;
                }
                var check_nv_co_2_tk = db.TaiKhoans.FirstOrDefault(tk => (tk.MaNhanVien == txt_manhanviensohuutaikhoan.Text.Trim()) && (tk.MaTaiKhoan!=txt_mataikhoan.Text.Trim()));
                if (check_nv_co_2_tk != null)
                {
                    MessageBox.Show("Mã nhân viên này đã có tài khoản, vui lòng chọn mã khác");
                    txt_manhanviensohuutaikhoan.Focus();
                    return;
                }
                tk_sua.TenDangNhap = txt_tendangnhap.Text.Trim();
                tk_sua.MatKhau = txt_matkhau.Text.Trim();
                tk_sua.MaNhanVien = txt_manhanviensohuutaikhoan.Text.Trim();
                string quyen = cbo_quyen.Text;
                tk_sua.Quyen = quyen == "Nhân viên" ? true : false;
                db.SaveChanges();
                hienthi_dgtaikhoan();
                MessageBox.Show("Sửa tài khoản thành công");
                reset_nhap();
            }
        }
        private void btn_xoataikhoan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_mataikhoan.Text.Trim()))
            {
                MessageBox.Show("chưa nhập mã tài khoản");
                txt_mataikhoan.Focus();
                return;
            }
            var tk_xoa = db.TaiKhoans.FirstOrDefault(tk => tk.MaTaiKhoan == txt_mataikhoan.Text.Trim());
            if( tk_xoa ==null )
            {
                MessageBox.Show("mã tài khoản không tồn tại");
                txt_mataikhoan.Focus() ; return;
            }
            if(MessageBox.Show("Bạn có chắc chắn muốn xóa tài khoản này không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }
            db.TaiKhoans.Remove(tk_xoa); 
            var nv_xoa=db.NhanViens.FirstOrDefault(nv=>nv.MaNhanVien==tk_xoa .MaNhanVien);
            if(nv_xoa != null)
            {
                db.NhanViens.Remove(nv_xoa);
            }
            MessageBox.Show("Xóa tài khoản thành công");
            db.SaveChanges();
            reset_nhap();
            hienthi_dgtaikhoan();
        }

        private void btn_nhaplai_Click(object sender, RoutedEventArgs e)
        {
            reset_nhap();
        }

        private void btn_thoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            manhinhchinh.Show();
        }

        private void btn_timtaikhoantheoten_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_timtaikhoantheoten.Text.Trim()))
            {
                MessageBox.Show("chưa nhập tên cần tìm");
                txt_timtaikhoantheoten.Focus();
                return;
            }
            List<DG_TaiKhoan> dstk = layduLieu_dgdstaikhoan();
            var kq= dstk.Where(tk => tk.TenNhanVien.ToLower().Contains(txt_timtaikhoantheoten.Text.Trim().ToLower())).ToList();
            if (kq.Count == 0)
            {
                MessageBox.Show("Không tìm thấy tài khoản nhân viên này");
                txt_timtaikhoantheoten.Focus();
                hienthi_dgtaikhoan();
                return;
            }
            dg_dstaikhoan.ItemsSource = kq;
        }

        private void txt_tendangnhap_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}
