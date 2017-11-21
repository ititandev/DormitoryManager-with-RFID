﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BUS;
using DTO;

namespace GUI
{
    public partial class Phong : KTXForm
    {
        DataTable dataTablePhong;
        DataTable dataTableSinhVien;

        private enum CheDo
        {
            XEM, SUA, THEM, CHON_PHONG
        }
        private static CheDo CheDoHienTai { get; set; }
        public Phong()
        {
            InitializeComponent();
        }
        private void Phong_Load(object sender, EventArgs e)
        {
            SetCheDo(CheDo.XEM);
            LoadDuLieu();
            cboLoaiPhong.SelectedIndex = 0;
            cboSoLuong.SelectedIndex = 0;
        }
        private void Phong_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.phongForm = null;
        }

        private void xemMode_Click(object sender, EventArgs e)
        {
            SetCheDo(CheDo.XEM);
        }

        private void suaMode_Click(object sender, EventArgs e)
        {
            SetCheDo(CheDo.SUA);
        }

        private void themMode_Click(object sender, EventArgs e)
        {
            SetCheDo(CheDo.THEM);
        }
        private void SetCheDo(CheDo cheDo)
        {
            CheDoHienTai = cheDo;
            if (CheDoHienTai == CheDo.XEM)
            {
                xemMode.BackColor = Color.LightSlateGray;
                suaMode.BackColor = Color.LightGray;
                themMode.BackColor = Color.LightGray;
                txtIDPhong.ReadOnly = txtKhuNha.ReadOnly = txtSoPhong.ReadOnly = txtTinhTrang.ReadOnly = true;
                btnHanhDong.Hide();
                btnChonPhong.Hide();
            }
            else if (CheDoHienTai == CheDo.THEM)
            {
                xemMode.BackColor = Color.LightGray;
                suaMode.BackColor = Color.LightGray;
                themMode.BackColor = Color.LightSlateGray;

                txtIDPhong.ReadOnly = txtKhuNha.ReadOnly = txtSoPhong.ReadOnly = txtTinhTrang.ReadOnly = false;
                txtIDPhong.Text = txtKhuNha.Text = txtSoPhong.Text = txtTinhTrang.Text = "";
                numToiDa.Value = 8;
                numHienTai.Value = 0;

                btnHanhDong.Text = "Thêm phòng";
                btnHanhDong.Show();
                btnChonPhong.Hide();
            }
            else if (CheDoHienTai == CheDo.SUA)
            {
                xemMode.BackColor = Color.LightGray;
                suaMode.BackColor = Color.LightSlateGray;
                themMode.BackColor = Color.LightGray;

                txtIDPhong.ReadOnly = txtKhuNha.ReadOnly = txtSoPhong.ReadOnly = txtTinhTrang.ReadOnly = false;
                btnHanhDong.Text = "Cập nhật";
                btnHanhDong.Show();
                btnChonPhong.Hide();
            }
            else if (CheDoHienTai == CheDo.CHON_PHONG)
            {
                LoadDuLieu();
                btnChonPhong.Show();
                cboLoaiPhong.SelectedIndex = 1;
            }
        }
        private void LoadDuLieu()
        {
            dataTablePhong = PhongBUS.LoadPhong();
            dataTablePhong.Columns[0].ColumnName = "Mã phòng";
            dataTablePhong.Columns[1].ColumnName = "Khu nhà";
            dataTablePhong.Columns[2].ColumnName = "Số phòng";
            dataTablePhong.Columns[3].ColumnName = "Số lượng hiện tại";
            dataTablePhong.Columns[4].ColumnName = "Số lượng tối đa";
            dataTablePhong.Columns[5].ColumnName = "Tình trạng";
            dgvPhong.DataSource = dataTablePhong;

            dataTableSinhVien = PhongBUS.LoadSinhVien();
            dataTableSinhVien.Columns[0].ColumnName = "Mã phòng";
            dataTableSinhVien.Columns[1].ColumnName = "MSSV";
            dataTableSinhVien.Columns[2].ColumnName = "Họ tên";
            dgvSinhVien.DataSource = dataTableSinhVien;
            dgvSinhVien.Columns[3].Visible = false;
            TimKiem();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("test");
        }

        private void Phong_Activated(object sender, EventArgs e)
        {
            MainForm.FormHienTai = MainForm.phongForm;
        }

        private void dgvPhong_SelectionChanged(object sender, EventArgs e)
        {
            if (CheDoHienTai == CheDo.THEM)
                return;
            foreach (DataGridViewRow row in dgvPhong.SelectedRows)
            {
                txtIDPhong.Text = row.Cells[0].Value.ToString();
                txtKhuNha.Text = row.Cells[1].Value.ToString();
                txtSoPhong.Text = row.Cells[2].Value.ToString();
                txtTinhTrang.Text = row.Cells[5].Value.ToString();
                numHienTai.Value = Convert.ToInt32(row.Cells[3].Value);
                numToiDa.Value = Convert.ToInt32(row.Cells[4].Value);
                try
                {
                    if (dataTableSinhVien != null)
                        dataTableSinhVien.DefaultView.RowFilter = $"[Mã phòng] = '{txtIDPhong.Text}'";
                }
                catch (Exception)
                {
                    return;
                }

            }
        }

