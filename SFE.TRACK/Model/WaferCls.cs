using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;


namespace SFE.TRACK.Model
{
    public class WaferCls : ViewModelBase
    {
        int index = 0;
        float sizeX = 0;
        float sizeY = 0;
        float screenX = 0;
        float screenY = 0;
        string lotNo = string.Empty;
        string id = string.Empty;
        enWaferState waferState = enWaferState.WAFER_NONE;
        RecipeInfoCls recipe = new RecipeInfoCls();
        ProcessWaferDataCls waferData = new ProcessWaferDataCls();
        string display = string.Empty;
        int blockNo = 0;
        int moduleNo = 0;
        int bufferIndex = 0;
        Visibility isWafer = Visibility.Visible;
        System.Windows.Media.SolidColorBrush waferColor = new System.Windows.Media.SolidColorBrush();
        bool use = true;

        public int Index
        {
            get { return index; }
            set { index = value; RaisePropertyChanged("Index"); }
        }

        public string LotNo
        {
            get { return lotNo; }
            set { lotNo = value; RaisePropertyChanged("LotNo"); }
        }

        public string ID
        {
            get { return id; }
            set { id = value; RaisePropertyChanged("ID"); }
        }

        public enWaferState WaferState
        {
            get { return waferState; }
            set { waferState = value; SetWaferColor(WaferState); RaisePropertyChanged("WaferState"); }
        }

        public RecipeInfoCls Recipe
        {
            get { return recipe; }
            set { recipe = value; RaisePropertyChanged("Recipe"); }
        }

        public ProcessWaferDataCls WaferData
        {
            get { return waferData; }
            set { waferData = value; RaisePropertyChanged("WaferData"); }
        }

        public int BufferIndex
        {
            get { return bufferIndex; }
            set { bufferIndex = value; RaisePropertyChanged("BufferIndex"); }
        }

        public int BlockNo
        {
            get { return blockNo; }
            set { blockNo = value; RaisePropertyChanged("BlockNo"); }
        }

        public int ModuleNo
        {
            get { return moduleNo; }
            set { moduleNo = value; RaisePropertyChanged("ModuleNo"); }
        }

        public float SizeX
        {
            get { return sizeX; }
            set { sizeX = value; RaisePropertyChanged("SizeX"); }
        }

        public float SizeY
        {
            get { return sizeY; }
            set { sizeY = value; RaisePropertyChanged("SizeY"); }
        }

        public float ScreenX
        {
            get { return screenX; }
            set { screenX = value; RaisePropertyChanged("ScreenX"); }
        }

        public float ScreenY
        {
            get { return screenY; }
            set { screenY = value; RaisePropertyChanged("ScreenY"); }
        }

        public bool Use
        {
            get { return use; }
            set { use = value;
                if (!Use) WaferState = enWaferState.WAFER_NO_USE;
                RaisePropertyChanged("Use"); }
        }

        public string Diplay
        {
            get { return display; }
            set { display = value; RaisePropertyChanged("Diplay"); }
        }

        public System.Windows.Media.SolidColorBrush WaferColor
        {
            get { return waferColor; }
            set { waferColor = value; RaisePropertyChanged("WaferColor"); }
        }

        public Visibility IsWafer
        {
            get { return isWafer; }
            set { isWafer = value; RaisePropertyChanged("IsWafer"); }
        }

        protected void SetWaferColor(enWaferState state)
        {
            switch (state)
            {
                case enWaferState.WAFER_NONE:
                    WaferColor = System.Windows.Media.Brushes.DarkGray;
                    IsWafer = Visibility.Hidden;
                    break;
                case enWaferState.WAFER_NORMAL:
                    WaferColor = System.Windows.Media.Brushes.AntiqueWhite;
                    IsWafer = Visibility.Visible;
                    break;
                case enWaferState.WAFER_ERROR:
                    WaferColor = System.Windows.Media.Brushes.SandyBrown;
                    break;
                case enWaferState.WAFER_EXTRA:
                    WaferColor = System.Windows.Media.Brushes.MediumTurquoise;
                    break;
                case enWaferState.WAFER_ABORT:
                    WaferColor = System.Windows.Media.Brushes.LightCoral;
                    break;
                case enWaferState.WAFER_PILOT:
                    WaferColor = System.Windows.Media.Brushes.MediumSeaGreen;
                    break;
                case enWaferState.WAFER_CUPWASH:
                    WaferColor = System.Windows.Media.Brushes.DeepSkyBlue;
                    break;
                case enWaferState.WAFER_PROCESS_END:
                    WaferColor = System.Windows.Media.Brushes.Magenta;
                    break;
                case enWaferState.WAFER_PROCESS_NORMAL:
                    WaferColor = System.Windows.Media.Brushes.BurlyWood;
                    IsWafer = Visibility.Visible;
                    break;
                case enWaferState.WAFER_PROCESS:
                    WaferColor = System.Windows.Media.Brushes.LimeGreen;
                    IsWafer = Visibility.Visible;
                    break;
                case enWaferState.WAFER_EXIST:
                    WaferColor = System.Windows.Media.Brushes.LightCoral;
                    IsWafer = Visibility.Visible;
                    break;
                case enWaferState.WAFER_EMPTY:
                    WaferColor = System.Windows.Media.Brushes.DarkGray;
                    IsWafer = Visibility.Visible;
                    break;
                case enWaferState.WAFER_EXIST_PROCESS_END:
                    WaferColor = System.Windows.Media.Brushes.Blue;
                    IsWafer = Visibility.Visible;
                    break;
                case enWaferState.WAFER_NO_USE:
                    WaferColor = System.Windows.Media.Brushes.Crimson;
                    IsWafer = Visibility.Visible;
                    break;

            }
        }

        public WaferCls Clone()
        {
            WaferCls wafer_ = new WaferCls();
            wafer_.Index = this.Index;
            wafer_.SizeX = this.SizeX;
            wafer_.SizeY = this.SizeY;
            wafer_.ScreenX = this.ScreenX;
            wafer_.ScreenY = this.ScreenY;
            wafer_.LotNo = this.LotNo;
            wafer_.ID = this.ID;
            wafer_.WaferState = this.WaferState;
            wafer_.Recipe = this.Recipe;
            wafer_.BlockNo = this.BlockNo;
            wafer_.ModuleNo = this.ModuleNo;
            wafer_.waferColor = this.WaferColor;
            wafer_.Use = this.Use;
            return wafer_;
        }
    }
}
