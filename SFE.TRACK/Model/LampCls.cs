using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Media;

namespace SFE.TRACK.Model
{

    public class LampCls : ViewModelBase
    {
        private string title = string.Empty;

        private string redString = enLamp.ON.ToString();
        private string yellowString = enLamp.ON.ToString();
        private string greenString = enLamp.ON.ToString();
        private string buzzerString = enLamp.ON.ToString();
        public int BuzzerTimeOut = 0;

        private SolidColorBrush redColor = Brushes.Tomato;
        private SolidColorBrush yellowColor = Brushes.LightYellow;
        private SolidColorBrush greenColor = Brushes.LightGreen;
        private SolidColorBrush buzzerColor = Brushes.CornflowerBlue;

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged("Title"); }
        }

        public string RedString
        {
            get { return redString; }
            set { redString = value;
                if (redString == enLamp.ON.ToString() || redString == enLamp.TOGGLE.ToString()) RedColor = Brushes.Tomato;
                else RedColor = Brushes.Transparent;
                RaisePropertyChanged("RedString"); }
        }

        public string YellowString
        {
            get { return yellowString; }
            set { yellowString = value;
                if (yellowString == enLamp.ON.ToString() || yellowString == enLamp.TOGGLE.ToString()) YellowColor = Brushes.LightYellow;
                else YellowColor = Brushes.Transparent;
                RaisePropertyChanged("YellowString"); }
        }

        public string GreenString
        {
            get { return greenString; }
            set { greenString = value;
                if (greenString == enLamp.ON.ToString() || greenString == enLamp.TOGGLE.ToString()) GreenColor = Brushes.LightGreen;
                else GreenColor = Brushes.Transparent;
                RaisePropertyChanged("GreenString"); }
        }

        public string BuzzerString
        {
            get { return buzzerString; }
            set { buzzerString = value;
                if (buzzerString == enBuzzer.ON.ToString()) BuzzerColor = Brushes.CornflowerBlue;
                else BuzzerColor = Brushes.Transparent;
                RaisePropertyChanged("BuzzerString"); }
        }

        public SolidColorBrush RedColor
        {
            get { return redColor; }
            set { redColor = value; RaisePropertyChanged("RedColor"); }
        }

        public SolidColorBrush YellowColor
        {
            get { return yellowColor; }
            set { yellowColor = value; RaisePropertyChanged("YellowColor"); }
        }

        public SolidColorBrush GreenColor
        {
            get { return greenColor; }
            set { greenColor = value; RaisePropertyChanged("GreenColor"); }
        }

        public SolidColorBrush BuzzerColor
        {
            get { return buzzerColor; }
            set { buzzerColor = value; RaisePropertyChanged("BuzzerColor"); }
        }
    }
}
