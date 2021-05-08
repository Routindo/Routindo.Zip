using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Routindo.Contract.Actions;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Exceptions;
using Routindo.Contract.Services;

namespace Routindo.Plugins.Zip.Components.ZipFiles
{
    [ExecutionArgumentsClass(typeof(ZipFilesActionExecutionArgs))]
    [PluginItemInfo(ComponentUniqueId, nameof(ZipFilesAction),
        "Create a ZIP archive from a files or append files to existing archive", Category = "Archive", FriendlyName = "Zip Files")]
    [ResultArgumentsClass(typeof(ZipFilesActionResultArgs))]
    public class ZipFilesAction: IAction
    {
        public const string ComponentUniqueId = "CFF0F532-3258-41BA-A5A9-52B6990964AD";

        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }
        [Argument(ZipFilesActionInstanceArgs.IgnoreMissingFiles)] public bool IgnoreMissingFiles { get; set; }
        [Argument(ZipFilesActionInstanceArgs.ArchivePath)] public string ArchivePath { get; set; }
        [Argument(ZipFilesActionInstanceArgs.FilePath)] public string FilePath { get; set; }
        [Argument(ZipFilesActionInstanceArgs.FilesInDirectoryPath)] public string FilesInDirectoryPath { get; set; }

        // Optional for After Zip
        [Argument(ZipFilesActionInstanceArgs.DeleteZippedFiles)] public bool DeleteZippedFiles { get; set; }
        [Argument(ZipFilesActionInstanceArgs.MoveZippedFiles)] public bool MoveZippedFiles { get; set; }
        [Argument(ZipFilesActionInstanceArgs.MoveZippedFilesToPath)] public string MoveZippedFilesToPath { get; set; }

        public ActionResult Execute(ArgumentCollection arguments)
        {
            List<string> files = null;
            try
            {
                if (arguments.HasArgument(ZipFilesActionExecutionArgs.FilesPaths))
                {
                    if (!(arguments[ZipFilesActionExecutionArgs.FilesPaths] is List<string> filePaths))
                        throw new ArgumentsValidationException(
                            $"unable to cast argument value into list of string. key({ZipFilesActionExecutionArgs.FilesPaths})");
                    files = filePaths;  // arguments.GetValue<string[]>(ZipFilesActionExecutionArgs.FilesPaths);
                }
                else if (arguments.HasArgument(ZipFilesActionExecutionArgs.FilesInDirectoryPath))
                {
                    var directory = arguments.GetValue<string>(ZipFilesActionExecutionArgs.FilesInDirectoryPath);
                    if (Directory.Exists(directory))
                    {
                        files = Directory.GetFiles(directory).ToList();
                    }
                }

                if (files == null)
                {
                    if (!string.IsNullOrWhiteSpace(FilePath))
                    {
                        files = new List<string>() {FilePath};
                    }
                    else if (!string.IsNullOrWhiteSpace(FilesInDirectoryPath) && Directory.Exists(FilesInDirectoryPath))
                    {
                        if (Directory.Exists(FilesInDirectoryPath))
                        {
                            files = Directory.GetFiles(FilesInDirectoryPath).ToList();
                        }
                    }
                }

                if (files == null)
                {
                    throw new MissingArgumentsException($"Not source files to zip are defined. " +
                                                        $"at least one of the following arguments must be set [{ZipFilesActionInstanceArgs.FilePath}, {ZipFilesActionInstanceArgs.FilesInDirectoryPath}, {ZipFilesActionExecutionArgs.FilesInDirectoryPath}, {ZipFilesActionExecutionArgs.FilesPaths} ]");
                }

                if (!files.Any())
                {
                    LoggingService.Warn($"No files found to zip");
                    return ActionResult.Succeeded();
                }

                var missingFiles = files.Where(f => !File.Exists(f)).ToList();
                if (!IgnoreMissingFiles && missingFiles.Any())
                {
                    throw new Exception(
                        $"Cannot execute action because the following files are not found : ({string.Join(",", missingFiles)})");
                }
                else if (IgnoreMissingFiles && files.Count == 1 && missingFiles.Any())
                {
                    throw new Exception(
                        $"Cannot execute action because the unique file to zip is not found ({files.Single()})");
                }

                string zipPath = null;
                if (arguments.HasArgument(ZipFilesActionExecutionArgs.ArchivePath)) 
                {
                    zipPath = arguments.GetValue<string>(ZipFilesActionExecutionArgs.ArchivePath);
                }
                else if (!string.IsNullOrWhiteSpace(ArchivePath))
                {
                    zipPath = ArchivePath;
                }

                if (string.IsNullOrWhiteSpace(zipPath))
                {
                    throw new MissingArgumentsException(
                        $"No archive path was specified in instance or executing arguments." +
                        $" at least one of this arguments must be set Instance[{ZipFilesActionInstanceArgs.ArchivePath}] Execution[{ZipFilesActionExecutionArgs.ArchivePath}]");
                }

                var results = ZipUtilities.AddFilesToArchive(zipPath, files);

                if (results)
                {
                    if (DeleteZippedFiles)
                    {
                        LoggingService.Debug($"Deleting ({files.Count}) Zipped files");
                        foreach (var file in files)
                        {
                            try
                            {
                                LoggingService.Debug($"Deleting Zipped File {file}");
                                File.Delete(file);
                                LoggingService.Debug($"Zipped File {file} deleted successfully");
                            }
                            catch (Exception exception)
                            {
                                LoggingService.Error(exception);
                            }
                        }
                    }
                    else if (MoveZippedFiles && !string.IsNullOrWhiteSpace(MoveZippedFilesToPath))
                    {
                        LoggingService.Debug($"Moving ({files.Count}) Zipped files to path {MoveZippedFilesToPath}");
                        if (!Directory.Exists(MoveZippedFilesToPath))
                        {
                            Directory.CreateDirectory(MoveZippedFilesToPath);
                        }

                        foreach (var file in files)
                        {
                            try
                            {
                                var destinationFilePath = Path.Combine(MoveZippedFilesToPath, Path.GetFileName(file));
                                LoggingService.Debug($"Moving file {file} to {destinationFilePath}");
                                File.Move(file, destinationFilePath);
                                LoggingService.Debug(
                                    $"Zipped file {Path.GetFileName(file)} moved successfully to {MoveZippedFilesToPath}");
                            }
                            catch (Exception exception)
                            {
                                LoggingService.Error(exception);
                            }
                        }
                    }

                }

                return results? ActionResult.Succeeded().WithAdditionInformation(ArgumentCollection.New()
                    .WithArgument(ZipFilesActionResultArgs.ArchivePath, ArchivePath)
                    .WithArgument(ZipFilesActionResultArgs.ZippedFiles, files)
                ) : ActionResult.Failed()
                        .WithAdditionInformation(ArgumentCollection.New()
                            .WithArgument(ZipFilesActionResultArgs.ArchivePath, ArchivePath)
                            .WithArgument(ZipFilesActionResultArgs.ZippedFiles, files?? new List<string>())
                        )
                    ;
            }
            catch (Exception exception)
            {
                return ActionResult.Failed().WithException(exception)
                        .WithAdditionInformation(ArgumentCollection.New()
                            .WithArgument(ZipFilesActionResultArgs.ArchivePath, ArchivePath)
                            .WithArgument(ZipFilesActionResultArgs.ZippedFiles, files ?? new List<string>())
                        )
                    ;
            }
        }
    }
}
