/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:SFE.TRACK"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
//using Microsoft.Practices.ServiceLocation;

namespace SFE.TRACK.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<Auto.AutoMainViewModel>();
            SimpleIoc.Default.Register<Auto.LotStartViewModel>();
            SimpleIoc.Default.Register<Auto.MachineLayOutViewModel>();
            SimpleIoc.Default.Register<Auto.CassetteLayOutViewModel>();
            SimpleIoc.Default.Register<Auto.SlotDetailViewModel>();
            SimpleIoc.Default.Register<Auto.JobStartViewModel>();
            SimpleIoc.Default.Register<Auto.CassetteSlotViewModel>();
            SimpleIoc.Default.Register<Auto.MotorInitialViewModel>();
            SimpleIoc.Default.Register<Auto.StopControlViewModel>();
            SimpleIoc.Default.Register<Auto.RecipeTransferViewModel>();
            SimpleIoc.Default.Register<Auto.RegistDummyLinkRecipeViewModel>();
            SimpleIoc.Default.Register<Auto.DataMonitoringViewModel>();
            SimpleIoc.Default.Register<Gem.GemMainViewModel>();
            SimpleIoc.Default.Register<Log.LogMainViewModel>();
            SimpleIoc.Default.Register<Maint.MaintMainViewModel>();
            SimpleIoc.Default.Register<Maint.MaintenanceModeViewModel>();
            SimpleIoc.Default.Register<Maint.UserRegistViewModel>();
            SimpleIoc.Default.Register<Maint.MaintSupportViewModel>();
            SimpleIoc.Default.Register<Maint.MaintChamberViewModel>();
            SimpleIoc.Default.Register<Maint.SelSupportDateViewModel>();
            SimpleIoc.Default.Register<Maint.EditMonitoringDataViewModel>();
            SimpleIoc.Default.Register<Motor.MotorMainViewModel>();
            SimpleIoc.Default.Register<Motor.IOControlViewModel>();
            SimpleIoc.Default.Register<Motor.MotorControlViewModel>();
            SimpleIoc.Default.Register<Param.ParamMainViewModel>();
            SimpleIoc.Default.Register<Util.UtilMainViewModel>();
            SimpleIoc.Default.Register<Util.DispenseConfigViewModel>();
            SimpleIoc.Default.Register<Util.RobotConfigViewModel>();
            SimpleIoc.Default.Register<Util.MachineConfigViewModel>();
            SimpleIoc.Default.Register<Util.CassetteConfigViewModel>();
            SimpleIoc.Default.Register<AlarmMessageViewModel>();
            SimpleIoc.Default.Register<Recipe.RecipeMainViewModel>();
            SimpleIoc.Default.Register<Recipe.WaferFlowRecipeViewModel>();            
            SimpleIoc.Default.Register<Recipe.SelectModuleViewModel>();
            SimpleIoc.Default.Register<Recipe.SystemRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.PumpRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.CoaterProcessRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.DevProcessRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.ADHProcessRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.LHPProcessRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.HHPProcessRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.CPLProcessRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.TCPProcessRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.DummyCondLinkRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.CleanCondRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.CoaterCleanRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.DevCleanRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.ADHDummySeqRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.CoaterDummySeqRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.DevDummySeqRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.ADHDummyCondRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.CoaterDummyCondRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.DevDummyCondRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.SelectRecipeViewModel>();
            SimpleIoc.Default.Register<Recipe.ArmPositionViewModel>();
            SimpleIoc.Default.Register<Recipe.SelectDispenseViewModel>();
            SimpleIoc.Default.Register<Account.UserAccountViewModel>();
            SimpleIoc.Default.Register<Account.AccountModifyViewModel>();
            SimpleIoc.Default.Register<Jog.JogControlViewModel>();
            SimpleIoc.Default.Register<Alarm.AlarmMainViewModel>();
            SimpleIoc.Default.Register<Language.LanguageViewModel>();
            SimpleIoc.Default.Register<Motion.MotionMainViewModel>();
            SimpleIoc.Default.Register<Motion.MotionPRAViewModel>();
            SimpleIoc.Default.Register<Motion.MotionCRAViewModel>();
            SimpleIoc.Default.Register<Motion.MotionIRAViewModel>();
        }

        public MainViewModel MainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        #region Account
        public Account.UserAccountViewModel UserAccountViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Account.UserAccountViewModel>(); }
        }
        public Account.AccountModifyViewModel AccountModifyViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Account.AccountModifyViewModel>(); }
        }
        #endregion

        #region Alarm
        public Alarm.AlarmMainViewModel AlarmMainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Alarm.AlarmMainViewModel>(); }
        }
        #endregion

        #region Auto
        public Auto.AutoMainViewModel AutoMainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Auto.AutoMainViewModel>(); }
        }        
        public Auto.MachineLayOutViewModel MachineLayOutViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Auto.MachineLayOutViewModel>(); }
        }
        public Auto.CassetteLayOutViewModel CassetteLayOutViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Auto.CassetteLayOutViewModel>(); }
        }
        public Auto.LotStartViewModel LotStartViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Auto.LotStartViewModel>(); }
        }
        public Auto.JobStartViewModel JobStartViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Auto.JobStartViewModel>(); }
        }
        public Auto.SlotDetailViewModel SlotDetailViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Auto.SlotDetailViewModel>(); }
        }
        public Auto.CassetteSlotViewModel CassetteSlotViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Auto.CassetteSlotViewModel>(); }
        }
        public Auto.MotorInitialViewModel MotorInitialViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Auto.MotorInitialViewModel>(); }
        }
        public Auto.StopControlViewModel StopControlViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Auto.StopControlViewModel>(); }
        }
        public Auto.RecipeTransferViewModel RecipeTransferViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Auto.RecipeTransferViewModel>(); }
        }
        public Auto.RegistDummyLinkRecipeViewModel RegistDummyLinkRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Auto.RegistDummyLinkRecipeViewModel>(); }
        }
        public Auto.DataMonitoringViewModel DataMonitoringViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Auto.DataMonitoringViewModel>(); }
        }
        #endregion

        #region Recipe
        public Recipe.RecipeMainViewModel RecipeMainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.RecipeMainViewModel>(); }
        }
        public Recipe.WaferFlowRecipeViewModel WaferFlowRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.WaferFlowRecipeViewModel>(); }
        }
        public Recipe.SelectModuleViewModel SelectModuleViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.SelectModuleViewModel>(); }
        }
        public Recipe.SystemRecipeViewModel SystemRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.SystemRecipeViewModel>(); }
        }
        public Recipe.PumpRecipeViewModel PumpRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.PumpRecipeViewModel>(); }
        }        
        public Recipe.CoaterProcessRecipeViewModel CoaterProcessRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.CoaterProcessRecipeViewModel>(); }
        }
        public Recipe.DevProcessRecipeViewModel DevProcessRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.DevProcessRecipeViewModel>(); }
        }
        public Recipe.ADHProcessRecipeViewModel ADHProcessRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.ADHProcessRecipeViewModel>(); }
        }
        public Recipe.LHPProcessRecipeViewModel LHPProcessRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.LHPProcessRecipeViewModel>(); }
        }
        public Recipe.HHPProcessRecipeViewModel HHPProcessRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.HHPProcessRecipeViewModel>(); }
        }
        public Recipe.CPLProcessRecipeViewModel CPLProcessRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.CPLProcessRecipeViewModel>(); }
        }
        public Recipe.TCPProcessRecipeViewModel TCPProcessRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.TCPProcessRecipeViewModel>(); }
        }
        public Recipe.DummyCondLinkRecipeViewModel DummyCondLinkRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.DummyCondLinkRecipeViewModel>(); }
        }        
        public Recipe.CleanCondRecipeViewModel CleanCondRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.CleanCondRecipeViewModel>(); }
        }
        public Recipe.CoaterCleanRecipeViewModel CoaterCleanRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.CoaterCleanRecipeViewModel>(); }
        }
        public Recipe.DevCleanRecipeViewModel DevCleanRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.DevCleanRecipeViewModel>(); }
        }
        public Recipe.ADHDummySeqRecipeViewModel ADHDummySeqRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.ADHDummySeqRecipeViewModel>(); }
        }
        public Recipe.CoaterDummySeqRecipeViewModel CoaterDummySeqRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.CoaterDummySeqRecipeViewModel>(); }
        }
        public Recipe.DevDummySeqRecipeViewModel DevDummySeqRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.DevDummySeqRecipeViewModel>(); }
        }
        public Recipe.ADHDummyCondRecipeViewModel ADHDummyCondRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.ADHDummyCondRecipeViewModel>(); }
        }
        public Recipe.CoaterDummyCondRecipeViewModel CoaterDummyCondRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.CoaterDummyCondRecipeViewModel>(); }
        }
        public Recipe.DevDummyCondRecipeViewModel DevDummyCondRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.DevDummyCondRecipeViewModel>(); }
        }
        public Recipe.SelectRecipeViewModel SelectRecipeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.SelectRecipeViewModel>(); }
        }
        public Recipe.ArmPositionViewModel ArmPositionViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.ArmPositionViewModel>(); }
        }
        public Recipe.SelectDispenseViewModel SelectDispenseViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Recipe.SelectDispenseViewModel>(); }
        }        
        #endregion

        #region Gem
        public Gem.GemMainViewModel GemMainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Gem.GemMainViewModel>(); }
        }
        #endregion

        #region Log
        public Log.LogMainViewModel LogMainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Log.LogMainViewModel>(); }
        }
        #endregion

        #region Maint
        public Maint.MaintMainViewModel MaintMainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Maint.MaintMainViewModel>(); }
        }
        public Maint.MaintenanceModeViewModel MaintenanceModeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Maint.MaintenanceModeViewModel>(); }
        }
        public Maint.UserRegistViewModel UserRegistViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Maint.UserRegistViewModel>(); }
        }
        public Maint.MaintSupportViewModel MaintSupportViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Maint.MaintSupportViewModel>(); }
        }
        public Maint.MaintChamberViewModel MaintChamberViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Maint.MaintChamberViewModel>(); }
        }
        public Maint.SelSupportDateViewModel SelSupportDateViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Maint.SelSupportDateViewModel>(); }
        }
        public Maint.EditMonitoringDataViewModel EditMonitoringDataViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Maint.EditMonitoringDataViewModel>(); }
        }
        #endregion

        #region Motor        
        public Motor.MotorMainViewModel MotorMainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Motor.MotorMainViewModel>(); }
        }
        public Motor.IOControlViewModel IOControlViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Motor.IOControlViewModel>(); }
        }
        public Motor.MotorControlViewModel MotorControlViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Motor.MotorControlViewModel>(); }
        }
        #endregion

        #region Motion
        public Motion.MotionMainViewModel MotionMainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Motion.MotionMainViewModel>(); }
        }
        public Motion.MotionPRAViewModel MotionPRAViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Motion.MotionPRAViewModel>(); }
        }
        public Motion.MotionIRAViewModel MotionIRAViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Motion.MotionIRAViewModel>(); }
        }
        public Motion.MotionCRAViewModel MotionCRAViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Motion.MotionCRAViewModel>(); }
        }
        #endregion

        #region Param
        public Param.ParamMainViewModel ParamMainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Param.ParamMainViewModel>(); }
        }
        #endregion

        #region Util
        public Util.UtilMainViewModel UtilMainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Util.UtilMainViewModel>(); }
        }
        public Util.DispenseConfigViewModel DispenseConfigViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Util.DispenseConfigViewModel>(); }
        }
        public Util.RobotConfigViewModel RobotConfigViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Util.RobotConfigViewModel>(); }
        }
        public Util.MachineConfigViewModel MachineConfigViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Util.MachineConfigViewModel>(); }
        }
        public Util.CassetteConfigViewModel CassetteConfigViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Util.CassetteConfigViewModel>(); }
        }
        #endregion

        #region Jog
        public Jog.JogControlViewModel JogControlViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Jog.JogControlViewModel>(); }
        }
        #endregion

        #region Lang
        public Language.LanguageViewModel LanguageViewModel
        {
            get { return ServiceLocator.Current.GetInstance<Language.LanguageViewModel>(); }
        }
        #endregion

        #region ETC
        public AlarmMessageViewModel AlarmMessageViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AlarmMessageViewModel>(); }
        }
        #endregion

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}