        private void btnHanhDong_Click(object sender, EventArgs e)
        {
            PhongDTO phongDTO = new PhongDTO();
            phongDTO.IDPhong = txtIDPhong.Text;
            phongDTO.KhuNha = txtKhuNha.Text;
            phongDTO.SoPhong = txtSoPhong.Text;
            phongDTO.TinhTrang = txtTinhTrang.Text;
            phongDTO.SoLuongToiDa = (int)numToiDa.Value;

            if (CheDoHienTai == CheDo.THEM)
            {
                if (PhongBUS.ThemPhongDTO(phongDTO))
                {
                    MessageBox.Show("Thêm phòng " + phongDTO.IDPhong + " thành công");
                    LoadDuLieu();
                }
                else
                    MessageBox.Show("Thêm phòng " + phongDTO.IDPhong + " thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (CheDoHienTai == CheDo.SUA)
            {
                if (PhongBUS.SuaPhongDTO(phongDTO))
                {
                    MessageBox.Show("Thêm phòng " + phongDTO.IDPhong + " thành công");
                    LoadDuLieu();
                }
                else
                    MessageBox.Show("Thêm phòng " + phongDTO.IDPhong + " thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public override void SendRFID(string RFID)
        {
            SetCheDo(CheDo.XEM);
            LoadDuLieu();
            string IDPhong = PhongBUS.GetIDPhongFromRFID(RFID);
            if (IDPhong == String.Empty)
                MessageBox.Show("Không tìm thấy SV này trong phòng nào", "Thông báo");
            for (int i = 0; i < dgvPhong.Rows.Count; i++)
            {
                if (dgvPhong.Rows[i].Cells["Mã phòng"].Value.ToString() == IDPhong)
                    dgvPhong.CurrentCell = this.dgvPhong["Mã phòng", i];
            }
            for (int i = 0; i < dgvSinhVien.Rows.Count; i++)
            {

                if (dgvSinhVien.Rows[i].Cells["RFID"].Value.ToString() == RFID)
                    dgvSinhVien.CurrentCell = this.dgvSinhVien["RFID", i];
            }
        }

        private void cboLoaiPhong_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimKiem();
        }

        private void cboSoLuong_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimKiem();
        }
        private void TimKiem()
        {
            string filter = string.Empty;
            if (cboLoaiPhong.SelectedIndex != 0)
                filter = "([Số lượng hiện tại] < [Số lượng tối đa]) ";

            if (cboSoLuong.SelectedIndex != 0)
            {
                if (filter != string.Empty)
                    filter += "And ";
                filter += "([Số lượng tối đa] = " + (cboSoLuong.SelectedIndex * 2 + 2).ToString() + ")";
            }
            dataTablePhong.DefaultView.RowFilter = filter;
        }
        public void XemPhong(string IDPhong, string MSSV)
        {
            LoadDuLieu();
            for (int i = 0; i < dgvPhong.Rows.Count; i++)
            {
                if (dgvPhong.Rows[i].Cells["Mã phòng"].Value.ToString() == IDPhong)
                    dgvPhong.CurrentCell = this.dgvPhong["Mã phòng", i];
            }
            for (int i = 0; i < dgvSinhVien.Rows.Count; i++)
            {
                if (dgvSinhVien.Rows[i].Cells["MSSV"].Value.ToString() == MSSV)
                    dgvSinhVien.CurrentCell = this.dgvSinhVien["MSSV", i];
            }
        }

        private void btnChonPhong_Click(object sender, EventArgs e)
        {
            if (dgvPhong.SelectedRows.Count != 1)
            {
                MessageBox.Show("Vui lòng chọn một phòng phù hợp", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int hienTai = Convert.ToInt32(dgvPhong.SelectedRows[0].Cells["Số lượng hiện tại"].Value);
            int toiDa = Convert.ToInt32(dgvPhong.SelectedRows[0].Cells["Số lượng tối đa"].Value);
            if (hienTai < toiDa)
            {
                MainForm.hopDongForm.SetIDPhong(dgvPhong.SelectedRows[0].Cells["Mã phòng"].Value.ToString());
                Program.mainForm.btnHopDong_Click(null, null);
            }
            else
                MessageBox.Show("Vui lòng chọn một phòng phù hợp", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        internal void SetChonPhong()
        {
            SetCheDo(CheDo.CHON_PHONG);
        }
    }
}
