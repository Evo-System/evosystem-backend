using System;
using System.Collections.Generic;
using System.Management;

namespace evosystem_backend
{
    // A UI vai receber
    public class DriverInfo
    {
        public string DeviceName { get; set; }
        public string DriverVersion { get; set; }
        public string Manufacturer { get; set; }
    }

    public class DriverManager
    {
        public List<DriverInfo> GetDrivers()
        {
            var driverList = new List<DriverInfo>();
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT DeviceName, DriverVersion, Manufacturer FROM Win32_PnPSignedDriver");

                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["DeviceName"] != null)
                    {
                        driverList.Add(new DriverInfo
                        {
                            DeviceName = obj["DeviceName"].ToString(),
                            DriverVersion = obj["DriverVersion"]?.ToString() ?? "N/A",
                            Manufacturer = obj["Manufacturer"]?.ToString() ?? "N/A"
                        });
                    }
                }
            }
            catch (Exception)
            {
                // Ignora erros
            }
            return driverList;
        }
    }
}