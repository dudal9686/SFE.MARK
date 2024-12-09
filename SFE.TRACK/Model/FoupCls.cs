using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFE.TRACK.Model
{
    public class FoupCls : ModuleBaseCls
    {
        private List<WaferCls> FoupWaferList_ = null;
        private bool isScan = false;
        private bool isDetect = false;
        private string recipeName = string.Empty;
        private string comment = string.Empty;
        private string lotID = string.Empty;
        System.Windows.Media.SolidColorBrush foupColor = new System.Windows.Media.SolidColorBrush();

        public FoupCls()
        {
            FoupColor = System.Windows.Media.Brushes.Gray;
        }
        public List<WaferCls> FoupWaferList
        {
            get { return FoupWaferList_; }
            set { FoupWaferList_ = value; RaisePropertyChanged("FoupWaferList"); }
        }

        public bool IsScan
        {
            get { return isScan; }
            set { isScan = value;
                if (!IsScan && !IsDetect) FoupColor = System.Windows.Media.Brushes.Gray;
                if (IsDetect) FoupColor = System.Windows.Media.Brushes.Red;
                if (IsScan) FoupColor = System.Windows.Media.Brushes.Blue;
                if (!Use) FoupColor = System.Windows.Media.Brushes.Gray;
                RaisePropertyChanged("IsScan"); }
        }

        public bool IsDetect
        {
            get { return isDetect; }
            set { isDetect = value;
                if (!IsScan && !IsDetect) FoupColor = System.Windows.Media.Brushes.Gray;
                if (IsDetect) FoupColor = System.Windows.Media.Brushes.Red;
                if (IsScan) FoupColor = System.Windows.Media.Brushes.Blue;
                if (!Use) FoupColor = System.Windows.Media.Brushes.Gray;
                RaisePropertyChanged("IsDetect"); }
        }

        public string RecipeName
        {
            get { return recipeName; }
            set { recipeName = value; RaisePropertyChanged("RecipeName"); }
        }

        public string Comment
        {
            get { return comment; }
            set { comment = value; RaisePropertyChanged("Comment"); }
        }

        public string LotID
        {
            get { return lotID; }
            set { lotID = value; RaisePropertyChanged("LotID"); }
        }

        public System.Windows.Media.SolidColorBrush FoupColor
        {
            get { return foupColor; }
            set { foupColor = value; RaisePropertyChanged("FoupColor"); }
        }

        public FoupCls Clone()
        {
            FoupCls Foup_ = new FoupCls();
            Foup_.FoupWaferList = new List<WaferCls>();
            Foup_.IsScan = this.IsScan;
            Foup_.IsDetect = this.IsDetect;
            Foup_.RecipeName = this.RecipeName;
            Foup_.Comment = this.Comment;
            Foup_.LotID = this.LotID;
            Foup_.FoupColor = this.FoupColor;
            Foup_.BlockNo = this.BlockNo;
            Foup_.ModuleNo = this.ModuleNo;
            Foup_.SizeX = this.SizeX;
            Foup_.SizeY = this.SizeY;
            Foup_.SizeX = this.ScreenX;
            Foup_.SizeY = this.SizeY;

            //foreach(WaferCls wafer in this.FoupWaferList)
            //{
            //    Foup_.FoupWaferList.Add(wafer.Clone());
            //}

            return Foup_;
        }
    }
}
