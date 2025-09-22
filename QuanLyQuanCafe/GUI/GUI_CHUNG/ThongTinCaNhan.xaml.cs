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
using QuanLyQuanCafe.GUI.GUI_ADMIN;
using QuanLyQuanCafe.Models;
using QuanLyQuanCafe.GUI.GUI_NHAN_VIEN;
namespace QuanLyQuanCafe.GUI.GUI_CHUNG
{
    /// <summary>
    /// Interaction logic for ThongTinCaNhan.xaml
    /// </summary>
    public partial class ThongTinCaNhan : Window
    {
        QuanLyQuanCafeContext db=new QuanLyQuanCafeContext();
        TaiKhoan taiKhoan = null;
        ManHinhChinh manhinhchinh = null;
        ManHinhChinh_NV ManHinhChinh_NV = null;
        bool nv_hay_admin=false;
        public ThongTinCaNhan(ManHinhChinh manhinhchinh, TaiKhoan tk)
        {
            InitializeComponent();
            this.manhinhchinh = manhinhchinh;
            this.taiKhoan = tk;   
        }
        public ThongTinCaNhan(ManHinhChinh_NV manhinhchinhnv, TaiKhoan tk)
        {
            InitializeComponent();
            this.ManHinhChinh_NV = manhinhchinhnv;
            this.taiKhoan = tk;
            nv_hay_admin = true;
        }
        private void hienthi_thongtincanhan()
        {
            var thongtintaikhoan = db.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == taiKhoan.MaNhanVien);
            if(thongtintaikhoan != null)
            {
               tb_manhanvien.Text = thongtintaikhoan.MaNhanVien;
                tb_tennhanvien.Text = thongtintaikhoan.TenNhanVien;
                tb_diachinhanvien.Text = thongtintaikhoan.DiaChi;
                tb_ngayvaolam.Text = thongtintaikhoan.NgayVaoLam.Value.ToString("dd/MM/yyyy");
                tb_tuoinhanvien.Text = thongtintaikhoan.Tuoi.ToString();
                tb_sodienthoainhanvien.Text = thongtintaikhoan.SoDienThoai;
                tb_gioitinhnhanvien.Text = thongtintaikhoan.GioiTinh;
                tb_tendangnhap.Text = taiKhoan.TenDangNhap;
            }
        }
        private void quanlythongtincanhan_Loaded(object sender, RoutedEventArgs e)
        {
            hienthi_thongtincanhan();
        }
        private bool check_nhap()
        {
            if(string.IsNullOrEmpty(txt_matkhaucu.Text.Trim()))
            {
                MessageBox.Show("Bạn chưa nhập mật khẩu cũ!");
                txt_matkhaucu.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_matkhaumoi.Text.Trim()))
            {
                MessageBox.Show("Bạn chưa nhập mật khẩu mới!");
               txt_matkhaumoi.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txt_nhaplaimatkhaumoi.Text.Trim()))
            {
                MessageBox.Show("Bạn chưa nhập lại mật khẩu mới!");
                txt_nhaplaimatkhaumoi.Focus();
                return false;
            }
            taiKhoan.MatKhau= taiKhoan.MatKhau.Trim();
            if(txt_matkhaucu.Text != taiKhoan.MatKhau)
            {
                MessageBox.Show("Mật khẩu cũ không đúng!");
                txt_matkhaucu.Focus();
                return false;
            }
            if(txt_matkhaumoi.Text.Trim() != txt_nhaplaimatkhaumoi.Text.Trim())
            {
                MessageBox.Show("Mật khẩu mới không khớp!");
                txt_nhaplaimatkhaumoi.Focus();
                return false;
            }
            return true;
        }
        private void reset_nhap()
        {
            txt_matkhaucu.Text = "";
            txt_matkhaumoi.Text = "";
            txt_nhaplaimatkhaumoi.Text = "";
        }
        private void btn_doimatkhau_Click(object sender, RoutedEventArgs e)
        {
            if(!check_nhap())
            {
                return;
            }
            var tk_moi = db.TaiKhoans.FirstOrDefault(tk => tk.TenDangNhap == taiKhoan.TenDangNhap);
            tk_moi.MatKhau = txt_nhaplaimatkhaumoi.Text.Trim();
            db.SaveChanges();
            MessageBox.Show("Đổi mật khẩu thành công!");
            reset_nhap();
        }

       

        private void btn_thoat_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
            if (nv_hay_admin == false)
            {
                manhinhchinh.Show();    
            }
            else ManHinhChinh_NV.Show();
        }
    }
}
