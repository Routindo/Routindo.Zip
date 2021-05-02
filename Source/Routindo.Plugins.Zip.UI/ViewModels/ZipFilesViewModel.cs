using System.Windows.Forms;
using System.Windows.Input;
using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.Zip.Components.ZipFiles;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Routindo.Plugins.Zip.UI.ViewModels
{
    public class ZipFilesViewModel: PluginConfiguratorViewModelBase
    {
        private bool _ignoreMissingFiles;
        private string _archivePath;
        private string _filePath;
        private string _filesInDirectoryPath;
        private bool _deleteZippedFiles;
        private bool _moveZippedFiles;
        private string _moveZippedFilesToPath;

        public ZipFilesViewModel()
        {
            SelectFilesSourceDirectoryCommand = new RelayCommand(SelectFilesSourceDirectory);
            SelectSingleFileCommand = new RelayCommand(SelectSingleFile);
            SelectArchiveFileCommand = new RelayCommand(SelectArchiveFile);
            SelectMovingPathDirectoryCommand = new RelayCommand(SelectMovingPathDirectory);
        }

        public ICommand SelectFilesSourceDirectoryCommand { get; } 
        public ICommand SelectMovingPathDirectoryCommand { get; }
        public ICommand SelectSingleFileCommand { get; } 
        public ICommand SelectArchiveFileCommand { get; }

        private void SelectFilesSourceDirectory()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(FilesInDirectoryPath))
                {
                    dialog.SelectedPath = FilesInDirectoryPath;
                }

                dialog.Description = "Output directory where to unzip the archive";
                dialog.ShowNewFolderButton = true;
                dialog.UseDescriptionForTitle = true;
                var dialogResult = dialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    FilesInDirectoryPath = dialog.SelectedPath;
                }
            }
        }

        private void SelectArchiveFile()
        {
            var dialog = new OpenFileDialog();

            if (!string.IsNullOrWhiteSpace(ArchivePath))
            {
                dialog.FileName = ArchivePath;
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
                ArchivePath = dialog.FileName;
            }
        }

        private void SelectSingleFile()
        {
            var dialog = new OpenFileDialog();

            if (!string.IsNullOrWhiteSpace(FilePath))
            {
                dialog.FileName = FilePath;
            }

            dialog.CheckFileExists = false;
            dialog.CheckPathExists = false;
            dialog.Multiselect = false;
            dialog.Title = "Select Single file to archive";
            dialog.Filter = "All files (*)|*";
            dialog.DefaultExt = "*";

            var dialogResult = dialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                FilePath = dialog.FileName;
            }
        }

        public bool IgnoreMissingFiles
        {
            get => _ignoreMissingFiles;
            set
            {
                _ignoreMissingFiles = value;
                OnPropertyChanged();
            }
        }

        public string ArchivePath
        {
            get => _archivePath;
            set
            {
                _archivePath = value;
                OnPropertyChanged();
            }
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged();
                if (!string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(FilesInDirectoryPath))
                {
                    FilesInDirectoryPath = string.Empty;
                }
            }
        }

        public string FilesInDirectoryPath
        {
            get => _filesInDirectoryPath;
            set
            {
                _filesInDirectoryPath = value;
                OnPropertyChanged();
                if (!string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(FilePath))
                {
                    FilePath = string.Empty;
                }
            }
        }

        public bool DeleteZippedFiles
        {
            get => _deleteZippedFiles;
            set
            {
                _deleteZippedFiles = value;
                OnPropertyChanged();
            }
        }

        public bool MoveZippedFiles
        {
            get => _moveZippedFiles;
            set
            {
                _moveZippedFiles = value;
                if (!_moveZippedFiles)
                    MoveZippedFilesToPath = string.Empty;
                else if (string.IsNullOrWhiteSpace(MoveZippedFilesToPath))
                {
                    ClearPropertyErrors(nameof(MoveZippedFilesToPath));
                    ValidateNonNullOrEmptyString(MoveZippedFilesToPath, nameof(MoveZippedFilesToPath));
                }
                OnPropertyChanged();
            }
        }

        public bool KeepZippedFiles => !DeleteZippedFiles && !MoveZippedFiles;

        public string MoveZippedFilesToPath
        {
            get => _moveZippedFilesToPath;
            set
            {
                _moveZippedFilesToPath = value;
                ClearPropertyErrors();
                if(MoveZippedFiles && string.IsNullOrWhiteSpace(_moveZippedFilesToPath))
                    ValidateNonNullOrEmptyString(_moveZippedFilesToPath);
                OnPropertyChanged();
            }
        }

        private void SelectMovingPathDirectory()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(MoveZippedFilesToPath))
                {
                    dialog.SelectedPath = MoveZippedFilesToPath;
                }

                dialog.Description = "Directory where to move the zipped files";
                dialog.ShowNewFolderButton = true;
                dialog.UseDescriptionForTitle = true;
                var dialogResult = dialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    MoveZippedFilesToPath = dialog.SelectedPath;
                }
            }
        }

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                    .WithArgument(ZipFilesActionInstanceArgs.FilePath, FilePath)
                    .WithArgument(ZipFilesActionInstanceArgs.ArchivePath, ArchivePath)
                    .WithArgument(ZipFilesActionInstanceArgs.FilesInDirectoryPath, FilesInDirectoryPath)
                    .WithArgument(ZipFilesActionInstanceArgs.IgnoreMissingFiles, IgnoreMissingFiles)
                    .WithArgument(ZipFilesActionInstanceArgs.DeleteZippedFiles, DeleteZippedFiles)
                    .WithArgument(ZipFilesActionInstanceArgs.MoveZippedFiles, MoveZippedFiles)
                    .WithArgument(ZipFilesActionInstanceArgs.MoveZippedFilesToPath, MoveZippedFilesToPath)
                ;
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if (arguments == null)
                return;

            if (arguments.HasArgument(ZipFilesActionInstanceArgs.FilePath))
                FilePath = arguments.GetValue<string>(ZipFilesActionInstanceArgs.FilePath);

            if (arguments.HasArgument(ZipFilesActionInstanceArgs.ArchivePath))
                ArchivePath = arguments.GetValue<string>(ZipFilesActionInstanceArgs.ArchivePath);

            if (arguments.HasArgument(ZipFilesActionInstanceArgs.FilesInDirectoryPath))
                FilesInDirectoryPath = arguments.GetValue<string>(ZipFilesActionInstanceArgs.FilesInDirectoryPath);

            if (arguments.HasArgument(ZipFilesActionInstanceArgs.IgnoreMissingFiles))
                IgnoreMissingFiles = arguments.GetValue<bool>(ZipFilesActionInstanceArgs.IgnoreMissingFiles);

            if (arguments.HasArgument(ZipFilesActionInstanceArgs.DeleteZippedFiles))
                DeleteZippedFiles = arguments.GetValue<bool>(ZipFilesActionInstanceArgs.DeleteZippedFiles);

            if (arguments.HasArgument(ZipFilesActionInstanceArgs.MoveZippedFiles))
                MoveZippedFiles = arguments.GetValue<bool>(ZipFilesActionInstanceArgs.MoveZippedFiles);

            if (arguments.HasArgument(ZipFilesActionInstanceArgs.MoveZippedFilesToPath))
                MoveZippedFilesToPath = arguments.GetValue<string>(ZipFilesActionInstanceArgs.MoveZippedFilesToPath);
        }
    }
}
