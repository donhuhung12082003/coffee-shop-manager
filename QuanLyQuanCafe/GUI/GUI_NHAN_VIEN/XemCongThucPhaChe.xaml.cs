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
namespace QuanLyQuanCafe.GUI.GUI_NHAN_VIEN
{
    /// <summary>
    /// Interaction logic for XemCongThucPhaChe.xaml
    /// </summary>
    public partial class XemCongThucPhaChe : Window
    {
        QuanLyQuanCafeContext db= new QuanLyQuanCafeContext();
        ManHinhChinh_NV manhinhchinh_nv = null;
        public XemCongThucPhaChe(ManHinhChinh_NV mhcnv)
        {
            manhinhchinh_nv= mhcnv;
            InitializeComponent();
        }
        private void hienthi_cboDoUong()
        {
            var dsdoUong = db.DoUongs.OrderBy(d => d.TenDoUong).ToList();
            cbo_douong.ItemsSource = dsdoUong;
            cbo_douong.DisplayMemberPath = "TenDoUong";
            cbo_douong.SelectedValuePath = "MaDoUong";
            cbo_douong.SelectedIndex = 0;
        }

        private void xemcongthucphache_Loaded(object sender, RoutedEventArgs e)
        {
            hienthi_cboDoUong();
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
                tb_congthuc.Text += ct.MoTaSoLuongNguyenLieuCanDung + " " + tennguyenLieu + "\n";
            }

        }

        private void btn_thoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            manhinhchinh_nv.Show();
        }
    }
}
