using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SFE.TRACK.Pad
{
    /// <summary>
    /// KeyBoard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyBoard : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public const int KEYEVENTF_KEYDOWN = 0x00;
        public const int KEYEVENTF_EXTENDEDKEY = 0x1;
        public const int KEYEVENTF_KEYUP = 0x02;
        public const int BACK = 0x08; // Backspace keycode.

        public string totalValue = string.Empty;

        public KeyBoard(string value)
        {
            InitializeComponent();
            txtValue.Text = value;
            txtValue.Select(txtValue.Text.Length, 0);
            txtValue.Focus();
        }

        private void btnBackSpace_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(BACK, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(BACK, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn01_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x31, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x32, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn02_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x32, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x32, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn03_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x33, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x33, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn04_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x34, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x34, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn05_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x35, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x35, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn06_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x36, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x36, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn07_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x37, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x37, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn08_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x38, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x38, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn09_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x39, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x39, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn00_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x30, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x30, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btnBar_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event((byte)Keys.OemMinus, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event((byte)Keys.OemMinus, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btnUnderBar_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();

            keybd_event((byte)Keys.ShiftKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0);
            keybd_event((byte)Keys.OemMinus, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0);
            keybd_event((byte)Keys.ShiftKey, (byte)Key.LeftShift, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            keybd_event((byte)Keys.OemMinus, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        private void btn_Q_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x51, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x51, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_W_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x57, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x57, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_E_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x45, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x45, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_R_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x52, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x52, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_T_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x54, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x54, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_Y_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x59, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x59, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_U_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x55, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x55, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_I_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x49, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x49, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_O_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x4F, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x4F, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_P_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x50, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x50, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btnLeftBracket_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0xDB, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0xDB, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btnRightBracket_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0xDD, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0xDD, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btnBackSlach_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0xDC, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0xDC, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_A_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x41, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x41, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_S_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x53, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x53, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_D_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x44, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x44, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_F_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x46, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x46, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_G_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x47, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x47, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_H_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x48, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x48, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_J_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x4A, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x4A, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_K_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x4B, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x4B, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_L_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x4C, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x4C, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btnSemiColon_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0xBA, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0xBA, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btnColon_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();

            keybd_event((byte)Keys.ShiftKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0);
            keybd_event(0xBA, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0);
            keybd_event((byte)Keys.ShiftKey, (byte)Key.LeftShift, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            keybd_event(0xBA, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            
        }

        private void btn_Z_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x5A, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x5A, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_X_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x58, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x58, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_C_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x43, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x43, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_V_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x56, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x56, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_B_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x42, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x42, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_N_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x4E, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x4E, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btn_M_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x4D, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x4D, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btnComma_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0xBC, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0xBC, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btnDot_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0xBE, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0xBE, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btnSlash_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0xBF, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0xBF, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btnCap_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x14, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x14, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btnSpace_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Focus();
            keybd_event(0x20, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(0x20, 0, KEYEVENTF_KEYUP, 0);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            totalValue = txtValue.Text;
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
