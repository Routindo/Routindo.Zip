using Routindo.Contract;
using Routindo.Contract.Attributes;
using Routindo.Plugins.Zip.Components.UnzipArchive;
using Routindo.Plugins.Zip.Components.ZipDirectory;
using Routindo.Plugins.Zip.Components.ZipFiles;
using Routindo.Plugins.Zip.UI.Views;

[assembly: ComponentConfigurator(typeof(ZipDirectoryView), ZipDirectoryAction.ComponentUniqueId, "Configurator for ZipDirectory Action")]
[assembly: ComponentConfigurator(typeof(UnzipArchiveView), UnzipArchiveAction.ComponentUniqueId, "Configurator for Unzip Archive Action")]
[assembly: ComponentConfigurator(typeof(ZipFilesView), ZipFilesAction.ComponentUniqueId, "Configurator for ZipFiles Action")]