using QuanLyQuanCafe.GUI.GUI_CHUNG;
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
using QuanLyQuanCafe.GUI.GUI_CHUNG;
using DocumentFormat.OpenXml.Math;
using System.Windows.Media.Animation;
using QuanLyQuanCafe.Class_hien_thi_datagridview;
using Microsoft.Identity.Client;
//thư viện tạo và in hóa đơn
using Stimulsoft.Report;
namespace QuanLyQuanCafe.GUI.GUI_ADMIN
{
    /// <summary>
    /// Interaction logic for ManHinhChinh.xaml
    /// </summary>
    public partial class ManHinhChinh : Window
    {
        QuanLyQuanCafeContext db = new QuanLyQuanCafeContext();
        TaiKhoan taiKhoan = null;
        public  Ban bandangchon = null;
        public List<DSDoUong_Ban> dsBan = new List<DSDoUong_Ban>();
        public ManHinhChinh(TaiKhoan tk)
        {
            InitializeComponent();
            taiKhoan = tk;
            khoi_tao_dsBan();
        }
        public void clear_dsdouong_ban_chon_nham(Ban ban)
        {
            var ban_chon_nham = dsBan.FirstOrDefault(b => b.ban.MaBan == ban.MaBan);
            if(ban_chon_nham == null) { return; }
            ban_chon_nham.dsDoUong.Clear();
            dg_dsdouonggoi.ItemsSource=null;
        }
        public void cap_nhat_dsBan_khi_them_ban(Ban ban)
        {
            DSDoUong_Ban dSDoUong_Ban = new DSDoUong_Ban();
            dSDoUong_Ban.ban = ban;
            dsBan.Add(dSDoUong_Ban);
        }
        public void cap_nhat_dsBan_khi_xoa_ban(Ban ban)
        {
            foreach (var b in dsBan)
            {
                if (b.ban.MaBan == ban.MaBan)
                {
                    dsBan.Remove(b);
                    return;
                }
            }
        }
        public void khoi_tao_dsBan()
        {
          
            foreach(var ban in db.Bans)
            {
                DSDoUong_Ban dSDoUong_Ban = new DSDoUong_Ban();
                dSDoUong_Ban.ban=ban;
                dsBan.Add(dSDoUong_Ban);
            }
        }
        public void hienthi_cboTang()
        {
            QuanLyQuanCafeContext db1 = new QuanLyQuanCafeContext();
            var tangs = db1.Tangs.OrderBy(t=>t.TenTang);
            cbo_tang.ItemsSource = tangs.ToList();
            cbo_tang.DisplayMemberPath = "TenTang";
            cbo_tang.SelectedValuePath = "MaTang";
            cbo_tang.SelectedIndex = 0;
        }
        public void hienthi_paneldsbantheotang()
        {
            QuanLyQuanCafeContext db1 = new QuanLyQuanCafeContext();
            var tang = cbo_tang.SelectedValue.ToString();
            var dsban = db1.Bans.Where(b => b.MaTang == tang).OrderBy(b=>b.TenBan).ToList();
            panel_dsbantungtang.Children.Clear();
            foreach (var ban in dsban)
            {
                Button btn = new Button
                {
                    Content = ban.TenBan,
                    Tag = ban.MaBan,
                    Width = 100,
                    Height = 100,
                    Margin = new Thickness(5),
                    Background = ban.TrangThai==true ? Brushes.LightGreen : Brushes.LightGray, // Màu nền bàn dựa trên trạng thái
                }
            ;
                btn.Click += sukienchonban;
                panel_dsbantungtang.Children.Add(btn);
            }
        }
        private void sukienchonban(object sender, RoutedEventArgs e)
        {
            
            QuanLyQuanCafeContext db1 = new QuanLyQuanCafeContext();
            Button btn = sender as Button;
            btn.Background = Brushes.LightGreen; // Đổi màu nền của nút khi được chọn
            string maBan = btn.Tag.ToString();
            var ban = db1.Bans.FirstOrDefault(b => b.MaBan == maBan);
            if (ban != null)
            {
                bandangchon = ban;
                ban.TrangThai = true; // Đánh dấu bàn đã được chọn  
                db1.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                hienthi_tbbandangchon();
                hienthi_dsdouongcuabandangduocchon();
                var bandanggoido = dsBan.FirstOrDefault(b => b.ban.MaBan.Trim() == bandangchon.MaBan.Trim());
                tb_tongtienthanhtoan.Text = bandanggoido.dsDoUong.Sum(du => du.SoLuong * du.Gia).ToString("N0");
            }
        }
        private void hienthi_dsdouongcuabandangduocchon()
        {
            var bandangduocchon = dsBan.FirstOrDefault(b => b.ban.MaBan.Trim() == bandangchon.MaBan.Trim());
            dg_dsdouonggoi.ItemsSource = bandangduocchon.dsDoUong.ToList();
        }
        public void hienthi_tbbandangchon()
        {
            if (bandangchon != null)
            {
                tb_bandangchon.Text = bandangchon.TenBan + " Tầng " + cbo_tang.Text;
            }
            else tb_bandangchon.Text = "Chưa chọn bàn";
        }
        public void hienthi_thongtinhoadon()
        {
            QuanLyQuanCafeContext db1 = new QuanLyQuanCafeContext();
            tb_ngaylaphoadon.Text = DateTime.Now.ToString("dd/MM/yyyy");
            tb_nhanvienlaphoadon.Text = db1.NhanViens.FirstOrDefault(nv => nv.MaNhanVien == taiKhoan.MaNhanVien).TenNhanVien;
        }
        
