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
        private bool _createOutputDirectory = true;
        private bool _useLocationAsOutput = true;
        private bool _eraseOutputIfExists = true;

        public ZipDirectoryViewModel()
        {
            this.SelectDirectoryCommand = new ActionCommand(SelectDirectory); 
        }

        public ICommand SelectDirectoryCommand { get; }

        private void SelectDirectory()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(OutputDirectory))
                {
                    dialog.SelectedPath = OutputDirectory;
                }

                dialog.Description = "Directory where to watch for new files";
                dialog.ShowNewFolderButton = true;
                dialog.UseDescriptionForTitle = true;
                var dialogResult = dialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    OutputDirectory = dialog.SelectedPath;
                }
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
                .WithArgument(ZipDirectoryActionInstanceArgs.OutputDirectory, OutputDirectory)
                .WithArgument(ZipDirectoryActionInstanceArgs.CreateOutputDirectory, CreateOutputDirectory)
                .WithArgument(ZipDirectoryActionInstanceArgs.EraseOutputIfExists, EraseOutputIfExists)
                .WithArgument(ZipDirectoryActionInstanceArgs.UseLocationAsOutput, UseLocationAsOutput);
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if(arguments == null)
                return;

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
