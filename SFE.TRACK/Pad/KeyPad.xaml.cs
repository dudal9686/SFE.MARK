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

namespace SFE.TRACK.Pad
{
    /// <summary>
    /// KeyPad.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyPad : Window
    {
        public float totalValue = 0;
        float minValue = -100;
        float maxValue = 100;
        public KeyPad(float value, float maxValue = 100000, float minValue = -100000)
        {
            InitializeComponent();
            txtValue.Text = value.ToString();
            this.maxValue = maxValue;
            this.minValue = minValue;
        }       

        private void btn01_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Text = txtValue.Text + "1";
            totalValue = Convert.ToSingle(txtValue.Text);
            txtValue.Text = totalValue.ToString();
        }

        private void btn02_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Text = txtValue.Text + "2";
            totalValue = Convert.ToSingle(txtValue.Text);
            txtValue.Text = totalValue.ToString();
        }

        private void btn03_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Text = txtValue.Text + "3";
            totalValue = Convert.ToSingle(txtValue.Text);
            txtValue.Text = totalValue.ToString();
        }

        private void btn04_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Text = txtValue.Text + "4";
            totalValue = Convert.ToSingle(txtValue.Text);
            txtValue.Text = totalValue.ToString();
        }

        private void btn05_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Text = txtValue.Text + "5";
            totalValue = Convert.ToSingle(txtValue.Text);
            txtValue.Text = totalValue.ToString();
        }

        private void btn06_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Text = txtValue.Text + "6";
            totalValue = Convert.ToSingle(txtValue.Text);
            txtValue.Text = totalValue.ToString();
        }

        private void btn07_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Text = txtValue.Text + "7";
            totalValue = Convert.ToSingle(txtValue.Text);
            txtValue.Text = totalValue.ToString();
        }

        private void btn08_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Text = txtValue.Text + "8";
            totalValue = Convert.ToSingle(txtValue.Text);
            txtValue.Text = totalValue.ToString();
        }

        private void btn09_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Text = txtValue.Text + "9";
            totalValue = Convert.ToSingle(txtValue.Text);
            txtValue.Text = totalValue.ToString();
        }

        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            totalValue = Convert.ToSingle(txtValue.Text);
            totalValue = (-1) * totalValue;
            txtValue.Text = totalValue.ToString();
        }

        private void btn00_Click(object sender, RoutedEventArgs e)
        {
            totalValue = Convert.ToSingle(txtValue.Text);
            txtValue.Text += "0";
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (txtValue.Text.Length == 1) txtValue.Text = "0";
            else txtValue.Text = txtValue.Text.Substring(0, txtValue.Text.Length - 1);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtValue.Text = "0";
        }

        private void btnComma_Click(object sender, RoutedEventArgs e)
        {
            if (txtValue.Text.IndexOf(".") != -1) return;

            totalValue = Convert.ToSingle(txtValue.Text);
            txtValue.Text += ".";
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            totalValue = Convert.ToSingle(txtValue.Text);

            if(totalValue > maxValue || totalValue < minValue)
            {
                Global.MessageOpen(enMessageType.OK, string.Format("Please check the range. [{0} ~ {1}]", maxValue, minValue));
                return;
            }

            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
