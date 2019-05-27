using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomActions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CopyInstallFilesToServerFolderForStandAloneMode(Session session)
        {
            try
            {
                var productVersion = session.CustomActionData["PRODUCTVERSION"].Trim('"');
                var installFolder = session.CustomActionData["INSTALLFOLDERS"].Trim('"');
            }
            catch (Exception ex)
            {
                session.Log(ex.ToString());
            }

            return ActionResult.Success;
        }
    }
}
