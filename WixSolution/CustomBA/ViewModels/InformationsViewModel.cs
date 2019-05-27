using CustomBA.Models;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CustomBA.ViewModels
{
    public class InformationsViewModel : BaseViewModel
    {
        private string errorMessage = string.Empty;
        public InformationsViewModel()
        {
            CloseCommand = new DelegateCommand(Close, IsValid);
            WireUpEventHandlers();
        }

        private void WireUpEventHandlers()
        {
            this.BootstrapperModel.BootstrapperApplication.DetectComplete += BootstrapperApplication_DetectComplete;
        }

        private void BootstrapperApplication_DetectComplete(object sender, Microsoft.Tools.WindowsInstallerXml.Bootstrapper.DetectCompleteEventArgs e)
        {
            this.ErrorMessage = installViewModel.ErrorMessage;
        }

        
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

        private DelegateCommand CloseCommand;
        public ICommand btn_close
        {
            get { return CloseCommand; }
        }
        public BitmapSource BackImage
        {
            get
            {
                Bitmap bmp = CustomBA.Properties.Resources.logo;
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(),
                    IntPtr.Zero, System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));
            }
        }
        public void Close()
        {
            CustomBootstrapperApplication.Dispatcher.InvokeShutdown();
        }
        public bool IsValid()
        {
            return true;
        }
    }
}
