﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Auto
{
    public class RecipeTransferViewModel : ViewModelBase
    {
        string recipeName = string.Empty;
        public RelayCommand RecipeSeletedRelayCommand { get; set; }
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }
        string recipeBackUp = string.Empty;
        public RecipeTransferViewModel()
        {
            RecipeSeletedRelayCommand = new RelayCommand(RecipeSeletedCommand);
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            RecipeName = Properties.Settings.Default.RECIPE_TRANSFER;
            recipeBackUp = RecipeName;
        }

        public string RecipeName
        {
            get { return recipeName; }
            set { recipeName = value; RaisePropertyChanged("RecipeName"); }
        }

        private void RecipeSeletedCommand()
        {
            if(Global.RecipeOpen(enRecipeMenu.WAFER_FLOW, RecipeName))
            {
                RecipeName = Global.STRecipePopUp.SelectRecipeName;
            }
        }

        private void OKCommand(Window window)
        {
            Properties.Settings.Default.RECIPE_TRANSFER = RecipeName;
            Properties.Settings.Default.Save();
            //챔버쪽에 알려야 한다.
            window.DialogResult = true;
        }
        private void CancelCommand(Window window)
        {
            RecipeName = recipeBackUp;
            window.DialogResult = false;
        }
    }
}