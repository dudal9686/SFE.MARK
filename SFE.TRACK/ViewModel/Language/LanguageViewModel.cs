using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Threading;

namespace SFE.TRACK.ViewModel.Language
{
    public class LanguageViewModel : ViewModelBase
    {
        List<LangDetailCls> list { get; set; } = new List<LangDetailCls>();
        public LangDetailCls SelectedItem { get; set; }
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }
        int selectedIndex = 0;
        string curLang = string.Empty;
        public LanguageViewModel()
        {
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            LangDetailCls langDetail = new LangDetailCls();
            langDetail.Lang = "English";
            langDetail.Code = "en-US";
            list.Add(langDetail);

            langDetail = new LangDetailCls();
            langDetail.Lang = "Chinese";
            langDetail.Code = "zh-CN";
            list.Add(langDetail);

            SetLanguage();
        }

        private void SetLanguage()
        {
            curLang = Properties.Settings.Default.LANG_CODE;
            if (curLang == "en-US") SelectedItem = list[0];
            else SelectedItem = list[1];
        }

        public List<LangDetailCls> LangList
        {
            get { return list; }
            set { list = value; RaisePropertyChanged("LangList"); }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged("SelectedIndex"); }
        }

        private void OKCommand(Window window)
        {
            if (!Global.MessageOpen(enMessageType.OKCANCEL, SFE.TRACK.Language.Localization.ResString("PROGRAM.RESTART")))
            {
                SetLanguage();
                return;
            }
            Properties.Settings.Default.LANG_CODE = SelectedItem.Code;
            Properties.Settings.Default.Save();
            ((MainWindow)System.Windows.Application.Current.MainWindow).Close();
            window.DialogResult = true;
        }

        private void CancelCommand(Window window)
        {
            SetLanguage();
            window.DialogResult = false;
        }
    }

    public class LangDetailCls : ViewModelBase
    {
        string lang = string.Empty;
        string code = string.Empty;

        public string Lang
        {
            get { return lang; }
            set { lang = value; RaisePropertyChanged("Lang"); }
        }

        public string Code
        {
            get { return code; }
            set { code = value; RaisePropertyChanged("Code"); }
        }

    }
}
