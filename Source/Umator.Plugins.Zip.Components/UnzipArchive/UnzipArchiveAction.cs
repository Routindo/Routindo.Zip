using System;
using Umator.Contract;
using Umator.Contract.Services;

namespace Umator.Plugins.Zip.Components.UnzipArchive
{
    [ExecutionArgumentsClass(typeof(UnzipArchiveActionExecutionArgs))]
    [PluginItemInfo(ComponentUniqueId, "Unzip archive",
        "Unzip existing archive to a specific directory")]
    public class UnzipArchiveAction: IAction
    {
        public const string ComponentUniqueId = "ABF48A71-D1C3-4500-9913-D5E9C72BA66B";

        [Argument(UnzipArchiveActionInstanceArgs.OutputDirPath)] public string OutputDirPath { get; set; } 
        [Argument(UnzipArchiveActionInstanceArgs.SourceZipPath)] public string SourceZipPath { get; set; }  
        [Argument(UnzipArchiveActionInstanceArgs.OverrideFiles)] public bool OverrideFiles { get; set; } 

        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }

        public ActionResult Execute(ArgumentCollection arguments)
        {
            try
            {
                string outputDirPath = null;
                string sourceZipPath = null;
                if (arguments.HasArgument(UnzipArchiveActionExecutionArgs.OutputDirPath))
                {
                    outputDirPath = arguments.GetValue<string>(UnzipArchiveActionExecutionArgs.OutputDirPath);
                }

                if (arguments.HasArgument(UnzipArchiveActionExecutionArgs.SourceZipPath))
                {
                    sourceZipPath = arguments.GetValue<string>(UnzipArchiveActionExecutionArgs.SourceZipPath);
                }

                if (string.IsNullOrWhiteSpace(outputDirPath))
                {
                    if (!string.IsNullOrWhiteSpace(OutputDirPath))
                        outputDirPath = OutputDirPath;
                }

                if (string.IsNullOrWhiteSpace(sourceZipPath))
                {
                    if (!string.IsNullOrWhiteSpace(SourceZipPath))
                        sourceZipPath = SourceZipPath;
                }

                if (string.IsNullOrWhiteSpace(sourceZipPath))
                    throw new MissingArgumentException(UnzipArchiveActionExecutionArgs.SourceZipPath);

                if (string.IsNullOrWhiteSpace(outputDirPath))
                    throw new MissingArgumentException(UnzipArchiveActionExecutionArgs.OutputDirPath);

                var result = ZipUtilities.ExtractArchive(sourceZipPath, outputDirPath, OverrideFiles);

                return result ? ActionResult.Succeeded() : ActionResult.Failed();
            }
            catch (Exception exception)
            {
                return ActionResult.Failed().WithException(exception);
            }
        }
    }
}
