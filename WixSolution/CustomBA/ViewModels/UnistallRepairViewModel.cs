using CustomBA.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CustomBA.ViewModels
{
    public class UnistallRepairViewModel
    {
        private InstallViewModel installViewModel
        {
            get { return InstallViewModel.GetViewModel(); }
        }
        private BootstrapperApplicationModel BootstrapperModel
        {
            get
            { return BootstrapperApplicationModel.GetBootstrapperAppModel(); }
        }
        private DelegateCommand CloseCommand;
        private DelegateCommand UninstallCommand;
        public ICommand btn_close
        {
            get { return CloseCommand; }
        }
        public ICommand btn_uninstall
        {
            get { return UninstallCommand; }
        }
        
        public string UninstallInformation{ get; set; }

        public UnistallRepairViewModel()
        {
            UninstallCommand = new DelegateCommand(Uninstall);
            CloseCommand = new DelegateCommand(Close);
            InitialUIDisplay();
        }

        private void InitialUIDisplay()
        {
            this.UninstallInformation = "Setup will uninstall the current language package from your computer. Please click Uninstall to continue or Cancel to exit.";
        }

        public void Uninstall()
        {
            this.installViewModel.ApplySuccessText = "Uninstallation Successfully Completed.";
            BootstrapperModel.PlanAction(LaunchAction.Uninstall);
        }
        public void Close()
        {
             CustomBootstrapperApplication.Dispatcher.InvokeShutdown();
        }
    }
}
