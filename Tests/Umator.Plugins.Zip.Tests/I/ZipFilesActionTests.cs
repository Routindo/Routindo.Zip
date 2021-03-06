using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Umator.Contract;
using Umator.Contract.Services;
using Umator.Plugins.Zip.Components.ZipFiles;

namespace Umator.Plugins.Zip.Tests.I
{
    [TestClass]
    public class ZipFilesActionTests
    {
        /// <summary>
        /// This fails with cases of No Source files to zip are set in Instance arguments
        /// Or in Execution arguments
        /// </summary>
        [TestMethod]
        public void FailsWhenNoSourceFilesToZipTest()
        {
            IAction action = new ZipFilesAction()
            {
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(null)
            };

            string outputZipPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".zip");

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipFilesActionInstanceArgs.ArchivePath, outputZipPath));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.AreEqual( typeof(MissingArgumentsException), actionResult.AttachedException.GetType());

            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipFilesActionExecutionArgs.ArchivePath, outputZipPath));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.AreEqual(typeof(MissingArgumentsException), actionResult.AttachedException.GetType());

            // CLEANUP
            if(File.Exists(outputZipPath))
                File.Delete(outputZipPath);
        }

        /// <summary>
        /// This execution of the action should fail because there is no output zip file path
        /// set in the instance or execution arguments. 
        /// </summary>
        [TestMethod]
        [Description("Fails because not output zip file is set")]
        public void FailsWhenNoOutputZipFileTest()
        {
            IAction action = new ZipFilesAction()
            {
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(null)
            };

            string ticks = $"{DateTime.Now.Ticks}";

            string filesDirectory = Path.Combine(Path.GetTempPath(), $"dir-{ticks}");

            string file1ToZip = Path.Combine(filesDirectory, $"file-1-{ticks}.txt");
            string file2ToZip = Path.Combine(filesDirectory, $"file-1-{ticks}.txt");

            // Create filesDirectory 
            Directory.CreateDirectory(filesDirectory);
            Assert.IsTrue(Directory.Exists(filesDirectory));

            // Create file 1
            File.WriteAllText(file1ToZip, nameof(file1ToZip)); 
            File.WriteAllText(file2ToZip, nameof(file1ToZip));

            // With execution: list of files 
            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipFilesActionExecutionArgs.FilesPaths, new List<string>() { file1ToZip, file2ToZip }));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.AreEqual(typeof(MissingArgumentsException), actionResult.AttachedException.GetType());

            // With execution : files in directory
            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipFilesActionExecutionArgs.FilesInDirectoryPath, filesDirectory));

            
            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.AreEqual(typeof(MissingArgumentsException), actionResult.AttachedException.GetType());

            // With execution: single File
            ((ZipFilesAction) action).FilePath = file1ToZip;
            actionResult = action.Execute(ArgumentCollection.New());

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.AreEqual(typeof(MissingArgumentsException), actionResult.AttachedException.GetType());

            // With execution : files in directory
            ((ZipFilesAction) action).FilePath = null;
            ((ZipFilesAction)action).FilesInDirectoryPath = filesDirectory;
            actionResult = action.Execute(ArgumentCollection.New());


            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.AreEqual(typeof(MissingArgumentsException), actionResult.AttachedException.GetType());

            // CLEANUP
            if (Directory.Exists(filesDirectory))
                Directory.Delete(filesDirectory, true);

            if (File.Exists(file1ToZip))
                File.Delete(file1ToZip);

            if (File.Exists(file2ToZip))
                File.Delete(file2ToZip);
        }

        /// <summary>
        /// Action execution fails because the source files to zip are not existing
        /// </summary>
        [TestMethod]
        [Description("Fails because the target files to zip not exist")]
        public void FailsWhenFilesNotExistTest()
        {
            IAction action = new ZipFilesAction()
            {
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(null)
            };

            string ticks = $"{DateTime.Now.Ticks}";

            string filesDirectory = Path.Combine(Path.GetTempPath(), $"dir-{ticks}");

            string file1ToZip = Path.Combine(filesDirectory, $"file-1-{ticks}.txt");
            string file2ToZip = Path.Combine(filesDirectory, $"file-1-{ticks}.txt");
            string outputZip = Path.Combine(filesDirectory, $"output-{ticks}.zip");
           // Create filesDirectory
            Directory.CreateDirectory(filesDirectory);
            Assert.IsTrue(Directory.Exists(filesDirectory));

            // Create file 1
            //File.WriteAllText(file1ToZip, nameof(file1ToZip));
            //File.WriteAllText(file2ToZip, nameof(file1ToZip));

            // With execution: list of files 
            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipFilesActionExecutionArgs.FilesPaths, new List<string> { file1ToZip, file2ToZip })
                .WithArgument(ZipFilesActionExecutionArgs.ArchivePath, outputZip)
            );

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.AreEqual(typeof(Exception), actionResult.AttachedException.GetType());

            // With execution : files in directory
            // directory empty => Not files to zip
            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipFilesActionExecutionArgs.FilesInDirectoryPath, filesDirectory)
                .WithArgument(ZipFilesActionExecutionArgs.ArchivePath, outputZip)
            );


            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsNull(actionResult.AttachedException);
            // Assert.AreEqual(typeof(Exception), actionResult.AttachedException.GetType());

            // With execution: single File
            ((ZipFilesAction)action).FilePath = file1ToZip;
            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipFilesActionExecutionArgs.ArchivePath, outputZip)
            );

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.AreEqual(typeof(Exception), actionResult.AttachedException.GetType());

            // With execution : files in directory
            ((ZipFilesAction)action).FilePath = null;
            ((ZipFilesAction)action).FilesInDirectoryPath = filesDirectory;
            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipFilesActionExecutionArgs.ArchivePath, outputZip)
            );

             
            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsNull(actionResult.AttachedException);
            // Assert.AreEqual(typeof(Exception), actionResult.AttachedException.GetType());

            // CLEANUP
            if (Directory.Exists(filesDirectory))
                Directory.Delete(filesDirectory, true);

            if (File.Exists(file1ToZip))
                File.Delete(file1ToZip);

            if (File.Exists(file2ToZip))
                File.Delete(file2ToZip);

            if (File.Exists(outputZip))
                File.Delete(outputZip);
        }


        /// <summary>
        /// Action zip success with all cases 
        /// </summary>
        [TestMethod]
        [Description("Fails because the target files to zip not exist")]
        public void SuccessWhenFilesExistTest()
        {
            IAction action = new ZipFilesAction()
            {
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(null)
            };

            string ticks = $"{DateTime.Now.Ticks}";

            string filesDirectory = Path.Combine(Path.GetTempPath(), $"dir-{ticks}");

            string file1ToZip = Path.Combine(filesDirectory, $"file-1-{ticks}.txt");
            string file2ToZip = Path.Combine(filesDirectory, $"file-1-{ticks}.txt");
            string outputZip = Path.Combine(filesDirectory, $"output-{ticks}.zip");
            // Create filesDirectory
            Directory.CreateDirectory(filesDirectory);
            Assert.IsTrue(Directory.Exists(filesDirectory));

            // Create file 1
            File.WriteAllText(file1ToZip, nameof(file1ToZip));
            File.WriteAllText(file2ToZip, nameof(file2ToZip));

            // With execution: list of files 
            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipFilesActionExecutionArgs.FilesPaths, new List<string>() { file1ToZip, file2ToZip })
                .WithArgument(ZipFilesActionExecutionArgs.ArchivePath, outputZip)
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(File.Exists(outputZip));
            File.Delete(outputZip);

            // With execution : files in directory
            // directory empty => Not files to zip
            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipFilesActionExecutionArgs.FilesInDirectoryPath, filesDirectory)
                .WithArgument(ZipFilesActionExecutionArgs.ArchivePath, outputZip)
            );


            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(File.Exists(outputZip));
            File.Delete(outputZip);

            // With execution: single File
            ((ZipFilesAction)action).FilePath = file1ToZip;
            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipFilesActionExecutionArgs.ArchivePath, outputZip)
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(File.Exists(outputZip));
            File.Delete(outputZip);

            // With execution : files in directory
            ((ZipFilesAction)action).FilePath = null;
            ((ZipFilesAction)action).FilesInDirectoryPath = filesDirectory;
            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(ZipFilesActionExecutionArgs.ArchivePath, outputZip)
            );


            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(File.Exists(outputZip));
            File.Delete(outputZip);

            // CLEANUP
            if (Directory.Exists(filesDirectory))
                Directory.Delete(filesDirectory, true);

            if (File.Exists(file1ToZip))
                File.Delete(file1ToZip);

            if (File.Exists(file2ToZip))
                File.Delete(file2ToZip);

            if (File.Exists(outputZip))
                File.Delete(outputZip);
        }
    }
}
