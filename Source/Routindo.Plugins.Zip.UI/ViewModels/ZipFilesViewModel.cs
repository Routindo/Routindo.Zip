using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Routindo.Contract;
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

        public ZipFilesViewModel()
        {
            SelectFilesSourceDirectoryCommand = new ActionCommand(SelectFilesSourceDirectory);
            SelectSingleFileCommand = new ActionCommand(SelectSingleFile);
            SelectArchiveFileCommand = new ActionCommand(SelectArchiveFile);
        }

        public ICommand SelectFilesSourceDirectoryCommand { get; } 
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

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                    .WithArgument(ZipFilesActionInstanceArgs.FilePath, FilePath)
                    .WithArgument(ZipFilesActionInstanceArgs.ArchivePath, ArchivePath)
                    .WithArgument(ZipFilesActionInstanceArgs.FilesInDirectoryPath, FilesInDirectoryPath)
                    .WithArgument(ZipFilesActionInstanceArgs.IgnoreMissingFiles, IgnoreMissingFiles)
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
        }
    }
}
