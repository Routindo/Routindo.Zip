using System;
using System.IO;
using System.IO.Compression;
using Routindo.Contract;
using Routindo.Contract.Actions;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Services;

namespace Routindo.Plugins.Zip.Components.ZipDirectory
{
    [ExecutionArgumentsClass(typeof(ZipDirectoryActionExecutionArgs))]
    [PluginItemInfo(ComponentUniqueId, nameof(ZipDirectoryAction),
        "Create a ZIP archive from a given directory", Category = "Archive", FriendlyName = "Zip Directory")]
    [ResultArgumentsClass(typeof(ZipDirectoryActionResultsArgs))]
    public class ZipDirectoryAction : IAction
    {
        public const string ComponentUniqueId = "3DA98CBC-4DAB-464C-9352-AAC85869A5BD";

        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }

        [Argument(ZipDirectoryActionInstanceArgs.SourceDirectory, false)]
        public string SourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        /// <value>
        /// The output directory.
        /// </value>
        [Argument(ZipDirectoryActionInstanceArgs.OutputDirectory, false)]
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [create output directory].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [create output directory]; otherwise, <c>false</c>.
        /// </value>
        [Argument(ZipDirectoryActionInstanceArgs.CreateOutputDirectory, false)]
        public bool CreateOutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [extract to same location].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [extract to same location]; otherwise, <c>false</c>.
        /// </value>
        [Argument(ZipDirectoryActionInstanceArgs.UseLocationAsOutput, false)]
        public bool UseLocationAsOutput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [erase output if exists].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [erase output if exists]; otherwise, <c>false</c>.
        /// </value>
        [Argument(ZipDirectoryActionInstanceArgs.EraseOutputIfExists, false)]
        public bool EraseOutputIfExists { get; set; }

        /// <summary>
        /// Executes the specified arguments.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Missing mandatory argument {ZipDirectoryActionExecutionArgs.Directory}
        /// or
        /// Directory to ZIP Not Found ({sourceDirectory})
        /// or
        /// Unable to get the parent directory of SourceDirectory ({sourceDirectory})
        /// </exception>
        public ActionResult Execute(ArgumentCollection arguments)
        {
            string outputZipPath = null;
            string sourceDirectory = null;
            try
            {
                // Check if Argument Directory exists
                if (string.IsNullOrWhiteSpace(SourceDirectory) &&
                    !arguments.HasArgument(ZipDirectoryActionExecutionArgs.Directory))
                    throw new Exception($"Missing mandatory argument {ZipDirectoryActionExecutionArgs.Directory}");

                sourceDirectory = string.IsNullOrWhiteSpace(SourceDirectory)
                    ? arguments[ZipDirectoryActionExecutionArgs.Directory].ToString()
                    : SourceDirectory;

                // Check if directory exists 
                if (!Directory.Exists(sourceDirectory))
                    throw new Exception($"Directory to ZIP Not Found ({sourceDirectory})");

                var directoryName = new DirectoryInfo(sourceDirectory).Name;
                if (string.IsNullOrWhiteSpace(OutputDirectory))
                {
                    LoggingService.Info($"{ZipDirectoryActionInstanceArgs.OutputDirectory} is not set.");
                    if (!UseLocationAsOutput)
                    {
                        LoggingService.Warn(
                            $"[{ZipDirectoryActionInstanceArgs.UseLocationAsOutput}] is set to ({UseLocationAsOutput})");
                        throw new Exception(
                            $"Output Directory not set and it's not allowed to use location as output.");
                    }

                    OutputDirectory = Directory.GetParent(sourceDirectory)?.FullName;
                    if (string.IsNullOrWhiteSpace(OutputDirectory))
                        throw new Exception(
                            $"Unable to get the parent directory of SourceDirectory ({sourceDirectory})");
                }
                else
                {
                    LoggingService.Info(
                        $"[{ZipDirectoryActionInstanceArgs.CreateOutputDirectory}] is set to ({CreateOutputDirectory})");
                    if (!Directory.Exists(OutputDirectory) && CreateOutputDirectory)
                    {
                        LoggingService.Info($"Creating output directory at ({OutputDirectory})");
                        Directory.CreateDirectory(OutputDirectory);
                    }
                }

                if (!Directory.Exists(OutputDirectory))
                {
                    throw new Exception(
                        $"Output Directory not found ({OutputDirectory})");
                }

                outputZipPath = Path.Combine(OutputDirectory, $"{directoryName}.zip");
                if (File.Exists(outputZipPath))
                {
                    LoggingService.Warn(
                        $"[{ZipDirectoryActionInstanceArgs.EraseOutputIfExists}]=({EraseOutputIfExists}) Another file with the same name already exists in the output folder ({outputZipPath})");
                    if (EraseOutputIfExists)
                    {
                        LoggingService.Info(
                            $"[{ZipDirectoryActionInstanceArgs.EraseOutputIfExists}]=({EraseOutputIfExists}) Deleting the existing file from the output folder ({outputZipPath})");
                        File.Delete(outputZipPath);
                        LoggingService.Info(
                            $"Existing file deleted successfully from the output folder ({outputZipPath})");
                    }
                    else
                    {
                        throw new Exception($"Another File with same file exists in output location ({outputZipPath})");
                    }
                }

                ZipFile.CreateFromDirectory(sourceDirectory, outputZipPath);

                return ActionResult.Succeeded().WithAdditionInformation(ArgumentCollection.New()
                    .WithArgument(ZipDirectoryActionResultsArgs.SourceDirectory, sourceDirectory)
                    .WithArgument(ZipDirectoryActionResultsArgs.OutputZipPath, outputZipPath)
                );
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);
                return ActionResult.Failed(exception).WithAdditionInformation(ArgumentCollection.New()
                    .WithArgument(ZipDirectoryActionResultsArgs.SourceDirectory, sourceDirectory)
                    .WithArgument(ZipDirectoryActionResultsArgs.OutputZipPath, outputZipPath)
                );
            }
        }
    }
}
