using System;
using System.Collections.Generic;
using System.IO;

namespace evosystem_backend
{
    public class FileManager
    {
        // Esta lista será usada por todas as varreduras
        private List<FileInfo> largeFiles;
        private long currentMinSizeInBytes;

        // 1. Executa uma varredura rápida nas pastas de conteúdo do usuário. (Fotos, Documentos, Músicas, etc.)
        public List<FileInfo> FindLargeFilesQuickScan(long minSizeInBytes)
        {
            largeFiles = new List<FileInfo>();
            currentMinSizeInBytes = minSizeInBytes;

            // Lista das pastas "seguras" para escanear
            List<string> foldersToScan = new List<string>
            {
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };

            foreach (string folderPath in foldersToScan)
            {
                // Verifica se a pasta existe antes de escanear
                if (Directory.Exists(folderPath))
                {
                    SearchDirectoryRecursively(new DirectoryInfo(folderPath));
                }
            }

            return largeFiles;
        }

        // 2. Executa uma varredura completa em um diretório inicial específico.
        public List<FileInfo> FindLargeFiles(string startDirectory, long minSizeInBytes)
        {
            largeFiles = new List<FileInfo>();
            currentMinSizeInBytes = minSizeInBytes;

            if (Directory.Exists(startDirectory))
            {
                SearchDirectoryRecursively(new DirectoryInfo(startDirectory));
            }

            return largeFiles;
        }


        // O motor de busca recursivo privado que as funções públicas usam.
        private void SearchDirectoryRecursively(DirectoryInfo currentDirectory)
        {
            // 1. Tenta verificar os arquivos na pasta ATUAL
            try
            {
                foreach (FileInfo file in currentDirectory.GetFiles())
                {
                    try
                    {
                        if (file.Length > currentMinSizeInBytes)
                        {
                            largeFiles.Add(file);
                        }
                    }
                    catch (FileNotFoundException) { }
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception) { }

            // 2. Tenta fazer o mesmo para TODAS as subpastas
            try
            {
                foreach (DirectoryInfo subDir in currentDirectory.GetDirectories())
                {
                    SearchDirectoryRecursively(subDir);
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception) { }
        }
    }
}