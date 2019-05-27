using CustomBA.Models;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CustomBA.ViewModels
{
    public class FinishViewModel : BaseViewModel
    {
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
        public FinishViewModel()
        {
            CloseCommand = new DelegateCommand(Close, IsValid);
            this.BootstrapperModel.BootstrapperApplication.ApplyComplete += BootstrapperApplication_ApplyComplete;
        }

        private void BootstrapperApplication_ApplyComplete(object sender, Microsoft.Tools.WindowsInstallerXml.Bootstrapper.ApplyCompleteEventArgs e)
        {
            this.ApplySuccessText = this.installViewModel.ApplySuccessText;
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

        private string applySuccessText = string.Empty;
        public string ApplySuccessText
        {
            get
            {
                return applySuccessText;
            }
            set
            {
                applySuccessText = value;
                OnPropertyChanged("ApplySuccessText");
            }
        }
    }
}
