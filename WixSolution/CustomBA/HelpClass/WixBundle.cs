using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomBA.HelpClass
{
    public class WixBundle
    {
        private readonly Engine _engine;
        public WixBundle(Engine engine)
        {
            _engine = engine;
        }

        public void SetString(string variableName, string valueString)
        {
            try
            {
                _engine.StringVariables[variableName] = valueString;
            }
            catch (Exception ex)
            {
                _engine.Log(LogLevel.Verbose, string.Format("WixVariables.SetString: Unable to set string variable {0} to {1}, excerption: {2}", variableName, valueString, ex));
                throw;
            }
        }

        public string GetString(string variableName)
        {
            string result = string.Empty;
            try
            {
                if (_engine.StringVariables.Contains(variableName))
                {
                    result = _engine.StringVariables[variableName];
                }
            }
            catch (Exception ex)
            {
                _engine.Log(LogLevel.Verbose, string.Format("WixVariables.GetString: for: {0}; exception: {1}", variableName, ex));
                throw;
            }

            return result;
        }

        public Version GetCurrentBundleVersion(string variableName)
        {
            Version result = new Version(0, 0);
            try
            {
                if (_engine.VersionVariables.Contains(variableName))
                {
                    result = _engine.VersionVariables[variableName];
                }
            }
            catch
            {
                result = new Version(0, 0);
                _engine.Log(LogLevel.Verbose, "WixVariables.GetVersion: Returning 0 version for: " + variableName);
            }
            return result;
        }

    }
}
