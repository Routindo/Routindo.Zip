using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Routindo.Contract.Services;

namespace Routindo.Plugins.Zip.Components
{
    public class ZipUtilities
    {
        private static readonly ILoggingService Logger;
        private static readonly object syncObj = new object();
        static ZipUtilities()
        {
            if (Logger != null)
                return;

            lock (syncObj)
            {
                if(Logger != null) 
                    return;
                Logger = ServicesContainer.ServicesProvider.GetLoggingService(nameof(ZipUtilities), typeof(ZipUtilities));
            }

        }
        public static bool AddFileToArchive(string archivePath, string file)
        {
            return AddFilesToArchive(archivePath, new List<string>() {file});
        }

        public static bool AddFilesToArchive(string archivePath, List<string> files)
        {
            try
            {
                if (files == null || !files.Any())
                {
                    return false;
                }

                using (var zipArchive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
                {
                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        zipArchive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
                return false;
            }
        }

        public static bool ExtractArchive(string archivePath, string outputPath, bool overrideFiles)
        {
            try
            {
                using (var zipArchive = ZipFile.Open(archivePath, ZipArchiveMode.Read))
                {
                    zipArchive.ExtractToDirectory(outputPath, overrideFiles);
                }

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
                return false;
            }
        }


    }
}
