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
using QuanLyQuanCafe.GUI.GUI_ADMIN;
using QuanLyQuanCafe.GUI.GUI_NHAN_VIEN;
namespace QuanLyQuanCafe.GUI.GUI_CHUNG
{
    /// <summary>
    /// Interaction logic for DangNhap.xaml
    /// </summary>
    public partial class DangNhap : Window
    {
        QuanLyQuanCafeContext db = new QuanLyQuanCafeContext();
        public DangNhap()
        {
            InitializeComponent();
        }
        private bool check_nhap()
        {
            if (string.IsNullOrEmpty(txt_tendangnhap.Text))
            {
                MessageBox.Show("Vui lòng nhập tài khoản");
                return false;
            }
            if (string.IsNullOrEmpty(txt_matkhau.Password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu");
                return false;
            }
            return true;
        }
        private void btn_dangnhap_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap())
            {
                return;
            }
            string tendangnhap = txt_tendangnhap.Text;
            string matkhau = txt_matkhau.Password;
            var user = db.TaiKhoans.FirstOrDefault(tk => tk.TenDangNhap == tendangnhap && tk.MatKhau == matkhau);
            if (user != null)
            {
                if (user.Quyen == false)
                {
                   ManHinhChinh manHinhChinh = new ManHinhChinh(user);
                    manHinhChinh.Show();
                    this.Close();
                }
                else
                {
                   ManHinhChinh_NV manhinhchinh_nv= new ManHinhChinh_NV(user);
                    manhinhchinh_nv.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Tài khoản hoặc mật khẩu không đúng.");
            }
        }

        private void btn_thoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();   
        }

        private void txt_tendangnhap_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