        private void menu_dangxuat_Click(object sender, RoutedEventArgs e)
        {
            DangNhap dangNhap = new DangNhap();
            dangNhap.Show();
            this.Close();
        }
        public void hienthi_dgdsdouong()
        {
            QuanLyQuanCafeContext db1 = new QuanLyQuanCafeContext();
            dg_dsdouong.ItemsSource= db1.DoUongs.ToList();
        }
        private void manhinhchinh_Loaded(object sender, RoutedEventArgs e)
        {
            hienthi_cboTang();
            hienthi_paneldsbantheotang();
            hienthi_thongtinhoadon();
            hienthi_dgdsdouong();

        }

        private void cbo_tang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbo_tang.SelectedValue != null)
            {
                hienthi_paneldsbantheotang();
            }
        }

        private void menuitem_nhanvien_Click(object sender, RoutedEventArgs e)
        {
            QuanLyNhanVien quanLyNhanVien = new QuanLyNhanVien(this, taiKhoan);
            quanLyNhanVien.Show();
            this.Hide();
        }

        private void menuitem_nhanvien1_Click(object sender, RoutedEventArgs e)
        {
            QuanLyTaiKhoan quanlytaikhoan = new QuanLyTaiKhoan(this);
            quanlytaikhoan.Show(); this.Hide();
        }

        private void menuitem_thongtincanhan_Click(object sender, RoutedEventArgs e)
        {
            ThongTinCaNhan thongTinCaNhan = new ThongTinCaNhan(this, taiKhoan);
            thongTinCaNhan.Show();
            this.Hide();
        }

        private void menuitem_loaidouong_Click(object sender, RoutedEventArgs e)
        {
            QuanLyLoaiDoUong quanlyloaidouong = new QuanLyLoaiDoUong(this);
            quanlyloaidouong.Show();
            this.Hide();
        }

        private void menuitem_douong_Click(object sender, RoutedEventArgs e)
        {
            QuanLyDoUong quanLyDoUong = new QuanLyDoUong(this);
            quanLyDoUong.Show(); 
            this.Hide();
        }

        private void menuitem_ban_Click(object sender, RoutedEventArgs e)
        {
            QuanLyBan quanLyBan = new QuanLyBan(this);
            quanLyBan.Show();
            this.Hide();
        }

        private void menuitem_tang_Click(object sender, RoutedEventArgs e)
        {
            QuanLyTang quanLyTang = new QuanLyTang(this);
            quanLyTang.Show();
            this.Hide();
        }

        private void menuitem_nguyenlieu_Click(object sender, RoutedEventArgs e)
        {
            QuanLyNguyenLieu quanLyNguyenLieu = new QuanLyNguyenLieu(this);
            quanLyNguyenLieu.Show();
            this.Hide();
        }

        private void menuitem_congthucphache_Click(object sender, RoutedEventArgs e)
        {
            QuanLyCongThucPhaChe quanLyCongThucPhaChe = new QuanLyCongThucPhaChe(this);
            quanLyCongThucPhaChe.Show();
            this.Hide();
        }

        private void menuitem_thongketheongay_Click(object sender, RoutedEventArgs e)
        {
            ThongKeTheoNgay thongKeTheoNgay = new ThongKeTheoNgay(this);
            thongKeTheoNgay.Show();
            this.Hide();
        }

        private void menuitem_thongketheothang_Click(object sender, RoutedEventArgs e)
        {
            ThongKeTheoThang thongKeTheoThang = new ThongKeTheoThang(this);
            thongKeTheoThang.Show();
            this.Hide();
        }

        private void menuitem_thongketheodouong_Click(object sender, RoutedEventArgs e)
        {
            ThongKeTheoDoUong thongketheodouong=new ThongKeTheoDoUong(this);
            thongketheodouong.Show();
            this.Hide();
        }

        private void menuitem_lichsuhoadon_Click(object sender, RoutedEventArgs e)
        {
            ThongKeLichSuHoaDon lichsuhoadon = new ThongKeLichSuHoaDon(this);
            lichsuhoadon.Show();
            this.Hide();
        }

        private void btn_thoat1_Click(object sender, RoutedEventArgs e)
        {
            QuanLyQuanCafeContext db1 = new QuanLyQuanCafeContext();
            foreach (var ban in db1.Bans)
            {
                ban.TrangThai = false;
            }
            db1.SaveChanges();
            this.Close();
        }

        private void btn_thoat_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txt_timdouong.Text.Trim()))
            {
                MessageBox.Show("chưa nhập tên đồ uống cần tìm!");
                txt_timdouong.Focus();
                return;
            }
            var kq=db.DoUongs.Where(du=>du.TenDoUong.ToLower().Trim().Contains(txt_timdouong.Text.ToLower().Trim())).ToList();
            if(kq.Count == 0)
            {
                MessageBox.Show("không tìm thấy!");
                txt_timdouong.Focus();
                hienthi_dgdsdouong();
                return;
            }
            dg_dsdouong.ItemsSource = kq;
        }
       
        private void btn_themdouonggoi_Click(object sender, RoutedEventArgs e)
        {
            if (bandangchon == null)
            {
                MessageBox.Show("chưa chọn bàn!");
                return;
            }
            if (dg_dsdouong.SelectedItem == null)
            {
                MessageBox.Show("chưa chọn đồ uống!");
                return;
            }
            if (string.IsNullOrEmpty(txt_soluongdouonggoi.Text.Trim()))
            {
                MessageBox.Show("chưa nhập số lượng đồ!");
                txt_soluongdouonggoi.Focus();
                return;
            }
            if(!int.TryParse(txt_soluongdouonggoi.Text.Trim(), out int soluong))
            {
                MessageBox.Show("số lượng đồ gọi phải là số nguyên!");
                txt_soluongdouonggoi.Focus(); 
                return;

            }
            if(soluong <= 0)
            {
                MessageBox.Show("số lượng đồ gọi phải là số nguyên dương!");
                txt_soluongdouonggoi.Focus();
                return;
            }
            them_do_uong_vao_dgdsdouonggoi();
            dg_dsdouong.SelectedItem = null;
            txt_soluongdouonggoi.Text= string.Empty;    
        }
        private void them_do_uong_vao_dgdsdouonggoi()
        {
            var douonggoi=dg_dsdouong.SelectedItem as DoUong;
            var bandanggoido = dsBan.FirstOrDefault(b => b.ban.MaBan.Trim() == bandangchon.MaBan.Trim());
            var du_them = bandanggoido.dsDoUong.FirstOrDefault(du => du.MaDoUong == douonggoi.MaDoUong);
            if(du_them != null)
            {
                if (MessageBox.Show("bàn này đã gọi " + douonggoi.TenDoUong + " , nếu gọi thêm sẽ cộng vào hóa đơn", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
                du_them.SoLuong += int.Parse(txt_soluongdouonggoi.Text.Trim());
                du_them.ThanhTien=du_them.SoLuong*du_them.Gia;
                dg_dsdouonggoi.ItemsSource = bandanggoido.dsDoUong.ToList();
                tb_tongtienthanhtoan.Text = bandanggoido.dsDoUong.Sum(du => du.SoLuong * du.Gia).ToString("N0");
                return;
            }
            DG_DSDoUongGoi.stt=bandanggoido.dsDoUong.Count;
            bandanggoido.dsDoUong.Add(new DG_DSDoUongGoi()
            {
                MaDoUong = douonggoi.MaDoUong,
                TenDoUong=douonggoi.TenDoUong,
                SoLuong=int.Parse(txt_soluongdouonggoi.Text.Trim()),
                Gia=(int)douonggoi.DonGia,
                ThanhTien= int.Parse(txt_soluongdouonggoi.Text.Trim())*(int)douonggoi.DonGia
            });
            dg_dsdouonggoi.ItemsSource=bandanggoido.dsDoUong.ToList();
            tb_tongtienthanhtoan.Text=bandanggoido.dsDoUong.Sum(du=>du.SoLuong*du.Gia).ToString("N0");
        }

        private void btn_xoadouonggoi_Click_1(object sender, RoutedEventArgs e)
        {
            if (bandangchon == null)
            {
                MessageBox.Show("chưa chọn bàn");
                return;
            }
            var bandangduochon = dsBan.FirstOrDefault(b => b.ban.MaBan.Trim() == bandangchon.MaBan.Trim());
            if (bandangduochon.dsDoUong.Count == 0)
            {
                MessageBox.Show("Bàn này chưa gọi đồ");
                return;
            }
            if (dg_dsdouonggoi.SelectedItem == null)
            {
                MessageBox.Show("chưa chọn đồ uống cần xóa");
                return;
            }
            var du_xoa = dg_dsdouonggoi.SelectedItem as DG_DSDoUongGoi;
            bandangduochon.dsDoUong.Remove(du_xoa);
            dg_dsdouonggoi.ItemsSource = bandangduochon.dsDoUong.ToList();
            dg_dsdouonggoi.SelectedItem = null;
            tb_tongtienthanhtoan.Text = bandangduochon.dsDoUong.Sum(du => du.SoLuong * du.Gia).ToString("N0");
        }

        private void btn_thanhtoanhoadon_Click(object sender, RoutedEventArgs e)
        {
            if (bandangchon == null)
            {
                MessageBox.Show("chưa chọn bàn");
                return;
            }
            var bandangduochon = dsBan.FirstOrDefault(b => b.ban.MaBan.Trim() == bandangchon.MaBan.Trim());
            if (bandangduochon.dsDoUong.Count == 0)
            {
                MessageBox.Show("Bàn này chưa gọi đồ");
                return;
            }
            HoaDon hd_new = new HoaDon();
            var sothutuhoadon = db.HoaDons.Count()+1;
            var stthd= (sothutuhoadon < 10) ? ("0" + sothutuhoadon.ToString()) : sothutuhoadon.ToString();
            hd_new.MaHoaDon = "HD" + stthd;
            hd_new.MaNhanVien = taiKhoan.MaNhanVien;
            hd_new.MaBan=bandangchon.MaBan;
            hd_new.NgayLap=DateOnly.FromDateTime(DateTime.Now);
            hd_new.TongTien = bandangduochon.dsDoUong.Sum(du => du.SoLuong * du.Gia);
            hd_new.TrangThai = true;
            if (MessageBox.Show("Xác nhận thanh toán hóa đơn " + hd_new.MaHoaDon + " với tổng tiền thanh toán " + tb_tongtienthanhtoan.Text + " VNĐ", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            db.HoaDons.Add(hd_new);
            foreach (var du in bandangduochon.dsDoUong)
            {
                ChiTietHoaDon cthd_new = new ChiTietHoaDon();
                cthd_new.MaHoaDon = hd_new.MaHoaDon;
                cthd_new.MaDoUong = du.MaDoUong;
                cthd_new.SoLuong = du.SoLuong;
                cthd_new.DonGia = du.Gia;
                cthd_new.ThanhTien = du.SoLuong * du.Gia;
                db.ChiTietHoaDons.Add(cthd_new);
            }
            //in hóa đơn
            var report = new StiReport();
            report.Load("D:\\lap_trinh.NET\\QuanLyQuanCafe\\QuanLyQuanCafe\\GUI\\GUI_CHUNG\\HoaDon.mrt");
            
            report.Dictionary.Variables["MaHoaDon"].Value = hd_new.MaHoaDon;
            report.Dictionary.Variables["TenNhanVien"].Value = db.NhanViens.FirstOrDefault(nv=>nv.MaNhanVien.Trim()==taiKhoan.MaNhanVien.Trim()).TenNhanVien;
            report.Dictionary.Variables["Ngay"].Value = DateTime.Now.ToString("dd/MM/yy");
            report.Dictionary.Variables["ViTri"].Value = bandangchon.TenBan + " Tầng " + cbo_tang.Text;
            report.RegBusinessObject("DG_DSDoUongGoi", bandangduochon.dsDoUong);
            report.Dictionary.Variables["TongTien"].Value = tb_tongtienthanhtoan.Text+" VNĐ";
            // ✅ Render sau khi gán biến
            report.Render();

            // Show
            report.ShowWithWpf();
            

            MessageBox.Show("Thanh toán và in hóa đơn thành công");
            bandangduochon.dsDoUong.Clear();
            
            var banthanhtoan=db.Bans.FirstOrDefault(b=>b.MaBan.Trim()==bandangchon.MaBan.Trim());
            banthanhtoan.TrangThai=false;
            db.SaveChanges();
            foreach(Button b in panel_dsbantungtang.Children)
            {
                if (b.Tag.ToString() == banthanhtoan.MaBan) b.Background = Brushes.LightGray;
            }

            bandangchon = null;
            hienthi_tbbandangchon();
            txt_timdouong.Text = "";
            txt_soluongdouonggoi.Text = "";
            tb_tongtienthanhtoan.Text = "";
            dg_dsdouonggoi.ItemsSource = null;
            dg_dsdouong.SelectedItem= null; 
        }

        
    }
}
