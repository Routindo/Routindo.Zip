using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Umator.Contract;
using Umator.Contract.UI;
using Umator.Plugins.Zip.Components.ZipDirectory;

namespace Umator.Plugins.Zip.UI.ViewModels
{
    public sealed class ZipDirectoryViewModel: PluginConfiguratorViewModelBase
    {
        private string _outputDirectory;
        private string _sourceDirectory;
        private bool _createOutputDirectory = true;
        private bool _useLocationAsOutput = true;
        private bool _eraseOutputIfExists = true;

        public ZipDirectoryViewModel()
        {
            this.SelectSourceDirectoryCommand = new ActionCommand(SelectSourceDirectory);
            this.SelectOutputDirectoryCommand = new ActionCommand(SelectOutputDirectory);
        }

        public ICommand SelectSourceDirectoryCommand { get; }
        public ICommand SelectOutputDirectoryCommand { get; }

        private void SelectSourceDirectory()
        {
            this.SourceDirectory = SelectDirectory(SourceDirectory);
        }

        private void SelectOutputDirectory()
        {
            this.OutputDirectory = SelectDirectory(OutputDirectory);
        }

        private string SelectDirectory(string currentDirectory)
        {
            string selectedDirectory = currentDirectory;
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(currentDirectory))
                {
                    dialog.SelectedPath = currentDirectory;
                }

                dialog.ShowNewFolderButton = true;
                dialog.UseDescriptionForTitle = true;
                var dialogResult = dialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    selectedDirectory = dialog.SelectedPath;
                }
            }

            return selectedDirectory;
        }

        public string SourceDirectory
        {
            get => _sourceDirectory;
            set
            {
                _sourceDirectory = value;
                OnPropertyChanged();
            }
        }

        public string OutputDirectory
        {
            get => _outputDirectory;
            set
            {
                _outputDirectory = value;
                OnPropertyChanged();
                if (UseLocationAsOutput && !string.IsNullOrWhiteSpace(value))
                {
                    UseLocationAsOutput = false;
                }

                if (string.IsNullOrWhiteSpace(value) && !UseLocationAsOutput)
                {
                    UseLocationAsOutput = true;
                }
            }
        }

        public bool CreateOutputDirectory
        {
            get => _createOutputDirectory;
            set
            {
                _createOutputDirectory = value;
                OnPropertyChanged();
            }
        }

        public bool UseLocationAsOutput
        {
            get => _useLocationAsOutput;
            set
            {
                _useLocationAsOutput = value;
                OnPropertyChanged();
                if (value && !string.IsNullOrWhiteSpace(OutputDirectory))
                {
                    OutputDirectory = string.Empty;
                }
            }
        }

        public bool EraseOutputIfExists
        {
            get => _eraseOutputIfExists;
            set
            {
                _eraseOutputIfExists = value;
                OnPropertyChanged();
            }
        }

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                .WithArgument(ZipDirectoryActionInstanceArgs.SourceDirectory, SourceDirectory)
                .WithArgument(ZipDirectoryActionInstanceArgs.OutputDirectory, OutputDirectory)
                .WithArgument(ZipDirectoryActionInstanceArgs.CreateOutputDirectory, CreateOutputDirectory)
                .WithArgument(ZipDirectoryActionInstanceArgs.EraseOutputIfExists, EraseOutputIfExists)
                .WithArgument(ZipDirectoryActionInstanceArgs.UseLocationAsOutput, UseLocationAsOutput);
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if(arguments == null)
                return;

            if (arguments.HasArgument(ZipDirectoryActionInstanceArgs.SourceDirectory))
                SourceDirectory = arguments.GetValue<string>(ZipDirectoryActionInstanceArgs.SourceDirectory);

            if (arguments.HasArgument(ZipDirectoryActionInstanceArgs.OutputDirectory))
                OutputDirectory = arguments.GetValue<string>(ZipDirectoryActionInstanceArgs.OutputDirectory);

            if (arguments.HasArgument(ZipDirectoryActionInstanceArgs.CreateOutputDirectory))
                CreateOutputDirectory = arguments.GetValue<bool>(ZipDirectoryActionInstanceArgs.CreateOutputDirectory);

            if (arguments.HasArgument(ZipDirectoryActionInstanceArgs.EraseOutputIfExists))
                EraseOutputIfExists = arguments.GetValue<bool>(ZipDirectoryActionInstanceArgs.EraseOutputIfExists);

            if (arguments.HasArgument(ZipDirectoryActionInstanceArgs.UseLocationAsOutput))
                UseLocationAsOutput = arguments.GetValue<bool>(ZipDirectoryActionInstanceArgs.UseLocationAsOutput);
        }
    }
}
