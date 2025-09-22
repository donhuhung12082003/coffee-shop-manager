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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using QuanLyQuanCafe.Models;
namespace QuanLyQuanCafe.GUI.GUI_ADMIN
{
    /// <summary>
    /// Interaction logic for QuanLyBan.xaml
    /// </summary>
    public partial class QuanLyBan : Window
    {
        QuanLyQuanCafeContext db = new QuanLyQuanCafeContext();
        ManHinhChinh manHinhChinh = null;
        public QuanLyBan( ManHinhChinh manHinhChinh)
        {
            InitializeComponent();
            this.manHinhChinh = manHinhChinh;
        }
        private void hienthi_cboTang()
        {
            var tangs = db.Tangs.OrderBy(t => t.TenTang);
            cbo_tang.ItemsSource = tangs.ToList();
            cbo_tang.DisplayMemberPath = "TenTang";
            cbo_tang.SelectedValuePath = "MaTang";
            cbo_tang.SelectedIndex = 0;
        }
        private void hienthi_cbotrangthaiban()
        {
            var trangthaiban = new List<string> { "Trống", "Có người" };
            cbo_trangthaiban.ItemsSource = trangthaiban;
            cbo_trangthaiban.SelectedIndex = 0;
        }
        private void hienthi_dgdsban()
        {
            dg_dsban.ItemsSource = db.Bans.ToList();    
        }
        private void quanlyban_Loaded(object sender, RoutedEventArgs e)
        {
            hienthi_cboTang();
            hienthi_cbotrangthaiban();
            hienthi_dgdsban();
        }

        private void dg_dsban_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dg_dsban.SelectedItem != null)
            {
                Ban selectedBan = dg_dsban.SelectedItem as Ban;
                if (selectedBan != null)
                {
                    txt_maban.Text = selectedBan.MaBan;
                    txt_tenban.Text = selectedBan.TenBan;
                    cbo_tang.SelectedValue = selectedBan.MaTang;
                    cbo_trangthaiban.SelectedIndex = selectedBan.TrangThai == false ? 0 : 1;
                }
            }
        }
        private bool check_nhap()
        {
            if(string.IsNullOrEmpty(txt_maban.Text.Trim()))
            {
                MessageBox.Show("Mã bàn không được để trống!");
                txt_maban.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_tenban.Text.Trim()))
            {
                MessageBox.Show("Tên bàn không được để trống!");
                txt_tenban.Focus();
                return false;
            }
            return true;    

        }
        private void reset_nhap()
        {
            txt_maban.Text = string.Empty;
            txt_tenban.Text = string.Empty;
            cbo_tang.SelectedIndex = 0;
            cbo_trangthaiban.SelectedIndex = 0;
            txt_timtheoten.Text = string.Empty;
        }
        private void btn_them_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap())
            {
                return;
            }
            var ban_them=db.Bans.FirstOrDefault(b => b.MaBan == txt_maban.Text.Trim());
            if (ban_them != null)
            {
                MessageBox.Show("Mã bàn đã tồn tại!");
                txt_maban.Focus();
                return;
            }
            Ban newBan = new Ban
            {
                MaBan = txt_maban.Text.Trim(),
                TenBan = txt_tenban.Text.Trim(),
                MaTang = cbo_tang.SelectedValue.ToString(),
                TrangThai = cbo_trangthaiban.SelectedIndex == 0 ? false : true
            };

            // Tìm entity cũ đang bị track (nếu có)
            var trackedEntity = db.ChangeTracker.Entries<Ban>()
                                  .FirstOrDefault(e => e.Entity.MaBan == newBan.MaBan);

            if (trackedEntity != null)
            {
                db.Entry(trackedEntity.Entity).State = EntityState.Detached;
            }

            db.Bans.Add(newBan);
            db.SaveChanges();
            MessageBox.Show("Thêm bàn thành công!");
            hienthi_dgdsban();
            reset_nhap();
            manHinhChinh.cap_nhat_dsBan_khi_them_ban(newBan);
        }

        private void btn_sua_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap())
            {
                return;
            }
            var ban_sua = db.Bans.FirstOrDefault(b => b.MaBan == txt_maban.Text.Trim());
            if (ban_sua == null)
            {
                MessageBox.Show("Mã bàn không tồn tại!");
                txt_maban.Focus();
                return;
            }
            ban_sua.TenBan = txt_tenban.Text.Trim();
            ban_sua.MaTang = cbo_tang.SelectedValue.ToString();
            ban_sua.TrangThai = cbo_trangthaiban.SelectedIndex == 0 ? false : true;
            db.SaveChanges();
            MessageBox.Show("Sửa bàn thành công!");
            hienthi_dgdsban();
            reset_nhap();
            if (manHinhChinh.bandangchon != null)
            {
                if (ban_sua.MaBan == manHinhChinh.bandangchon.MaBan && ban_sua.TrangThai == false)
                {
                    manHinhChinh.bandangchon = null;
                    manHinhChinh.hienthi_tbbandangchon();
                    manHinhChinh.clear_dsdouong_ban_chon_nham(ban_sua);
                }
            }
        }

        private void btn_xoa_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_maban.Text.Trim()))
            { 
                MessageBox.Show("Vui lòng chọn bàn để xóa!");
                txt_maban.Focus();
                return;
            }
            var ban_xoa = db.Bans.FirstOrDefault(b => b.MaBan == txt_maban.Text.Trim());
            if (ban_xoa == null)
            {
                MessageBox.Show("Mã bàn không tồn tại!");
                txt_maban.Focus();
                return;
            }
            if(ban_xoa.TrangThai == true)
            {
                MessageBox.Show("Bàn này đang có người ngồi, không thể xóa!");
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa bàn này?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                db.Bans.Remove(ban_xoa);
                db.SaveChanges();
                MessageBox.Show("Xóa bàn thành công!");
                hienthi_dgdsban();
                reset_nhap();
                manHinhChinh.cap_nhat_dsBan_khi_xoa_ban(ban_xoa);   
            }
        }

        private void btn_nhaplai_Click(object sender, RoutedEventArgs e)
        {
            reset_nhap();
        }

        private void btn_thoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            manHinhChinh.hienthi_paneldsbantheotang();
            manHinhChinh.Show();
        }

        private void btn_timtheoten_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_timtheoten.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập tên bàn để tìm kiếm!");
                txt_tenban.Focus();
                return;
            }
            var kq = db.Bans.Where(b => b.TenBan.ToLower().Trim().Contains(txt_timtheoten.Text.ToLower().Trim())).ToList();
            if (kq.Count == 0)
            {
                MessageBox.Show("Không tìm thấy bàn nào với tên này!");
                hienthi_dgdsban();
                reset_nhap();
            }
            else
            {
                dg_dsban.ItemsSource = kq;
            }
        }
    }
}
