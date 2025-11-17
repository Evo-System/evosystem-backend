using System;
using System.Collections.Generic;
using System.IO;
using evosystem_backend;

namespace EvoSystem.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Instanciamos todos os "gerenciadores" do backend uma vez
            SystemInfo info = new SystemInfo();
            StartupManager startup = new StartupManager();
            AppManager appManager = new AppManager();
            DriverManager driverManager = new DriverManager();
            WingetManager winget = new WingetManager();
            FileManager fileManager = new FileManager();
            long minBytes = 1024 * 1024 * 1024; // Para buscar arquivos grandes

            // 2. Loop infinito para manter o menu
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- MENU DE TESTE DO EvoSystem.Backend ---");
                Console.WriteLine("!!! IMPORTANTE: Execute como Administrador para ver todos os resultados !!!\n");

                Console.WriteLine("1. Testar SystemInfo (CPU, RAM, GPU, etc.)");
                Console.WriteLine("2. Testar StartupManager (Apps de Inicialização)");
                Console.WriteLine("3. Testar AppManager (Apps Instalados)");
                Console.WriteLine("4. Testar DriverManager (Drivers Instalados)");
                Console.WriteLine("5. Testar WingetManager (Apps Atualizáveis)");
                Console.WriteLine("6. Testar FileManager (Varredura Rápida - RECOMENDADO)");
                Console.WriteLine("7. Testar FileManager (Pasta Específica 'Documentos')");
                Console.WriteLine("0. Sair");
                Console.Write("\nEscolha uma opção: ");

                string escolha = Console.ReadLine();

                switch (escolha)
                {
                    case "1":
                        Console.WriteLine("\n--- Testando SystemInfo ---");
                        Console.WriteLine($"Sistema Operacional: {info.GetOperatingSystem()}");
                        Console.WriteLine($"CPU: {info.GetCpuName()}");
                        Console.WriteLine($"Placa de Vídeo: {info.GetGpuName()}");
                        Console.WriteLine($"Placa-Mãe: {info.GetMotherboardName()}");
                        Console.WriteLine($"Memória RAM: {info.GetTotalRam()}");
                        break;

                    case "2":
                        Console.WriteLine("\n--- Testando StartupManager ---");
                        List<StartupApp> startupApps = startup.GetStartupApps();
                        Console.WriteLine($"Total de apps encontrados: {startupApps.Count}");
                        foreach (var app in startupApps)
                        {
                            string local = app.IsFromCurrentUser ? " (Usuário Atual)" : " (Todos os Usuários)";
                            Console.WriteLine($"- {app.Name}{local}: {app.FilePath}");
                        }
                        break;

                    case "3":
                        Console.WriteLine("\n--- Testando AppManager ---");
                        List<InstalledApp> installedApps = appManager.GetInstalledApps();
                        Console.WriteLine($"Total de apps encontrados: {installedApps.Count}");
                        foreach (var app in installedApps)
                        {
                            Console.WriteLine($"- {app.DisplayName} (Instalado em: {app.InstallDate})");
                        }
                        break;

                    case "4":
                        Console.WriteLine("\n--- Testando DriverManager ---");
                        List<DriverInfo> drivers = driverManager.GetDrivers();
                        Console.WriteLine($"Total de drivers encontrados: {drivers.Count}");
                        foreach (var driver in drivers)
                        {
                            Console.WriteLine($"- {driver.DeviceName} (Versão: {driver.DriverVersion})");
                        }
                        break;

                    case "5":
                        Console.WriteLine("\n--- Testando WingetManager (pode demorar um pouco) ---");
                        List<string> upgradableApps = winget.ListUpgradableApps();
                        Console.WriteLine($"Total de apps atualizáveis: {upgradableApps.Count}");
                        if (upgradableApps.Count == 0)
                        {
                            Console.WriteLine("Nenhuma atualização encontrada.");
                        }
                        foreach (var appLine in upgradableApps)
                        {
                            Console.WriteLine($"- {appLine}");
                        }
                        break;

                    case "6":
                        Console.WriteLine("\n--- Testando FileManager (Varredura Rápida) ---");

                        List<FileInfo> quickScanFiles = fileManager.FindLargeFilesQuickScan(minBytes);
                        Console.WriteLine($"Total de arquivos > 1GB encontrados: {quickScanFiles.Count}");
                        foreach (var file in quickScanFiles)
                        {
                            double fileSizeGB = Math.Round(file.Length / (1024.0 * 1024.0 * 1024.0), 2);
                            Console.WriteLine($"- {file.FullName} ({fileSizeGB} GB)");
                        }
                        break;

                    case "7":
                        Console.WriteLine("\n--- Testando FileManager (buscando em 'Documentos') ---");
                        // Aqui no teste busca em documents
                        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                        List<FileInfo> largeFiles = fileManager.FindLargeFiles(documentsPath, minBytes);
                        Console.WriteLine($"Total de arquivos > 1GB encontrados: {largeFiles.Count}");
                        foreach (var file in largeFiles)
                        {
                            double fileSizeGB = Math.Round(file.Length / (1024.0 * 1024.0 * 1024.0), 2);
                            Console.WriteLine($"- {file.FullName} ({fileSizeGB} GB)");
                        }
                        break;

                    case "0":
                        Console.WriteLine("Saindo do testador...");
                        return;

                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }

                Console.WriteLine("\nPressione Enter para voltar ao menu...");
                Console.ReadLine();
            }
        }
    }
}