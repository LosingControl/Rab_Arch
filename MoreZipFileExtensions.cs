using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Rabota_Arch
{
    public static class MoreZipFileExtensions
    {

        public static ZipArchiveEntry CreateEntryFromFolder(this ZipArchive destination, string sourceFolderName, string entryName)
        {
            return CreateEntryFromFolder(destination, sourceFolderName, entryName, CompressionLevel.Fastest);
        }

        public static ZipArchiveEntry CreateEntryFromFolder(this ZipArchive destination, string sourceFolderName, string entryName, CompressionLevel compressionLevel)
        {
            string sourceFolderFullPath = Path.GetFullPath(sourceFolderName);
            string basePath = entryName + "/";

            var createdFolders = new HashSet<string>();

            var entry = destination.CreateEntry(basePath);
            createdFolders.Add(basePath);

            foreach (string dirFolder in Directory.EnumerateDirectories(sourceFolderName, "*.*", SearchOption.AllDirectories))
            {
                string dirFileFullPath = Path.GetFullPath(dirFolder);
                string relativePath = (basePath + dirFileFullPath.Replace(sourceFolderFullPath + Path.DirectorySeparatorChar, ""))
                    .Replace(Path.DirectorySeparatorChar, '/');
                string relativePathSlash = relativePath + "/";

                if (!createdFolders.Contains(relativePathSlash))
                {
                    destination.CreateEntry(relativePathSlash, compressionLevel);
                    createdFolders.Add(relativePathSlash);
                }
            }

            // тут
            foreach (string dirFile in Directory.EnumerateFiles(sourceFolderName, "*.*", SearchOption.AllDirectories))
            {
                DateTime m_DateLogs = DateTime.Now;

                string dirFileFullPath = Path.GetFullPath(dirFile);
                string relativePath = (basePath + dirFileFullPath.Replace(sourceFolderFullPath + Path.DirectorySeparatorChar, ""))
                    .Replace(Path.DirectorySeparatorChar, '/');

                FileInfo myfile = new FileInfo(dirFileFullPath);

                TimeSpan diff = m_DateLogs.Subtract(File.GetLastWriteTimeUtc(myfile.FullName));
                
                if (diff.TotalDays > 6.238)
                {
                    destination.CreateEntryFromFile(dirFile, relativePath, compressionLevel);
                }
            }

            return entry;
        }
    }
}
