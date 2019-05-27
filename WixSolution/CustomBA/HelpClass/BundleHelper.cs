using CustomBA.Models;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomBA.HelpClass
{
    public class BundleHelper
    {
        private static WixBundle wixBundle;

        public BundleHelper(Engine engine)
        {
            if (null == wixBundle)
            {
                wixBundle = new WixBundle(engine);
            }
        }

        private static Version GetInstalledBundleVersion()
        {
            Version result = new Version(0, 0, 0, 0);
            string versionStr = wixBundle.GetString("InstalledLanguagePackageVersion");
            Version.TryParse(versionStr, out result);
            return result;
        }

        private static string GetBundleProductName()
        {
            var result = wixBundle.GetString("ProductName");
            return result;
        }

        private static Version GetCurrentBundleVersion()
        {
            var result = wixBundle.GetCurrentBundleVersion("WixBundleVersion");
            return result;
        }

        public WixBundleModel GetCurrentBundleInfo()
        {
            var result = new WixBundleModel();
            result.ProductName = GetBundleProductName();
            //result.InstalledBundleVersion = GetInstalledBundleVersion();
            result.BundleVersion = GetCurrentBundleVersion();
            result.BundleName = wixBundle.GetString("WixBundleName");
            return result;
        }
    }
}
