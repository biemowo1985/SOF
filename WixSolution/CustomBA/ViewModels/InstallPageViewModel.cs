using CustomBA.HelpClass;
using CustomBA.Models;
using CustomBA.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CustomBA.ViewModels
{
    public class InstallPageViewModel : BaseViewModel
    {
        private DelegateCommand InstallCommand;
        private DelegateCommand CloseCommand;

        private InstallViewModel installViewModel
        {
            get { return InstallViewModel.GetViewModel(); }
        }

        private BootstrapperApplicationModel BootstrapperModel
        {
            get
            {
                return BootstrapperApplicationModel.GetBootstrapperAppModel();
            }
        }
        public InstallPageViewModel()
        {
            InitialCommand();

            WireUpEventHandlers();
        }

        public string InstallButtonText { get; set; }

        public string InstallInformation { get; set; }


        //public Visibility SeleFileVisibility
        //{
        //    get { return selectFileVisibility; }
        //    set
        //    {
        //        selectFileVisibility = value;
        //        OnPropertyChanged("SeleFileVisibility");
        //    }
        //}

        //public bool CreateShortCut
        //{
        //    get { return createShortCut; }
        //    set
        //    {
        //        createShortCut = value;
        //        OnPropertyChanged("CreateShortCut");
        //        this.SetBurnVariable("CreateShortCut", createShortCut.ToString());
        //    }
        //}

        //public string InstallFolder
        //{
        //    get { return installFolder; }
        //    set
        //    {
        //        try {
        //            if (value != installFolder && ValidDir(value))
        //            {
        //                string[] para = value.Split('\\');
        //                bool hassoftwarename = false;
        //                foreach (string pa in para)
        //                {
        //                    if (pa == SoftWareName)
        //                        hassoftwarename = true;
        //                }
        //                if (hassoftwarename)
        //                    installFolder = value;
        //                else
        //                    installFolder = value + "\\" + SoftWareName;
        //                OnPropertyChanged("InstallFolder");
        //                this.SetBurnVariable("InstallFolder", installFolder);
        //            }
        //        }
        //        catch {
        //            installFolder = value;
        //        }
        //    }
        //}


        public ICommand btn_install
        {
            get { return InstallCommand; }
        }
        public ICommand btn_cancel
        {
            get { return CloseCommand; }
        }

        private void InitialCommand()
        {
            InstallCommand = new DelegateCommand(Install, IsValid);
            CloseCommand = new DelegateCommand(Close, IsValid);
        }

        private void InitialUIDisplay()
        {
            this.BootstrapperModel.LogMessage("----- Enter InstallPageViewModel-InitialUIDisplay Method -----");
            InstallButtonText = installViewModel.BundleAction;
            InstallInformation = GenerateInstallationInformation(installViewModel.ProductName, installViewModel.DisplayBundleVersion, installViewModel.BundleAction);
            this.BootstrapperModel.LogMessage("----- Exit InstallPageViewModel-InitialUIDisplay Method -----");
        }

        private string GenerateInstallationInformation(string productName, string productVersion, string action)
        {
            StringBuilder result = new StringBuilder();
            if ("Upgrade" == action)
            {
                result.Append("Setup will upgrade ")
                  .Append(productName)
                  .Append(" from ")
                  .Append("v" + installViewModel.InstalledBundleVersion.ToString())
                  .Append(" to v" + productVersion)
                  .Append(" on your computer. Please click ")
                  .Append(action)
                  .Append(" ")
                  .Append("to continue or Cancel to exit.");
            }
            else
            {
                result.Append("Setup will install ")
                  .Append(productName)
                  .Append(" ")
                  .Append("v" + productVersion)
                  .Append(" on your computer. Please click ")
                  .Append(action)
                  .Append(" ")
                  .Append("to continue or Cancel to exit.");
            }
            return result.ToString();
        }

        //public void Browse()
        //{
        //    var folderBrowserDialog = new FolderBrowserDialog { SelectedPath = InstallFolder };

        //    if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //    {
        //        InstallFolder = folderBrowserDialog.SelectedPath;
        //    }
        //}

        public void Install()
        {
            BootstrapperModel.LogMessage("----- Enter InstallPage Install Command -----");
            this.installViewModel.ApplySuccessText = "Installation Successfully Completed.";
            BootstrapperModel.LogMessage("InstallButtonText in InstallCommand: " + InstallButtonText);

            if (BootstrapperModel.BootstrapperApplication.Command.Action == LaunchAction.Uninstall &&
            (BootstrapperModel.BootstrapperApplication.Command.Display == Display.None || BootstrapperModel.BootstrapperApplication.Command.Display == Display.Embedded))
            {
                BootstrapperModel.LogMessage("CommandAction is: " + BootstrapperModel.BootstrapperApplication.Command.Action);
                this.BootstrapperModel.PlanAction(LaunchAction.Uninstall);
            }
            else
            {
                BootstrapperModel.LogMessage("CommandAction is: " + BootstrapperModel.BootstrapperApplication.Command.Action);
                this.BootstrapperModel.PlanAction(LaunchAction.Install);
            }
            BootstrapperModel.LogMessage("----- Exit InstallPage Install Command -----");
        }

        public void Close()
        {
            installViewModel.State = InstallState.Cancelled;
            CustomBootstrapperApplication.Dispatcher.InvokeShutdown();
        }

        //public void Show()
        //{
        //    if (SeleFileVisibility == Visibility.Collapsed)
        //        SeleFileVisibility = Visibility.Visible;
        //    else
        //        SeleFileVisibility = Visibility.Collapsed;
        //}

        protected void ApplyBegin(object sender, ApplyBeginEventArgs e)
        {
            this.installViewModel.State = InstallState.Applying;
        }

        protected void PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (installViewModel.State == InstallState.Cancelled)
            {
                CustomBootstrapperApplication.Dispatcher
                  .InvokeShutdown();
                return;
            }
            this.BootstrapperModel.ApplyAction();
        }


        private void WireUpEventHandlers()
        {
            this.BootstrapperModel.BootstrapperApplication.PlanComplete += this.PlanComplete;
            this.BootstrapperModel.BootstrapperApplication.ApplyBegin += this.ApplyBegin;
            this.BootstrapperModel.BootstrapperApplication.DetectComplete += BootstrapperApplication_DetectComplete;
        }

        private void BootstrapperApplication_DetectComplete(object sender, DetectCompleteEventArgs e)
        {
            this.BootstrapperModel.LogMessage("----- Enter InstallPage BootstrapperApplication_DetectComplete Event -----");
            this.InstallButtonText = installViewModel.BundleAction;
            InitialUIDisplay();
            this.BootstrapperModel.LogMessage("InstallButtonText: " + InstallButtonText);
            OnPropertyChanged("InstallButtonText");
            this.BootstrapperModel.LogMessage("----- Exit InstallPage BootstrapperApplication_DetectComplete Event -----");
        }

        public bool ValidDir(string path)
        {
            try
            {
                string p = new DirectoryInfo(path).FullName;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetBurnVariable(string variableName, string value)
        {
            this.BootstrapperModel.SetBurnVariable(variableName, value);
        }
        public bool IsValid()
        {
            return true;
        }
    }
}
