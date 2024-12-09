using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Recipe
{
    public class RecipeMainViewModel : ViewModelBase
    {
        bool isWaferRecipe = true;
        bool isSystemRecipe = false;
        bool isPumpRecipe = false;
        bool isCotProcessRecipe = false;
        bool isDevProcessRecipe = false;
        bool isAdhProcessRecipe = false;
        bool isLhpProcessRecipe = false;
        bool isHhpProcessRecipe = false;
        bool isCplProcessRecipe = false;
        bool isTcpProcessRecipe = false;
        bool isDummyCondLinkRecipe = false;
        bool isCleanCondRecipe = false;
        bool isCotCleanRecipe = false;
        bool isDevCleanRecipe = false;
        bool isAdhDummySeqRecipe = false;
        bool isCotDummySeqRecipe = false;
        bool isDevDummySeqRecipe = false;
        bool isAdhDummyCondRecipe = false;
        bool isCotDummyCondRecipe = false;
        bool isDevDummyCondRecipe = false;

        public bool IsWaferRecipe
        {
            get { return isWaferRecipe; }
            set { isWaferRecipe = value; RaisePropertyChanged("IsWaferRecipe"); }
        }
        public bool IsSystemRecipe
        {
            get { return isSystemRecipe; }
            set { isSystemRecipe = value; RaisePropertyChanged("IsSystemRecipe"); }
        }
        public bool IsPumpRecipe
        {
            get { return isPumpRecipe; }
            set { isPumpRecipe = value; RaisePropertyChanged("IsPumpRecipe"); }
        }
        public bool IsCotProcessRecipe
        {
            get { return isCotProcessRecipe; }
            set { isCotProcessRecipe = value; RaisePropertyChanged("IsCotProcessRecipe"); }
        }
        public bool IsDevProcessRecipe
        {
            get { return isDevProcessRecipe; }
            set { isDevProcessRecipe = value; RaisePropertyChanged("IsDevProcessRecipe"); }
        }
        public bool IsAdhProcessRecipe
        {
            get { return isAdhProcessRecipe; }
            set { isAdhProcessRecipe = value; RaisePropertyChanged("IsAdhProcessRecipe"); }
        }
        public bool IsLhpProcessRecipe
        {
            get { return isLhpProcessRecipe; }
            set { isLhpProcessRecipe = value; RaisePropertyChanged("IsLhpProcessRecipe"); }
        }
        public bool IsHhpProcessRecipe
        {
            get { return isHhpProcessRecipe; }
            set { isHhpProcessRecipe = value; RaisePropertyChanged("IsHhpProcessRecipe"); }
        }
        public bool IsCplProcessRecipe
        {
            get { return isCplProcessRecipe; }
            set { isCplProcessRecipe = value; RaisePropertyChanged("IsCplProcessRecipe"); }
        }
        public bool IsTcpProcessRecipe
        {
            get { return isTcpProcessRecipe; }
            set { isTcpProcessRecipe = value; RaisePropertyChanged("IsTcpProcessRecipe"); }
        }
        public bool IsDummyCondLinkRecipe
        {
            get { return isDummyCondLinkRecipe; }
            set { isDummyCondLinkRecipe = value; RaisePropertyChanged("IsDummyCondLinkRecipe"); }
        }
        public bool IsCleanCondRecipe
        {
            get { return isCleanCondRecipe; }
            set { isCleanCondRecipe = value; RaisePropertyChanged("IsCleanCondRecipe"); }
        }
        public bool IsCotCleanRecipe
        {
            get { return isCotCleanRecipe; }
            set { isCotCleanRecipe = value; RaisePropertyChanged("IsCotCleanRecipe"); }
        }
        public bool IsDevCleanRecipe
        {
            get { return isDevCleanRecipe; }
            set { isDevCleanRecipe = value; RaisePropertyChanged("IsDevCleanRecipe"); }
        }
        public bool IsAdhDummySeqRecipe
        {
            get { return isAdhDummySeqRecipe; }
            set { isAdhDummySeqRecipe = value; RaisePropertyChanged("IsAdhDummySeqRecipe"); }
        }
        public bool IsCotDummySeqRecipe
        {
            get { return isCotDummySeqRecipe; }
            set { isCotDummySeqRecipe = value; RaisePropertyChanged("IsCotDummySeqRecipe"); }
        }
        public bool IsDevDummySeqRecipe
        {
            get { return isDevDummySeqRecipe; }
            set { isDevDummySeqRecipe = value; RaisePropertyChanged("IsDevDummySeqRecipe"); }
        }
        public bool IsAdhDummyCondRecipe
        {
            get { return isAdhDummyCondRecipe; }
            set { isAdhDummyCondRecipe = value; RaisePropertyChanged("IsAdhDummyCondRecipe"); }
        }
        public bool IsCotDummyCondRecipe
        {
            get { return isCotDummyCondRecipe; }
            set { isCotDummyCondRecipe = value; RaisePropertyChanged("IsCotDummyCondRecipe"); }
        }
        public bool IsDevDummyCondRecipe
        {
            get { return isDevDummyCondRecipe; }
            set { isDevDummyCondRecipe = value; RaisePropertyChanged("IsDevDummyCondRecipe"); }
        }
    }
}
