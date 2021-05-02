using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract;
using Routindo.Contract.Actions;
using Routindo.Contract.Arguments;
using Routindo.Contract.Services;
using Routindo.Plugins.Zip.Components.ZipDirectory;

namespace Routindo.Plugins.Zip.Tests.I
{
    [TestClass]
    public class ZipDirectoryTests
    {
        [TestMethod]
        [TestCategory("Integration Test")]
        public void CreateZipFromDirectoryTest()
        {
            var outputDirectory = Path.Combine(Path.GetTempPath(), "TEST_OUTPUT");
            IAction action = new ZipDirectoryAction()
            {
                OutputDirectory = outputDirectory,
                CreateOutputDirectory = true,
                EraseOutputIfExists = true, 
                Id = PluginUtilities.GetUniqueId(), LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(null)
            };

            var testDir = Path.Combine(Path.GetTempPath(), "TEMPO_TEST");
            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
            }

            var createdDir = new DirectoryInfo(testDir);
            Assert.IsTrue(Directory.Exists(createdDir.FullName));
            var file1 = Path.Combine(createdDir.FullName, "file1.txt");
            if (!File.Exists(file1))
                File.WriteAllText(file1, "Hello world");
            Assert.IsTrue(File.Exists(file1));
            var file2 = Path.Combine(createdDir.FullName, "file2.txt");
            if (!File.Exists(file2))
                File.WriteAllText(file2, "Hello world");
            Assert.IsTrue(File.Exists(file2));

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipDirectoryActionExecutionArgs.Directory, createdDir.FullName));

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);

            Assert.IsTrue(File.Exists(Path.Combine(outputDirectory, "TEMPO_TEST.zip")));

            Directory.Delete(createdDir.FullName, true);

            File.Delete(Path.Combine(Path.GetTempPath(), "TEMPO_TEST.zip"));
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void CreateZipFailsOnSourceDirectoryNotFoundTest()
        {
            var outputDirectory = Path.Combine(Path.GetTempPath(), "TEST_OUTPUT");
            IAction action = new ZipDirectoryAction()
            {
                OutputDirectory = outputDirectory,
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(null)
            };
            var testDir = Path.Combine(Path.GetTempPath(), "TEMPO_TEST");
            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipDirectoryActionExecutionArgs.Directory, testDir));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void CreateZipFailsOnOutputDirectoryNotFoundTest()
        {
            var outputDirectory = Path.Combine(Path.GetTempPath(), "TEST_OUTPUT");
            IAction action = new ZipDirectoryAction()
            {
                OutputDirectory = outputDirectory,
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(null)
            };
            var testDir = Path.Combine(Path.GetTempPath(), "TEMPO_TEST");
            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
            }

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipDirectoryActionExecutionArgs.Directory, testDir));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void CreateZipFailsOnNoEraseExistingOptionTest()
        { 
            var outputDirectory = Path.Combine(Path.GetTempPath(), "TEST_OUTPUT");
            IAction action = new ZipDirectoryAction()
            {
                OutputDirectory = outputDirectory,
                CreateOutputDirectory = true,
                EraseOutputIfExists = false,
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(null)
            };
            var testDir = Path.Combine(Path.GetTempPath(), "TEMPO_TEST");
            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
            }

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);
            File.WriteAllText(Path.Combine(outputDirectory, "TEMPO_TEST.zip"), "");

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipDirectoryActionExecutionArgs.Directory, testDir));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void CreateZipWithEraseExistingOptionTest()
        {
            var outputDirectory = Path.Combine(Path.GetTempPath(), "TEST_OUTPUT");
            IAction action = new ZipDirectoryAction()
            {
                OutputDirectory = outputDirectory,
                CreateOutputDirectory = true,
                EraseOutputIfExists = true, 
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(null)
            };
            var testDir = Path.Combine(Path.GetTempPath(), "TEMPO_TEST");
            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
            }

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);
            File.WriteAllText(Path.Combine(outputDirectory, "TEMPO_TEST.zip"), "");

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipDirectoryActionExecutionArgs.Directory, testDir));

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void CreateZipOnSameExistingOptionTest()
        {
            var outputDirectory = Path.GetTempPath();
            IAction action = new ZipDirectoryAction()
            {
                EraseOutputIfExists = true,
                UseLocationAsOutput = true,
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(null)
            };

            var testDir = Path.Combine(Path.GetTempPath(), "TEMPO_TEST");
            if (!Directory.Exists(testDir))
            {
                Directory.CreateDirectory(testDir);
            }

            File.WriteAllText(Path.Combine(outputDirectory, "TEMPO_TEST.zip"), "");

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipDirectoryActionExecutionArgs.Directory, testDir));

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
        }
    }
}
