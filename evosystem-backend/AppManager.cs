using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace evosystem_backend
{
    // A UI vai receber
    public class InstalledApp
    {
        public string DisplayName { get; set; }
        public string InstallDate { get; set; }
    }

    public class AppManager
    {
        public List<InstalledApp> GetInstalledApps()
        {
            var appList = new List<InstalledApp>();
            string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            // Tenta ler de HKEY_LOCAL_MACHINE (requer admin para ver tudo)
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(uninstallKey))
                {
                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        using (RegistryKey subKey = key.OpenSubKey(subKeyName))
                        {
                            string displayName = subKey.GetValue("DisplayName") as string;
                            string installDate = subKey.GetValue("InstallDate") as string;

                            if (!string.IsNullOrEmpty(displayName))
                            {
                                appList.Add(new InstalledApp
                                {
                                    DisplayName = displayName,
                                    InstallDate = installDate ?? "N/A"
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Ignora erros
            }

            return appList;
        }
    }
}