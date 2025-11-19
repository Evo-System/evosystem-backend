using System;
using System.Management;

namespace evosystem_backend
{
    public class SystemInfo
    {
        // Função "ajudante" privada
        private string GetWmiProperty(string wmiClass, string property)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT " + property + " FROM " + wmiClass);
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj[property] != null)
                    {
                        return obj[property].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                return "Erro: " + e.Message;
            }
            return "Não encontrado";
        }

        // --- FUNÇÕES PÚBLICAS QUE A UI VAI USAR ---

        public string GetOperatingSystem()
        {
            return GetWmiProperty("Win32_OperatingSystem", "Caption");
        }

        public string GetCpuName()
        {
            return GetWmiProperty("Win32_Processor", "Name");
        }

        public string GetMotherboardName()
        {
            return GetWmiProperty("Win32_BaseBoard", "Product");
        }

        public string GetGpuName()
        {
            string gpuName = "Não encontrado";
            try
            {
                ManagementObjectSearcher gpuSearcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                foreach (ManagementObject obj in gpuSearcher.Get())
                {
                    if (obj["Name"] != null)
                    {
                        string currentName = obj["Name"].ToString();
                        if (!currentName.Contains("Virtual") && !currentName.Contains("Meta") && !currentName.Contains("Remote"))
                        {
                            gpuName = currentName;
                            break;
                        }
                        if (gpuName == "Não encontrado")
                        {
                            gpuName = currentName;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                gpuName = "Erro: " + e.Message;
            }
            return gpuName;
        }

        public string GetTotalRam()
        {
            try
            {
                ManagementObjectSearcher ramSearcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in ramSearcher.Get())
                {
                    if (obj["TotalPhysicalMemory"] != null)
                    {
                        long totalBytes = Convert.ToInt64(obj["TotalPhysicalMemory"]);
                        double totalGB = Math.Round(totalBytes / (1024.0 * 1024.0 * 1024.0), 2);
                        return totalGB + " GB";
                    }
                }
            }
            catch (Exception e)
            {
                return "Erro ao consultar RAM: " + e.Message;
            }
            return "Não encontrado";
        }
    }
}