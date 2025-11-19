using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace evosystem_backend
{
    // Modelo que a UI pode usar
    public class CleanupResult
    {
        public int FilesDeleted { get; set; }
        public long FreedBytes { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public string FreedMB => (FreedBytes / (1024.0 * 1024.0)).ToString("F2") + " MB";
    }

    public class CleanupManager
    {
        
        //  LIMPA TODOS OS TIPOS DE LIXO — MÉTODO “FULL CLEAN”
        
        public CleanupResult RunFullCleanup()
        {
            var total = new CleanupResult();

            total = Merge(total, CleanTempFolders());
            total = Merge(total, CleanWindowsTemp());
            total = Merge(total, CleanThumbnailCache());
            total = Merge(total, CleanPrefetch());
            total = Merge(total, CleanWindowsUpdateCache());
            total = Merge(total, CleanRecycleBin());

            return total;
        }

        // Mescla resultados de várias funções em 1 só
        private CleanupResult Merge(CleanupResult main, CleanupResult add)
        {
            main.FilesDeleted += add.FilesDeleted;
            main.FreedBytes += add.FreedBytes;
            main.Errors.AddRange(add.Errors);
            return main;
        }

        
        // 1) Limpeza: %TEMP% + Local AppData Temp
        
        public CleanupResult CleanTempFolders()
        {
            var result = new CleanupResult();

            string temp1 = Path.GetTempPath();
            string temp2 = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Temp"
            );

            result = Merge(result, DeleteFromDirectory(temp1));
            result = Merge(result, DeleteFromDirectory(temp2));

            return result;
        }

        
        // 2) Limpeza: C:\Windows\Temp  (pode exigir admin)
        
        public CleanupResult CleanWindowsTemp()
        {
            string path = @"C:\Windows\Temp";
            return DeleteFromDirectory(path);
        }

        
        // 3) Limpeza: Prefetch (C:\Windows\Prefetch)
        
        public CleanupResult CleanPrefetch()
        {
            string path = @"C:\Windows\Prefetch";
            return DeleteFromDirectory(path);
        }

        // 4) Limpeza: Cache de Thumbnails do Explorer
        
        public CleanupResult CleanThumbnailCache()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Microsoft\Windows\Explorer"
            );

            var result = new CleanupResult();

            try
            {
                if (!Directory.Exists(path)) return result;

                foreach (string thumbnailFile in Directory.GetFiles(path, "thumbcache*"))
                {
                    try
                    {
                        FileInfo fi = new FileInfo(thumbnailFile);
                        long size = fi.Length;
                        fi.Delete();

                        result.FilesDeleted++;
                        result.FreedBytes += size;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(thumbnailFile + " → " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add("Erro acessando a pasta de thumbnails: " + ex.Message);
            }

            return result;
        }


        // 5) Limpeza: Cache do Windows Update

        public CleanupResult CleanWindowsUpdateCache()
        {
            string path = @"C:\Windows\SoftwareDistribution\Download";
            return DeleteFromDirectory(path);
        }

        
        // 6) Limpar a Lixeira (Recycle Bin)
        [DllImport("shell32.dll")]
        private static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);

        private const uint SHERB_NOCONFIRMATION = 0x00000001;
        private const uint SHERB_NOPROGRESSUI = 0x00000002;
        private const uint SHERB_NOSOUND = 0x00000004;

        public CleanupResult CleanRecycleBin()
        {
            var result = new CleanupResult();

            try
            {
                // Limpa TODAS as unidades
                SHEmptyRecycleBin(IntPtr.Zero, null,
                    SHERB_NOCONFIRMATION |
                    SHERB_NOPROGRESSUI |
                    SHERB_NOSOUND
                );
            }
            catch (Exception ex)
            {
                result.Errors.Add("Erro ao limpar a Lixeira: " + ex.Message);
            }

            return result;
        }

       
        // FUNÇÃO BASE PARA APAGAR ARQUIVOS DE UMA PASTA
       
        private CleanupResult DeleteFromDirectory(string path)
        {
            var result = new CleanupResult();

            try
            {
                if (!Directory.Exists(path))
                    return result;

                // Apaga arquivos
                foreach (string file in Directory.GetFiles(path))
                {
                    try
                    {
                        FileInfo fi = new FileInfo(file);
                        long size = fi.Length;
                        fi.Delete();

                        result.FilesDeleted++;
                        result.FreedBytes += size;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(file + " → " + ex.Message);
                    }
                }

                // Apaga subpastas
                foreach (string dir in Directory.GetDirectories(path))
                {
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        long size = GetDirectorySize(di);

                        di.Delete(true);

                        result.FilesDeleted++; // conta como 1 item
                        result.FreedBytes += size;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(dir + " → " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add("Erro na pasta " + path + ": " + ex.Message);
            }

            return result;
        }

        // Soma o tamanho de uma pasta
        private long GetDirectorySize(DirectoryInfo dir)
        {
            long size = 0;

            try
            {
                foreach (FileInfo fi in dir.GetFiles("*", SearchOption.AllDirectories))
                {
                    size += fi.Length;
                }
            }
            catch { }

            return size;
        }
    }
}
