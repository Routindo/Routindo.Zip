using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Umator.Contract;

namespace Umator.Plugins.Zip.Components.ZipFiles
{
    [ExecutionArgumentsClass(typeof(ZipFilesActionExecutionArgs))]
    [PluginItemInfo(ComponentUniqueId, "Zip files",
        "Create a ZIP archive from a files or append files to existing archive")]
    public class ZipFilesAction: IAction
    {
        public const string ComponentUniqueId = "CFF0F532-3258-41BA-A5A9-52B6990964AD";

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public string Id { get; set; }
        [Argument(ZipFilesActionInstanceArgs.IgnoreMissingFiles)] public bool IgnoreMissingFiles { get; set; }
        [Argument(ZipFilesActionInstanceArgs.ArchivePath)] public string ArchivePath { get; set; }
        [Argument(ZipFilesActionInstanceArgs.FilePath)] public string FilePath { get; set; }
        [Argument(ZipFilesActionInstanceArgs.FilesInDirectoryPath)] public string FilesInDirectoryPath { get; set; }

        public ActionResult Execute(ArgumentCollection arguments)
        {
            try
            {
                List<string> files = null;  
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
                    _logger.Warn($"No files found to zip");
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

                return results? ActionResult.Succeeded() : ActionResult.Failed();
            }
            catch (Exception exception)
            {
                return ActionResult.Failed().WithException(exception);
            }
        }
    }
}
