using Umator.Contract;
using Umator.Plugins.Zip.Components.UnzipArchive;
using Umator.Plugins.Zip.Components.ZipDirectory;
using Umator.Plugins.Zip.Components.ZipFiles;
using Umator.Plugins.Zip.UI.Views;

[assembly: ComponentConfigurator(typeof(ZipDirectoryView), ZipDirectoryAction.ComponentUniqueId, "Configurator for ZipDirectory Action")]
[assembly: ComponentConfigurator(typeof(UnzipArchiveView), UnzipArchiveAction.ComponentUniqueId, "Configurator for Unzip Archive Action")]
[assembly: ComponentConfigurator(typeof(ZipFilesView), ZipFilesAction.ComponentUniqueId, "Configurator for ZipFiles Action")]