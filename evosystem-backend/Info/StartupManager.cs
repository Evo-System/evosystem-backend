using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace evosystem_backend
{
    public class StartupApp
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public bool IsFromCurrentUser { get; set; } // Para a UI saber de onde veio
    }

    public class StartupManager
    {
        public List<StartupApp> GetStartupApps()
        {
            var appList = new List<StartupApp>();

            // Caminho 1: HKEY_CURRENT_USER (Apps do usuário logado)
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false))
                {
                    foreach (string appName in key.GetValueNames())
                    {
                        appList.Add(new StartupApp
                        {
                            Name = appName,
                            FilePath = key.GetValue(appName).ToString(),
                            IsFromCurrentUser = true
                        });
                    }
                }
            }
            catch (Exception)
            {
                // Ignora erros (ex: permissão)
            }

            // Caminho 2: HKEY_LOCAL_MACHINE (Apps de todos os usuários)
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false))
                {
                    foreach (string appName in key.GetValueNames())
                    {
                        appList.Add(new StartupApp
                        {
                            Name = appName,
                            FilePath = key.GetValue(appName).ToString(),
                            IsFromCurrentUser = false
                        });
                    }
                }
            }
            catch (Exception)
            {
                // Ignora erros (ex: requer admin)
            }

            return appList;
        }
    }
}