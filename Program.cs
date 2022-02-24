using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Rabota_Arch
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string m_PathTargetFolder = Environment.CurrentDirectory;

            DateTime m_DateLogs = DateTime.Now;
            int day = 30;
            try
            {
                string m_NameFolderLogs = m_DateLogs.ToShortDateString() + " Text logs";
                

                DeleteOldZip(m_DateLogs, m_PathTargetFolder, day);

                ArchivingDirectories(m_DateLogs, m_PathTargetFolder);

                ArchivingFiles(m_PathTargetFolder, m_DateLogs);
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                Console.ReadKey();
            }
        }

        //передалать параметры, проверять не дир а файлы в дир, файл эксист если есть , то делать архив(1)
        private static void ArchivingDirectories(DateTime m_DateLogs, string m_PathTargetFolder)
        {
            string zipName = $"{m_DateLogs.ToShortDateString()}.zip";
            string[] dirs = Directory.GetDirectories(m_PathTargetFolder);

            if (!File.Exists(zipName))
            {
                using (ZipArchive archive = ZipFile.Open(zipName, ZipArchiveMode.Create))
                {
                    foreach (var dir in dirs)
                    {
                        TimeSpan diff = m_DateLogs.Subtract(Directory.GetLastAccessTime(dir.ToString()));

                        DirectoryInfo di = new DirectoryInfo(dir);

                        if (Directory.Exists(dir) && diff.TotalDays > 0)
                        {
                            archive.CreateEntryFromFolder(di.FullName, di.Name, CompressionLevel.Optimal);

                            //Directory.Delete(dir, true);
                        }
                    }
                }
            }
            else
            {
                zipName = $"{m_DateLogs.ToLongDateString()}.zip";

                using (ZipArchive archive = ZipFile.Open(zipName, ZipArchiveMode.Create))
                {
                    foreach (var dir in dirs)
                    {
                        TimeSpan diff = m_DateLogs.Subtract(Directory.GetLastAccessTime(dir.ToString()));

                        DirectoryInfo di = new DirectoryInfo(dir);

                        if (Directory.Exists(dir) && diff.TotalDays > 0)
                        {
                            archive.CreateEntryFromFolder(di.FullName, di.Name, CompressionLevel.Optimal);

                            //Directory.Delete(dir, true);
                        }
                    }
                }
            }
        }

        // посмотреть код с гит, сделать там отсечку не правильных файлов 
        private static void ArchivingFiles(string m_PathTargetFolder, DateTime m_DateLogs)
        {
            string zipName = $"{m_DateLogs.ToShortDateString()}.zip";
            string[] files = Directory.GetFiles(m_PathTargetFolder, "*.txt");

            using (ZipArchive archive = ZipFile.Open(zipName, ZipArchiveMode.Update))
            {
                foreach (var fileToArchive in files)
                {
                    TimeSpan diff = m_DateLogs.Subtract(File.GetLastAccessTime(fileToArchive.ToString()));

                    if (File.Exists(fileToArchive) && diff.TotalDays > 0)
                    {
                        archive.CreateEntryFromFile(fileToArchive, Path.GetFileName(fileToArchive), CompressionLevel.Optimal);

                        File.Delete(fileToArchive);
                    }
                }
            }
        }

        // добавить везде трай кетч
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_DateLogs"></param>
        /// <param name="zipDirs"></param>
        private static void DeleteOldZip(DateTime m_DateLogs, string m_PathTargetFolder, int day)
        {
            try
            {
                string[] zipDirs = Directory.GetFiles(m_PathTargetFolder, "*.zip");

                foreach (var zipDir in zipDirs)
                {
                    TimeSpan diff = m_DateLogs.Subtract(File.GetCreationTime(zipDir));

                    if (diff.TotalDays > day)
                    {
                        File.Delete(zipDir);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            
        }
    }
}
