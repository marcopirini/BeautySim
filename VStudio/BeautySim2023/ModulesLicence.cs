using Microsoft.Win32;
using System.Reflection;
using Microsoft.VisualBasic.Devices;
using System;
using System.Net;
using System.Text;


namespace BeautySim2023
{
    public class ModulesLicence
    {
        private static RegistryKey ROOT_KEY = Registry.CurrentUser;
        private static string KEY_APPLICATION_PATH = $"SOFTWARE\\{Assembly.GetExecutingAssembly().GetName().Name}";
        private static string KEY_MODULES_PATH = "Modules";

        public void UpdateModulesLicence(bool btyConnected, string serial)
        {
            try
            {
                bool[] modulesActivation = CheckModulesOnServer(btyConnected, serial);

                if (btyConnected)
                    SaveModulesLicence(serial, modulesActivation);
            }
            catch(Exception)
            {

            }
        }

        private bool[] CheckModulesOnServer(bool bksConnected, string serial)
        {
            string[] modulesNames = Enum.GetNames(typeof(Enum_Modules));
            bool[] modulesActivation = new bool[modulesNames.Length];

            string osVersion = new ComputerInfo().OSFullName;
            if (Environment.Is64BitOperatingSystem)
                osVersion = string.Format("{0} (64 bit)", osVersion);

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            string productVersion = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);

            WebClient client = new WebClient();
            string url = "http://deimos.accuratesolutions.it/BeautySim/check-modules.php?serial=" + serial +
                "&os=" + osVersion +
                "&version=" + productVersion;

            byte[] html = client.DownloadData(url);

            UTF8Encoding utf = new UTF8Encoding();
            string mystring = utf.GetString(html);

            string[] splittedResult = mystring.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (!ValidateServerResponse(splittedResult))
            {
                throw new Exception("Bad response from license server.");
            }

            for (int i = 0; i < modulesActivation.Length; i++)
            {
                modulesActivation[i] = false;

                if (splittedResult.Length > i && splittedResult[i] == "1")
                    modulesActivation[i] = true;
            }

            return modulesActivation;
        }

        private bool ValidateServerResponse(string[] responseWords)
        {
            bool isValid = false;
            bool badValuePresent = false;

            for (int i = 0; i < responseWords.Length; i++)
            {
                if (responseWords[i] != "0" && responseWords[i] != "1")
                    badValuePresent = true;
            }

            if (responseWords.Length == Enum.GetValues(typeof(Enum_Modules)).Length && !badValuePresent)
            {
                isValid = true;
            }

            return isValid;
        }

        public bool CheckModuleOnRegistry(bool bksConnected, string serial, string moduleName)
        {
            bool isModuleEnabled = false;

            try
            {
                if (bksConnected)
                {
                    using (RegistryKey moduleRegKey = OpenRegistryKey(moduleName, serial, null, string.Empty, false))
                    {
                        if (moduleRegKey != null  && moduleRegKey.GetValue(moduleName) != null)
                        {
                            string moduleRegKeyValue = GetRegistryKeyString(moduleName, serial);

                            if (moduleRegKeyValue == "1")
                                isModuleEnabled = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                isModuleEnabled = false;
            }

            return isModuleEnabled;
        }

        private RegistryKey OpenRegistryKey(string name, string serial, RegistryKey rootKey, string keyPathName, bool save)
        {
            if (rootKey == null)
            {
                rootKey = ROOT_KEY;
            }

            if (string.IsNullOrEmpty(keyPathName))
            {
                keyPathName = $"{KEY_APPLICATION_PATH}\\{serial}\\{KEY_MODULES_PATH}";
            }

            RegistryKey regKey = rootKey.OpenSubKey(keyPathName, true);
            if (regKey == null && save)
                regKey = rootKey.CreateSubKey(keyPathName);

            return regKey;
        }

        private string GetRegistryKeyString(string name, string serial, RegistryKey rootKey = null, string keyPathName = "")
        {
            RegistryKey regKey = OpenRegistryKey(name, serial, rootKey, keyPathName, false);
            string value = string.Empty;

            if (regKey != null)
            {
                value = Encryption.Decrypt(regKey.GetValue(name).ToString(), "");
                regKey.Close();
            }

            return value;
        }

        private void SaveModulesLicence(string serial, bool[] modulesActivation)
        {
            try
            {
                string[] modulesNames = Enum.GetNames(typeof(Enum_Modules));
                string[] regKeyNewValues = new string[modulesNames.Length];

                for (int i = 0; i < regKeyNewValues.Length; i++)
                {
                    regKeyNewValues[i] = "0";

                    if (modulesActivation[i])
                        regKeyNewValues[i] = "1";

                    SaveRegistryKeyString(modulesNames[i], serial, regKeyNewValues[i]);
                }
            }
            catch (Exception)
            {

            }
        }

        private void SaveRegistryKeyString(string name, string serial, string value, RegistryKey rootKey = null, string keyPathName = "")
        {
            RegistryKey regKey = OpenRegistryKey(name, serial, rootKey, keyPathName, true);

            if (regKey != null)
            {
                string encryptedValue = Encryption.Encrypt(value, "");
                regKey.SetValue(name, encryptedValue, RegistryValueKind.String);
                regKey.Close();
            }
        }
    }
}
