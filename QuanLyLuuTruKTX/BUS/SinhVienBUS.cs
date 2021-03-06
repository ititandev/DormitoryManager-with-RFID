﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using DAO;
using DTO;
namespace BUS
{
    /// <summary>
    /// Class xử lý nghiệp vụ của form SinhVien
    /// </summary>
    public static class SinhVienBUS
    {
        public static string HoTen = "Họ và tên";
        public static string MaSo = "MSSV";
        public static string NgaySinh = "Ngày sinh";
        public static string GioiTinh = "Giới tính";
        public static string CMND = "CMND";
        public static string DienThoai = "Điện thoại";
        public static string Lop = "Lớp";
        public static string Khoa = "Khoa";
        public static string Que = "Quê";
        public static string DienUuTien = "Diện ưu tiên";
        public static string Anh = "Ảnh";
        public static string Email = "Email";
        public static string RFID = "RFID";

        /// <summary>
        /// Xem tất cả Sinh viên từ cơ sở dữ liệu
        /// </summary>
        /// <returns></returns>
        public static DataTable LoadSinhVien()
        {
            DataTable dt = SinhVienDAO.LoadSinhVien();
            dt.Columns[0].ColumnName = MaSo;
            dt.Columns[1].ColumnName = HoTen;
            dt.Columns[2].ColumnName = NgaySinh;
            dt.Columns[3].ColumnName = GioiTinh;
            dt.Columns[4].ColumnName = CMND;
            dt.Columns[5].ColumnName = Lop;
            dt.Columns[6].ColumnName = DienThoai;
            dt.Columns[7].ColumnName = Khoa;
            dt.Columns[8].ColumnName = Que;
            dt.Columns[9].ColumnName = DienUuTien;
            dt.Columns[10].ColumnName = Anh;
            dt.Columns[11].ColumnName = Email;
            dt.Columns[12].ColumnName = RFID;
            return dt;
        }
        /// <summary>
        /// Cập nhật thông tin sinh viên theo SinhVienDTO
        /// </summary>
        /// <param name="sv"></param>
        /// <returns></returns>
        public static bool CapNhatSinhVien(SinhVienDTO sv)
        {
            bool test = Data.KiemTraRong(sv.HoTen, sv.MSSV, sv.GioiTinh, sv.CMND, sv.Lop, sv.SoDienThoai, sv.Khoa, sv.QueQuan, sv.Email);
            if (test)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!Data.KiemTraRong(sv.RFID) && SinhVienDAO.KiemTraTrungRFID(sv.RFID))
            {
                MessageBox.Show("Thẻ đã bị trùng với một sinh viên khác", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (SinhVienDAO.CapNhatSinhVien(sv) == 1)
            {
                MessageBox.Show("Cập nhật sinh viên '" + sv.HoTen + "' (" + sv.MSSV + ") thành công");
                return true;
            }
            else
            {
                MessageBox.Show("Cập nhật sinh viên thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public static bool XoaSinhVien(string MSSV)
        {
            if (SinhVienDAO.XoaSinhVien(MSSV) == 1)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Thêm sinh viên theo SinhVienDTO truyền vào
        /// </summary>
        /// <param name="sv"></param>
        public static bool ThemSinhVien(SinhVienDTO sv)
        {
            bool test = Data.KiemTraRong(sv.HoTen, sv.MSSV, sv.GioiTinh, sv.CMND, sv.Lop, sv.SoDienThoai, sv.Khoa, sv.QueQuan, sv.Email);
            if (test)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!Data.KiemTraRong(sv.RFID) && SinhVienDAO.KiemTraTrungRFID(sv.RFID))
            {
                MessageBox.Show("Thẻ đã bị trùng với một sinh viên khác", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (SinhVienDAO.ThemSinhVien(sv) == 1)
            {
                MessageBox.Show("Thêm sinh viên '" + sv.HoTen + "' (" + sv.MSSV + ") thành công");
                return true;
            }
            else
            {
                MessageBox.Show("Thêm sinh viên thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra sinh viên có hợp lệ hay không
        /// </summary>
        /// <param name="MSSV"></param>
        /// <returns></returns>
        public static bool KiemTraSinhVien(string MSSV)
        {
            if (SinhVienDAO.KiemTraSinhVien(MSSV) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Lấy SinhVienDTO theo MSSV
        /// </summary>
        /// <param name="MSSV"></param>
        /// <returns></returns>
        public static SinhVienDTO GetSinhVienDTO(string MSSV)
        {
            return SinhVienDAO.GetSinhVienDTO(MSSV);
        }
        /// <summary>
        /// Chuyển sinh viên có MSSV tới phòng IDPhong
        /// check là tùy chọn kiểm tra phòng quá số lượng hay không
        /// </summary>
        /// <param name="MSSV"></param>
        /// <param name="idPhong"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static int ChuyenPhong(string MSSV, string idPhong, bool check)
        {
            if (check)
                return Data.ExecuteNonQuery($"IF dbo.KiemTraChuyenPhong('{ idPhong}')=1 UPDATE HopDong SET IDPhong='{idPhong}' WHERE MSSV='{MSSV}' AND dbo.KiemTraThoiHan('{MSSV}')=1");
            return Data.ExecuteNonQuery($"UPDATE HopDong SET IDPhong='{idPhong}' WHERE MSSV='{MSSV}' AND dbo.KiemTraThoiHan('{MSSV}')=1");
        }
    }
}
