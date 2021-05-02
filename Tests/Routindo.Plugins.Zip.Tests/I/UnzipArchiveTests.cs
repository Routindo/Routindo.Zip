using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract;
using Routindo.Contract.Actions;
using Routindo.Contract.Arguments;
using Routindo.Plugins.Zip.Components;
using Routindo.Plugins.Zip.Components.UnzipArchive;

namespace Routindo.Plugins.Zip.Tests.I
{
    [TestClass]
    public class UnzipArchiveTests
    {
        [TestMethod]
        [TestCategory("Integration Test")]
        public void UnzipArchiveSuccessfulTest()
        {
            string ticks = $"{DateTime.Now.Ticks}";
            string directoryName = $"dir{ticks}";
            string directoryPath = Path.Combine(Path.GetTempPath(), directoryName);
            string zipPath = Path.Combine(directoryPath, $"source{ticks}.zip");
            string outputDir = Path.Combine(directoryPath, "output");

            // Create empty directory 
            Directory.CreateDirectory(directoryPath);
            Assert.IsTrue(Directory.Exists(directoryPath));

            // Create file with some data
            string fileName = $"file{ticks}.txt";
            string filePath = Path.Combine(directoryPath, fileName);
            File.WriteAllText(filePath, $"{this.GetType()}");
            Assert.IsTrue(File.Exists(filePath));

            ZipUtilities.AddFileToArchive(zipPath, filePath);
            Assert.IsTrue(File.Exists(zipPath));

            // With instance arguments
            IAction action = new UnzipArchiveAction()
            {
                SourceZipPath = zipPath,
                OutputDirPath = outputDir,
            };

            var actionResult = action.Execute(ArgumentCollection.New());
            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(Directory.Exists(outputDir));
            Assert.IsTrue(Directory.GetFiles(outputDir).Any());
            Directory.Delete(outputDir,true);

            // With output dir defined in execution arguments 
            ((UnzipArchiveAction) action).OutputDirPath = null;
            action.Execute(ArgumentCollection.New()
                .WithArgument(UnzipArchiveActionExecutionArgs.OutputDirPath, outputDir)
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(Directory.Exists(outputDir));
            Assert.IsTrue(Directory.GetFiles(outputDir).Any());
            Directory.Delete(outputDir, true);


            // With only execution arguments 
            ((UnzipArchiveAction)action).SourceZipPath = null;
            action.Execute(ArgumentCollection.New()
                .WithArgument(UnzipArchiveActionExecutionArgs.OutputDirPath, outputDir)
                .WithArgument(UnzipArchiveActionExecutionArgs.SourceZipPath, zipPath)
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(Directory.Exists(outputDir));
            Assert.IsTrue(Directory.GetFiles(outputDir).Any());
            Directory.Delete(outputDir, true);

            // With Output dir defined in instance arguments 
            ((UnzipArchiveAction)action).OutputDirPath = outputDir;
            action.Execute(ArgumentCollection.New()
                .WithArgument(UnzipArchiveActionExecutionArgs.SourceZipPath, zipPath)
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(Directory.Exists(outputDir));
            Assert.IsTrue(Directory.GetFiles(outputDir).Any());

            // Override existing file 
            ((UnzipArchiveAction) action).OverrideFiles = true;
            action.Execute(ArgumentCollection.New()
                .WithArgument(UnzipArchiveActionExecutionArgs.SourceZipPath, zipPath)
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(Directory.Exists(outputDir));
            Assert.IsTrue(Directory.GetFiles(outputDir).Any());

            // Don't Override existing file 
            ((UnzipArchiveAction)action).OverrideFiles = false;
            action.Execute(ArgumentCollection.New()
                .WithArgument(UnzipArchiveActionExecutionArgs.SourceZipPath, zipPath)
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(Directory.Exists(outputDir));
            Assert.IsTrue(Directory.GetFiles(outputDir).Any());

            // Cleanup
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);

            if (Directory.Exists(directoryPath))
                Directory.Delete(directoryPath, true);

            if (File.Exists(filePath))
                File.Delete(filePath);

            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void FailsOnMissingSourceZipTest()
        {
            string ticks = $"{DateTime.Now.Ticks}";
            string directoryName = $"dir{ticks}";
            string directoryPath = Path.Combine(Path.GetTempPath(), directoryName);
            string zipPath = Path.Combine(directoryPath, $"source{ticks}.zip");
            string outputDir = Path.Combine(directoryPath, "output");

            // Create empty directory 
            Directory.CreateDirectory(directoryPath);
            Assert.IsTrue(Directory.Exists(directoryPath));

            // Create file with some data
            string fileName = $"file{ticks}.txt";
            string filePath = Path.Combine(directoryPath, fileName);
            File.WriteAllText(filePath, $"{this.GetType()}");
            Assert.IsTrue(File.Exists(filePath));

            ZipUtilities.AddFileToArchive(zipPath, filePath);
            Assert.IsTrue(File.Exists(zipPath));

            // With instance arguments
            IAction action = new UnzipArchiveAction()
            {
                OutputDirPath = outputDir,
            };

            var actionResult = action.Execute(ArgumentCollection.New());
            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);

            // With output dir defined in execution arguments 
            ((UnzipArchiveAction)action).OutputDirPath = null;
            action.Execute(ArgumentCollection.New()
                .WithArgument(UnzipArchiveActionExecutionArgs.OutputDirPath, outputDir)
            );

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);



            // Cleanup
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);

            if (Directory.Exists(directoryPath))
                Directory.Delete(directoryPath, true);

            if (File.Exists(filePath))
                File.Delete(filePath);

            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void FailsOnMissingOutputDirTest()
        {
            string ticks = $"{DateTime.Now.Ticks}";
            string directoryName = $"dir{ticks}";
            string directoryPath = Path.Combine(Path.GetTempPath(), directoryName);
            string zipPath = Path.Combine(directoryPath, $"source{ticks}.zip");
            string outputDir = Path.Combine(directoryPath, "output");

            // Create empty directory 
            Directory.CreateDirectory(directoryPath);
            Assert.IsTrue(Directory.Exists(directoryPath));

            // Create file with some data
            string fileName = $"file{ticks}.txt";
            string filePath = Path.Combine(directoryPath, fileName);
            File.WriteAllText(filePath, $"{this.GetType()}");
            Assert.IsTrue(File.Exists(filePath));

            ZipUtilities.AddFileToArchive(zipPath, filePath);
            Assert.IsTrue(File.Exists(zipPath));

            // With instance arguments
            IAction action = new UnzipArchiveAction()
            {
                SourceZipPath = zipPath,
            };

            var actionResult = action.Execute(ArgumentCollection.New());
            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);

            // With output dir defined in execution arguments 
            ((UnzipArchiveAction)action).SourceZipPath = null;
            action.Execute(ArgumentCollection.New()
                .WithArgument(UnzipArchiveActionExecutionArgs.SourceZipPath, zipPath)
            );

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);



            // Cleanup
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);

            if (Directory.Exists(directoryPath))
                Directory.Delete(directoryPath, true);

            if (File.Exists(filePath))
                File.Delete(filePath);

            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }
}
