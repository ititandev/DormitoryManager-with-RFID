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
using System.Text.RegularExpressions;
using System.IO.Ports;
using System.Media;
using System.Runtime.InteropServices;

namespace GUI
{
    /// <summary>
    /// Class form cài đặt phần mềm (RFID, cơ sở dữ liệu)
    /// </summary>
    public partial class CaiDat : Form
    {
        public CaiDat()
        {
            InitializeComponent();
            btnRefresh_Click(0, null);
            hMenu = GetSystemMenu(this.Handle, false);
        }
        
        private IntPtr hMenu;

        #region Hide exit button
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(IntPtr hMenu, uint wIDEnableItem, uint wEnable);
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            EnableMenuItem(hMenu, 0xf060, 0x01);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            EnableMenuItem(hMenu, 0xf060, 0x01);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            EnableMenuItem(hMenu, 0xf060, 0x01);
        }
        #endregion
        

        #region CaiDat
        private void CaiDat_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.caiDatForm = null;
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        private void btnChonDuongDan_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtDuongDan.Text = folderBrowserDialog.SelectedPath;
                if (txtDuongDan.Text.LastIndexOf("\\") != txtDuongDan.Text.Length - 1)
                    txtDuongDan.Text += "\\";
            }
        }
        public void CapNhatDuLieu()
        {
            txtConnectString.Text = HopDongBUS.ConnectString;
            txtDuongDan.Text = Properties.Settings.Default.ThuMucAnh;
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (txtDuongDan.Text.LastIndexOf("\\") != txtDuongDan.Text.Length - 1)
                txtDuongDan.Text += "\\";

            HopDongBUS.ConnectString = txtConnectString.Text;
            Properties.Settings.Default.ThuMucAnh = txtDuongDan.Text;
            Properties.Settings.Default.Save();
            this.WindowState = FormWindowState.Minimized;
            SaveRFIDSettings();
        }
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region RFID
        delegate void SetTextCallbackCom(string text);

        String inputDataCom = "";
        String allIDCard = "";
        String currentIDCard = "";
        int currentIndex = 0;
        Regex regexFindLast = new Regex("\\[\\d{8}(\\d?)(\\d?)\\]\\z");
        Match match;

        private void SetTextCom(string text)
        {
            match = regexFindLast.Match(allIDCard);
            if (match.Success && (match.Value != currentIDCard || allIDCard.LastIndexOf(currentIDCard) != currentIndex))
            {
                currentIDCard = match.Value.Remove(0, 1);
                currentIDCard = currentIDCard.Remove(currentIDCard.Length - 1);
                lblRFID.Text = currentIDCard;
                if (allIDCard.Length > 100)
                    allIDCard.Remove(0, allIDCard.Length - 99);
                Console.Beep(4000, 200);
                HanhDong();
                currentIndex = allIDCard.LastIndexOf(currentIDCard);
            }
        }

        private void HanhDong()
        {
            if (MainForm.FormHienTai != null)
                MainForm.FormHienTai.SendRFID(currentIDCard);
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string[] ComList = SerialPort.GetPortNames();

            int[] ComNumberList = new int[ComList.Length];
            for (int i = 0; i < ComList.Length; i++)
            {
                ComNumberList[i] = int.Parse(ComList[i].Substring(3));
            }

            Array.Sort(ComNumberList);
            cbxComList.Items.Clear();
            foreach (int ComNumber in ComNumberList)
            {
                cbxComList.Items.Add("COM" + ComNumber.ToString());
            }

            bool found = false;
            foreach (string str in cbxComList.Items)
            {
                if (str == Properties.Settings.Default.PortName)
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                cbxComList.Text = Properties.Settings.Default.PortName;
            }

            chkAutoConnect.Checked = Properties.Settings.Default.AutoConnect;
            if (Properties.Settings.Default.AutoConnect)
            {
                if (e == null)
                    btnConnect_Click(null, null);
            }
        }


        private void SaveRFIDSettings()
        {
            Properties.Settings.Default.PortName = cbxComList.Text;
            Properties.Settings.Default.AutoConnect = chkAutoConnect.Checked;
            Properties.Settings.Default.Save();
        }
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            inputDataCom = serialPort.ReadExisting();
            allIDCard += inputDataCom;
            if (inputDataCom != String.Empty)
            {
                this.BeginInvoke(new SetTextCallbackCom(SetTextCom), new object[] { inputDataCom });
            }
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (cbxComList.Text == "")
            {
                lblState.Text = "Trạng thái: " + "Vui lòng chọn cổng COM";
                lblState.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (serialPort.IsOpen)
            {
                serialPort.Close();
                btnConnect.Text = "Kết nối";
                cbxComList.Enabled = true;
                btnRefresh.Enabled = true;
                lblState.Text = "Trạng thái: " + "Ngắt kết nối thành công";
                lblState.ForeColor = System.Drawing.Color.CadetBlue;
            }
            else
            {
                try
                {
                    serialPort.PortName = cbxComList.Text;
                    serialPort.BaudRate = 9600;
                    serialPort.Open();

                    lblState.Text = "Trạng thái: " + "Kết nối thành công " + serialPort.PortName;
                    lblState.ForeColor = System.Drawing.Color.Blue;
                    btnConnect.Text = "Ngắt kết nối";
                    cbxComList.Enabled = false;
                    btnRefresh.Enabled = false;
                }
                catch
                {
                    lblState.Text = "Trạng thái: " + "Không thể mở cổng " + serialPort.PortName;
                    lblState.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
        private void chkAutoConnect_CheckedChanged(object sender, EventArgs e)
        {
            SaveRFIDSettings();
        }

        #endregion


    }
}
