using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace evosystem_backend
{
    public class WingetManager
    {
        // Esta função apenas LISTA os apps. Ela não atualiza nada.
        public List<string> ListUpgradableApps()
        {
            var upgradableApps = new List<string>();

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = "list --upgrade-available --accept-source-agreements",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8
            };

            try
            {
                using (Process process = Process.Start(psi))
                {
                    while (!process.StandardOutput.EndOfStream)
                    {
                        string line = process.StandardOutput.ReadLine();

                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("Nome") || line.StartsWith("----"))
                        {
                            continue;
                        }

                        // Adiciona na lista
                        upgradableApps.Add(line);
                    }
                    process.WaitForExit();
                }
            }
            catch (Exception)
            {
                // Se o winget falhar, apenas retorna uma lista vazia.
                // A UI pode tratar isso.
            }

            return upgradableApps;
        }
    }
}