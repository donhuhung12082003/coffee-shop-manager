using Microsoft.EntityFrameworkCore;
using QuanLyQuanCafe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
    /// Interaction logic for QuanLyTang.xaml
    /// </summary>
    public partial class QuanLyTang : Window
    {
        ManHinhChinh manHinhChinh = null;
        QuanLyQuanCafeContext db = new QuanLyQuanCafeContext();
               
        public QuanLyTang(ManHinhChinh mhc)
        {
            InitializeComponent();
            manHinhChinh = mhc;
           
        }
        private void hienthi_dgtang()
        {
            dg_dstang.ItemsSource = db.Tangs.ToList();
        }
        private bool check_nhap()
        {
            if(string.IsNullOrEmpty(txt_ma.Text.Trim()))
            {
                MessageBox.Show("Mã tầng không được để trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_ma.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_ten.Text.Trim()))
            {
                MessageBox.Show("Tên tầng không được để trống!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_ten.Focus();
                return false;
            }
            return true;
        }
        private void quanlytang_Loaded(object sender, RoutedEventArgs e)
        {
            hienthi_dgtang();
        }
        private void reset_nhap()
        {
            txt_ma.Clear();
            txt_ten.Clear();
            txt_timtheoten.Clear();
        }
        private void btn_them_Click(object sender, RoutedEventArgs e)
        {
            if (!check_nhap())
            {
                return;
            }
            var tang_them = db.Tangs.FirstOrDefault(t => t.MaTang == txt_ma.Text.Trim());
            if (tang_them != null)
            {
                MessageBox.Show("Mã tầng đã tồn tại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_ma.Focus();
                return;
            }
            Tang newTang = new Tang
            {
                MaTang = txt_ma.Text.Trim(),
                TenTang = txt_ten.Text.Trim()
            };
            // Tìm entity cũ đang bị track (nếu có)
            var trackedEntity = db.ChangeTracker.Entries<Tang>()
                                  .FirstOrDefault(e => e.Entity.MaTang == newTang.MaTang);

            if (trackedEntity != null)
            {
                db.Entry(trackedEntity.Entity).State = EntityState.Detached;
            }

            db.Tangs.Add(newTang);
            db.SaveChanges();
            MessageBox.Show("Thêm tầng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            hienthi_dgtang();
            reset_nhap();
        }

        private void btn_sua_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap())
            {
                return;
            }
            var tang_sua = db.Tangs.FirstOrDefault(t => t.MaTang == txt_ma.Text.Trim());
            if (tang_sua == null)
            {
                MessageBox.Show("Mã tầng không tồn tại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_ma.Focus();
                return;
            }
            tang_sua.TenTang = txt_ten.Text.Trim();
     
            db.SaveChanges();
            MessageBox.Show("Sửa tầng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            hienthi_dgtang();
            reset_nhap();
           
        }

        private void btn_xoa_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txt_ma.Text.Trim()))
            {
                MessageBox.Show("Vui lòng chọn tầng để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
               txt_ma.Focus();
                return;
            }
            var tang_xoa = db.Tangs.FirstOrDefault(t => t.MaTang == txt_ma.Text.Trim());
            if (tang_xoa == null)
            {
                MessageBox.Show("Mã tầng không tồn tại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_ma.Focus();
                return;
            }
            var ds_ban = db.Bans.Where(b => b.MaTang == tang_xoa.MaTang).ToList();
            if (ds_ban.Any(b => b.TrangThai == true))
            { 
                MessageBox.Show("Tầng này đang có bàn có người ngồi, không thể xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa tầng này?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Kiểm tra xem tầng có bàn nào không
                var ds_ban_tang = db.Bans.Where(b => b.MaTang == tang_xoa.MaTang).ToList();
                db.Bans.RemoveRange(ds_ban_tang); // Xóa tất cả bàn thuộc tầng này
                db.Tangs.Remove(tang_xoa);
                db.SaveChanges();
                MessageBox.Show("Xóa tầng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                hienthi_dgtang();
                reset_nhap();
                
            }
        }

        private void btn_nhaplai_Click(object sender, RoutedEventArgs e)
        {
            reset_nhap();
        }

        private void btn_thoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            manHinhChinh.hienthi_cboTang();
            manHinhChinh.hienthi_paneldsbantheotang();
            manHinhChinh.Show();
        }

        private void btn_timtheoten_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txt_timtheoten.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập tên tầng để tìm kiếm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txt_timtheoten.Focus();
                return;
            }
            var kq = db.Tangs.Where(t => t.TenTang.ToLower().Trim().Contains(txt_timtheoten.Text.ToLower().Trim())).ToList();
            if (kq.Count == 0)
            {
                MessageBox.Show("Không tìm thấy tầng nào với tên này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                hienthi_dgtang();
                return;
            }
            dg_dstang.ItemsSource = kq;
        }

        private void dg_dstang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dg_dstang.SelectedItem != null)
            {
                Tang selectedTang = dg_dstang.SelectedItem as Tang;
                txt_ma.Text = selectedTang.MaTang;
                txt_ten.Text = selectedTang.TenTang;
            }
        }
    }
}
