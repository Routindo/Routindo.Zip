using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Plugins.Zip.Components;

namespace Routindo.Plugins.Zip.Tests.U
{
    [TestClass]
    public class ZipUtilitiesUTests
    {
        [TestMethod]
        [TestCategory("Unit Test")]
        public void AddNoFilesToArchiveFails()
        {
            string zipPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".zip");

            Assert.IsFalse(ZipUtilities.AddFilesToArchive(zipPath, null));
            Assert.IsFalse(ZipUtilities.AddFilesToArchive(zipPath, new List<string>()));
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public void AddNoFileToArchiveFails()
        {
            string zipPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".zip");

            Assert.IsFalse(ZipUtilities.AddFileToArchive(zipPath, null));
            Assert.IsFalse(ZipUtilities.AddFileToArchive(zipPath, string.Empty));
        }
    }
}
