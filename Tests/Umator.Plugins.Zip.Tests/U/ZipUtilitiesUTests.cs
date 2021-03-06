using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Umator.Plugins.Zip.Components;

namespace Umator.Plugins.Zip.Tests.U
{
    [TestClass]
    public class ZipUtilitiesUTests
    {
        [TestMethod]
        public void AddNoFilesToArchiveFails()
        {
            string zipPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".zip");

            Assert.IsFalse(ZipUtilities.AddFilesToArchive(zipPath, null));
            Assert.IsFalse(ZipUtilities.AddFilesToArchive(zipPath, new List<string>()));
        }

        [TestMethod]
        public void AddNoFileToArchiveFails()
        {
            string zipPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".zip");

            Assert.IsFalse(ZipUtilities.AddFileToArchive(zipPath, null));
            Assert.IsFalse(ZipUtilities.AddFileToArchive(zipPath, string.Empty));
        }
    }
}
