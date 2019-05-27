using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomBA.Models
{
    public class WixBundleModel
    {
        public Version BundleVersion { get; set; }
        public Version InstalledBundleVersion { get; set; }
        public string BundleName { get; set; }
        public string ProductName { get; set; }
        public string PlanAction { get; set; }
    }
}
