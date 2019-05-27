using CustomBA.HelpClass;
using CustomBA.Models;
using CustomBA.Views;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

namespace CustomBA
{
    public class CustomBootstrapperApplication : BootstrapperApplication
    {
        private const string MutexName = "CustomBootstrapperApplication";

        static Mutex mutex;
        public static Dispatcher Dispatcher { get; set; }
        protected override void Run()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;

            var model = BootstrapperApplicationModel.GetBootstrapperAppModel(this);
            CheckForOnlyOneInstance(model);
            var view = new InstallView();

            model.SetWindowHandle(view);

            this.Engine.Detect();
            view.Show();
            Dispatcher.Run();
            this.Engine.Quit(model.FinalResult);
        }

        private void CheckForOnlyOneInstance(BootstrapperApplicationModel model)
        {
            if (model.BootstrapperApplication.Command.Display == Display.Full)
            {
                bool mutexCreated = false;
                mutex = new Mutex(true, "CustomBootstrapperApplication", out mutexCreated);
                if (!mutexCreated)
                {
                    MessageBox.Show("Another instance of this setup is already running. Please wait for the other instance to finish and then try again.",
                                    "Installer already running", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    model.LogMessage("Another instance of this setup is already running.");
                    Exit(model.FinalResult);
                }
            }
        }

        private void Exit(int actionResult)
        {

            if (mutex != null)
            {
                mutex.Close();
                mutex = null;
            }

            Engine.Quit(actionResult);
        }
    }
}
