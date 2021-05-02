using System.Windows.Forms;
using System.Windows.Input;
using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.Zip.Components.UnzipArchive;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Routindo.Plugins.Zip.UI.ViewModels
{
    public class UnzipArchiveViewModel : PluginConfiguratorViewModelBase
    {
        private string _outputDirPath;
        private string _sourceZipPath;
        private bool _overrideFiles;

        public UnzipArchiveViewModel()
        {
            SelectOutputDirectoryCommand = new RelayCommand(SelectOutputDirectory);
            SelectArchiveFileCommand = new RelayCommand(SelectArchiveFile);
        }

        public ICommand SelectOutputDirectoryCommand { get; }
        public ICommand SelectArchiveFileCommand { get; }

        private void SelectOutputDirectory()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(OutputDirPath))
                {
                    dialog.SelectedPath = OutputDirPath;
                }

                dialog.Description = "Output directory where to unzip the archive";
                dialog.ShowNewFolderButton = true;
                dialog.UseDescriptionForTitle = true;
                var dialogResult = dialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    OutputDirPath = dialog.SelectedPath;
                }
            }
        }

        private void SelectArchiveFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (!string.IsNullOrWhiteSpace(SourceZipPath))
            {
                dialog.FileName = SourceZipPath;
            }

            dialog.CheckFileExists = false;
            dialog.CheckPathExists = false;
            dialog.Multiselect = false;
            dialog.Title = "Select Source archive file";
            dialog.Filter = "Archive File (.zip)|*.zip";
            dialog.DefaultExt = "*.zip";

            var dialogResult = dialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                SourceZipPath = dialog.FileName;
            }
        }

        public string OutputDirPath
        {
            get => _outputDirPath;
            set
            {
                _outputDirPath = value;
                OnPropertyChanged();
            }
        }

        public string SourceZipPath
        {
            get => _sourceZipPath;
            set
            {
                _sourceZipPath = value;
                OnPropertyChanged();
            }
        }

        public bool OverrideFiles
        {
            get => _overrideFiles;
            set
            {
                _overrideFiles = value;
                OnPropertyChanged();
            }
        }

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                .WithArgument(UnzipArchiveActionInstanceArgs.SourceZipPath, SourceZipPath)
                .WithArgument(UnzipArchiveActionInstanceArgs.OutputDirPath, OutputDirPath)
                .WithArgument(UnzipArchiveActionInstanceArgs.OverrideFiles, OverrideFiles)
                ;
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if (arguments == null)
                return;

            if (arguments.HasArgument(UnzipArchiveActionInstanceArgs.SourceZipPath))
                SourceZipPath = arguments.GetValue<string>(UnzipArchiveActionInstanceArgs.SourceZipPath);

            if (arguments.HasArgument(UnzipArchiveActionInstanceArgs.OutputDirPath))
                OutputDirPath = arguments.GetValue<string>(UnzipArchiveActionInstanceArgs.OutputDirPath);

            if (arguments.HasArgument(UnzipArchiveActionInstanceArgs.OverrideFiles))
                OverrideFiles = arguments.GetValue<bool>(UnzipArchiveActionInstanceArgs.OverrideFiles);

        }
    }
}
