using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Plugins.Zip.Components;

namespace Routindo.Plugins.Zip.Tests.I
{
    [TestClass]
    public class ZipUtilitiesTests
    {
        [TestMethod]
        public void AddSingleFileToExistingZipSuccessTest()
        {
            string ticks =  $"{DateTime.Now.Ticks}";
            string directoryName = $"dir{ticks}";
            string directoryPath = Path.Combine(Path.GetTempPath(), directoryName);
            string zipPath = $"{directoryPath}.zip";

            // Create empty directory 
            Directory.CreateDirectory(directoryPath);
            Assert.IsTrue(Directory.Exists(directoryPath));

            // Zip empty directory
            ZipFile.CreateFromDirectory(directoryPath, zipPath);
            Assert.IsTrue(File.Exists(zipPath));

            // Create file with some data
            string fileName = $"file{ticks}.txt";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            File.WriteAllText(filePath, $"{this.GetType()}");
            Assert.IsTrue(File.Exists(filePath));

            // Get initial zip size 
            FileInfo fileInfo = new FileInfo(zipPath);
            var initialZipSize = fileInfo.Length;

            // Add the file to the zip
            bool result = ZipUtilities.AddFilesToArchive(zipPath, new List<string>() {filePath});
            Assert.IsTrue(result);

            // Get new zip size
            fileInfo.Refresh();
            var newZipSize = fileInfo.Length;

            // Expect new size is bigger than initial zip size
            Assert.IsTrue(newZipSize > initialZipSize);

            // Cleanup
            Directory.Delete(directoryPath);
            File.Delete(filePath);
            File.Delete(zipPath);
        }

        [TestMethod]
        public void AddSingleFileToNonExistingZipSuccessTest()
        { 
            string ticks = $"{DateTime.Now.Ticks}";
            string zipName = $"arc{ticks}.zip";
            string zipPath = $"{Path.Combine(Path.GetTempPath(), zipName)}";

            // Create file with some data
            string fileName = $"file{ticks}.txt";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            File.WriteAllText(filePath, $"{this.GetType()}");
            Assert.IsTrue(File.Exists(filePath));

            // Add the file to the zip
            bool result = ZipUtilities.AddFilesToArchive(zipPath, new List<string>() {filePath});
            Assert.IsTrue(result);

            Assert.IsTrue(File.Exists(zipPath));

            // Cleanup
            File.Delete(filePath);
            File.Delete(zipPath);
        }

        [TestMethod]
        public void ExtractArchiveToDirectoryTest() 
        {
            string ticks = $"{DateTime.Now.Ticks}";
            string directoryName = $"dir{ticks}";
            string directoryPath = Path.Combine(Path.GetTempPath(), directoryName);
            string zipPath = $"{directoryPath}.zip";

            // Create empty directory 
            Directory.CreateDirectory(directoryPath);
            Assert.IsTrue(Directory.Exists(directoryPath));

            // Zip empty directory
            ZipFile.CreateFromDirectory(directoryPath, zipPath);
            Assert.IsTrue(File.Exists(zipPath));

            // Create file with some data
            string fileName = $"file{ticks}.txt";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            File.WriteAllText(filePath, $"{this.GetType()}");
            Assert.IsTrue(File.Exists(filePath));

            // Get initial zip size 
            FileInfo fileInfo = new FileInfo(zipPath);
            var initialZipSize = fileInfo.Length;

            // Add the file to the zip
            bool result = ZipUtilities.AddFilesToArchive(zipPath, new List<string>() {filePath});
            Assert.IsTrue(result);

            // Get new zip size
            fileInfo.Refresh();
            var newZipSize = fileInfo.Length;

            // Expect new size is bigger than initial zip size
            Assert.IsTrue(newZipSize > initialZipSize);

            string outputDir = Path.Combine(Path.GetTempPath(), $"output{ticks}");
            var extractionResult =  ZipUtilities.ExtractArchive(zipPath, outputDir, false);
            Assert.IsTrue(extractionResult);
            Assert.IsTrue(Directory.Exists(outputDir));

            extractionResult = ZipUtilities.ExtractArchive(zipPath, outputDir, true);
            Assert.IsTrue(extractionResult);

            // Cleanup
            Directory.Delete(outputDir, true);
            Directory.Delete(directoryPath);
            File.Delete(filePath);
            File.Delete(zipPath);
        }
    }
}
