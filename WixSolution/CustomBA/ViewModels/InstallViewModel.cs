using CustomBA.HelpClass;
using CustomBA.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CustomBA.ViewModels
{
    public class InstallViewModel : BaseViewModel
    {
        private bool isRelatedBundlePresent = false;
        private static string MyInstellerName = "SetupProject";
        private InstallState state;
        private static InstallViewModel viewmodel;
        private InstallViewModel()
        {
            this.State = InstallState.Initializing;
            this.BundleAction = "Install";
            this.WireUpEventHandlers();
            InitBundleInformation(this.model.BootstrapperApplication.Engine);
        }
        public static InstallViewModel GetViewModel()
        {
            if (viewmodel == null)
                viewmodel = new InstallViewModel();
            return viewmodel;
        }
        private BootstrapperApplicationModel model
        {
            get
            {
                return BootstrapperApplicationModel.GetBootstrapperAppModel();
            }
        }

        public InstallState State
        {
            get
            {
                return this.state;
            }
            set
            {
                if (this.state != value)
                {
                    this.state = value;
                    OnPropertyChanged("State");
                    OnPropertyChanged("CancelEnabled");
                    OnPropertyChanged("InstallEnabled");
                    OnPropertyChanged("UninstallEnabled");
                    OnPropertyChanged("ProgressEnabled");
                    OnPropertyChanged("FinishEnabled");
                    OnPropertyChanged("ErrorEnabled");
                }

            }
        }

        public Version InstalledBundleVersion
        {
            get;
            set;
        }

        public Version CurrentBundleVersion
        {
            get;
            set;
        }

        public bool CancelEnabled
        {
            get
            {
                return State == InstallState.Cancelled;
            }
        }
        public bool InstallEnabled
        {
            get
            {
                return State == InstallState.NotPresent;
            }
        }

        public bool UninstallEnabled
        {
            get
            {
                return State == InstallState.Present;
            }
        }
        public bool ProgressEnabled
        {
            get
            {
                return State == InstallState.Applying;
            }
        }
        public bool FinishEnabled
        {
            get
            {
                return State == InstallState.Applied;
            }
        }

        public bool ErrorEnabled
        {
            get
            {
                return State == InstallState.Failed;
            }
        }
        private string errorMessage = string.Empty;
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }


        protected void DetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            model.BootstrapperApplication.Engine.Log(LogLevel.Standard, "----- Enter Detect Package Complete Event -----");
            if (e.PackageId.Equals(MyInstellerName, StringComparison.Ordinal))
            {
                model.LogMessage("InstallerName: " + MyInstellerName.ToString());
                this.State = e.State == PackageState.Present ? InstallState.Present : InstallState.NotPresent;
                model.LogMessage("e.State: " + e.State);
                model.LogMessage("This.State: " + this.State);
            }
            model.LogMessage("----- Exit Detect Package Complete Event -----");
        }

        private void WireUpEventHandlers()
        {
            this.model.BootstrapperApplication.DetectPackageComplete += this.DetectPackageComplete;
            this.model.BootstrapperApplication.DetectComplete += BootstrapperApplication_DetectComplete;
            this.model.BootstrapperApplication.Error += BootstrapperApplication_Error;
            this.model.BootstrapperApplication.DetectRelatedBundle += BootstrapperApplication_DetectRelatedBundle;
            this.model.BootstrapperApplication.PlanRelatedBundle += BootstrapperApplication_PlanRelatedBundle;
            this.model.BootstrapperApplication.ApplyComplete += BootstrapperApplication_ApplyComplete;
        }

        private void BootstrapperApplication_ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            if (this.model.BootstrapperApplication.Command.Action == LaunchAction.Uninstall && isRelatedBundlePresent) // this will be called in case of Upgrade of the bundle
            {
                CustomBootstrapperApplication.Dispatcher.InvokeShutdown();
            }
        }

        private void BootstrapperApplication_PlanRelatedBundle(object sender, PlanRelatedBundleEventArgs e)
        {
            model.LogMessage("-----Enter PlanRelatedBundle-----");
            model.LogMessage("BundleId: " + e.BundleId);
            model.LogMessage("Result: " + e.Result);
            model.LogMessage("State: " + e.State);
            model.LogMessage("CommandAction: " + model.BootstrapperApplication.Command.Action);
            model.LogMessage("-----Exit PlanRelatedBundle-----");
        }

        private void BootstrapperApplication_DetectRelatedBundle(object sender, DetectRelatedBundleEventArgs e)
        {
            model.LogMessage("----- Enter DetectRelatedBundle Event -----");
            this.InstalledBundleVersion = e.Version;
            if (e.RelationType == RelationType.Upgrade && InstalledBundleVersion < CurrentBundleVersion)
            {
                this.BundleAction = "Upgrade";
                this.isRelatedBundlePresent = true;
            }
            model.LogMessage("BundleTag: " + e.BundleTag);
            model.LogMessage("Operation: " + e.Operation);
            model.LogMessage("PerMachine: " + e.PerMachine);
            model.LogMessage("ProductCode: " + e.ProductCode);
            model.LogMessage("RelationType: " + e.RelationType);
            model.LogMessage("Result: " + e.Result);
            model.LogMessage("Version: " + e.Version);
            model.LogMessage("----- Exit DetectRelatedBundle Event -----");
        }

        private void BootstrapperApplication_Error(object sender, Microsoft.Tools.WindowsInstallerXml.Bootstrapper.ErrorEventArgs e)
        {
            model.LogMessage("----- Enter Error Event -----");
            model.LogMessage("e.Data ----- " + e.Data);
            model.LogMessage("e.ErrorCode ----- " + e.ErrorCode);
            model.LogMessage("e.ErrorMessage ----- " + e.ErrorMessage);
            model.LogMessage("e.ErrorType ----- " + e.ErrorType);
            model.LogMessage("e.PackageId ----- " + e.PackageId);
            model.LogMessage("e.Result ----- " + e.Result);
            model.LogMessage("e.UIHint ----- " + e.UIHint);
            this.State = InstallState.Failed;
            model.LogMessage("Error: " + e.ErrorMessage);
            this.errorMessage = "Error: " + e.ErrorMessage;
            model.LogMessage("----- Exit Error Event -----");
            CustomBootstrapperApplication.Dispatcher.InvokeShutdown();
        }

        private void BootstrapperApplication_DetectComplete(object sender, DetectCompleteEventArgs e)
        {
            model.LogMessage("----- Enter Detect Complete Event -----");
            model.LogMessage("e.Status: " + e.Status);
            model.LogMessage("DisplayLevel: " + model.BootstrapperApplication.Command.Display);

            //if (!CheckPreconditionProductInstalled())
            //{
            //    this.State = InstallState.Failed;
            //    this.errorMessage = "AccessCenter related product should be installed firstly.";
            //}
            //string errorMsg;
            //if (!CheckVersionAllowInstalled(out errorMsg))
            //{
            //    this.State = InstallState.Failed;
            //    this.errorMessage = errorMsg;
            //}

            model.LogMessage("State: " + State);
            model.LogMessage("InstalledBundleVersion: " + InstalledBundleVersion);
            model.LogMessage("CurrentBundleVersion: " + CurrentBundleVersion);
            model.LogMessage("BundleAction: " + BundleAction);
            model.LogMessage("----- Exit Detect Complete Event -----");
        }

        private bool CheckPreconditionProductInstalled()
        {
            return true;
        }

        private bool CheckVersionAllowInstalled(out string errorMessage)
        {
            if (CurrentBundleVersion == InstalledBundleVersion)
            {
                errorMessage = "The same version has been installed.";
                return false;
            }
            else if (CurrentBundleVersion < InstalledBundleVersion)
            {
                errorMessage = "A newer version has been installed. Downgrade is not allowed.";
                return false;
            }
            else
            {
                errorMessage = string.Empty;
                return true;
            }
        }

        public string ProductName { get; set; }

        public string DisplayBundleVersion { get; set; }

        public string DisplayVersion { get; set; }

        public string WindowTitle { get; set; }

        public string BundleAction { get; set; }

        public bool BundleInstalled
        {
            get
            {
                return model.GetString("WixBundleInstalled") == "1";
            }
        }

        private void InitBundleInformation(Engine engine)
        {
            model.LogMessage("----- Enter InitBundleInformation Method -----");
            BundleHelper bundle = new BundleHelper(engine);
            var bundleModel = bundle.GetCurrentBundleInfo();
            this.ProductName = bundleModel.ProductName;
            this.CurrentBundleVersion = bundleModel.BundleVersion;
            this.InstalledBundleVersion = bundleModel.InstalledBundleVersion;
            this.DisplayBundleVersion = CurrentBundleVersion.ToString();
            this.WindowTitle = bundleModel.BundleName;
            this.DisplayVersion = "Version: " + DisplayBundleVersion;
            model.LogMessage("----- Exit InitBundleInformation Method -----");
        }

        public BitmapSource LogoImage
        {
            get
            {
                Bitmap bmp = CustomBA.Properties.Resources.logo;
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(),
                    IntPtr.Zero, System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));
            }
        }

        public string ApplySuccessText
        {
            get;
            set;
        }
    }
}