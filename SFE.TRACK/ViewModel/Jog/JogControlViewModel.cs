using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace SFE.TRACK.ViewModel.Jog
{
    public class JogControlViewModel : ViewModelBase
    {
        public RelayCommand MouseDownPlusRelayCommand { get; set; }
        public RelayCommand MouseUpPlusRelayCommand { get; set; }
        public RelayCommand MouseDownMinusRelayCommand { get; set; }
        public RelayCommand MouseUpMinusRelayCommand { get; set; }
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }
        public RelayCommand PitchClickRelayCommand { get; set; }
        bool[] isVelocity = new bool[3] { true, false, false };
        float pitchLen = 0.1f;
        enJogMode jogMode = enJogMode.LOW;
        public JogControlViewModel()
        {
            MouseDownPlusRelayCommand = new RelayCommand(MouseDownPlusCommand);
            MouseUpPlusRelayCommand = new RelayCommand(MouseUpPlusCommand);
            MouseDownMinusRelayCommand = new RelayCommand(MouseDownMinusCommand);
            MouseUpMinusRelayCommand = new RelayCommand(MouseUpMinusCommand);
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            PitchClickRelayCommand = new RelayCommand(PitchClickCommand);
            Messenger.Default.Register<TeachingDataMessageCls>(this, OnReceiveMessageAction);
        }
        public bool[] IsVelocity
        {
            get { return isVelocity; }
            set { isVelocity = value;
                if (isVelocity[0]) jogMode = enJogMode.LOW;
                else if (isVelocity[1]) jogMode = enJogMode.HIGH;
                else if (isVelocity[2]) jogMode = enJogMode.PITCH;
                RaisePropertyChanged("IsVelocity"); }
        }

        public float PitchLen
        {
            get { return pitchLen; }
            set { pitchLen = value; RaisePropertyChanged("PitchLen"); }
        }

        private void OnReceiveMessageAction(TeachingDataMessageCls o)
        {
            
        }

        private void MouseDownPlusCommand()
        {
            Console.WriteLine(jogMode);
            Console.WriteLine("AAAAA");
        }

        private void MouseUpPlusCommand()
        {
            Console.WriteLine("BBBBB");
        }

        private void MouseDownMinusCommand()
        {
            Console.WriteLine(jogMode);
            Console.WriteLine("CCCCC");
        }

        private void MouseUpMinusCommand()
        {
            Console.WriteLine("DDDDD");
        }

        private void OKCommand(Window window)
        {
            //현 위치 값.
            Global.STTeachingMessage.Position = 0;
            window.DialogResult = true;
        }

        private void CancelCommand(Window window)
        {
            window.DialogResult = false;
        }

        private void PitchClickCommand()
        {
            PitchLen = Global.KeyPad(PitchLen);
        }
    }
}
