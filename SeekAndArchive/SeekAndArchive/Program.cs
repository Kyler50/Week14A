using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace SeekAndArchive
{
    class Program
    {
            static List<FileInfo> _foundFiles;
            static List<FileSystemWatcher> _watchers;
            static List<DirectoryInfo> _archiveDirs;

            static void RecursiveSearch(List<FileInfo> foundFiles, string fileName, DirectoryInfo currentDirectory)
            {
            foreach (FileInfo file in currentDirectory.GetFiles())
            {
                if (file.Name == fileName)
                    foundFiles.Add(file);
            }
            foreach (DirectoryInfo dir in currentDirectory.GetDirectories())
            {
                RecursiveSearch(foundFiles, fileName, dir);
            }
        }

        static void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
                Console.WriteLine("{0} has been changed!", e.FullPath);

            FileSystemWatcher senderWatcher = (FileSystemWatcher)sender;
            int index = _watchers.IndexOf(senderWatcher, 0);
            ArchiveFile(_archiveDirs[index], _foundFiles[index]);
        }

        static void ArchiveFile(DirectoryInfo archiveDir, FileInfo fileToArchive)
        {
            FileStream input = fileToArchive.OpenRead();
            FileStream output = File.Create(archiveDir.FullName + @"\" + fileToArchive.Name + ".gz");

            GZipStream compressor = new GZipStream(output, CompressionMode.Compress);

            int b = input.ReadByte();

            while (b != -1)
            {
                compressor.WriteByte((byte)b);
                b = input.ReadByte();
            }

            compressor.Close();
            input.Close();
            output.Close();
        }

        static void Main(string[] args)
        {
                string fileName = args[0];
                string directoryName = args[1];
                _foundFiles = new List<FileInfo>();
                _watchers = new List<FileSystemWatcher>();
                _archiveDirs = new List<DirectoryInfo>();

                DirectoryInfo rootDir = new DirectoryInfo(directoryName);
                if (!rootDir.Exists)
                {
                    Console.WriteLine("The specified directory does not exist.");
                    return;
                }

                RecursiveSearch(_foundFiles, fileName, rootDir);

                Console.WriteLine("Found {0} files.", _foundFiles.Count);

                foreach (FileInfo file in _foundFiles)
                {
                    Console.WriteLine("{0}", file.FullName);
                }

                foreach (FileInfo file in _foundFiles)
                {
                    FileSystemWatcher newWatcher = new FileSystemWatcher(file.DirectoryName, file.Name);
                    newWatcher.Changed += new FileSystemEventHandler(WatcherChanged);
                    newWatcher.EnableRaisingEvents = true;
                    _watchers.Add(newWatcher);
                }

                for (int i = 0; i < _foundFiles.Count; i++)
                {
                    _archiveDirs.Add(Directory.CreateDirectory("archive" + i.ToString()));
                }


                Console.ReadKey();
            }
    }
}
