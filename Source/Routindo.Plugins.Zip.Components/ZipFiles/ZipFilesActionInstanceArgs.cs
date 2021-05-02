namespace Routindo.Plugins.Zip.Components.ZipFiles
{
    public static class ZipFilesActionInstanceArgs 
    {
        public const string ArchivePath = nameof(ArchivePath);
        public const string FilePath = nameof(FilePath);
        public const string FilesInDirectoryPath = nameof(FilesInDirectoryPath);
        public const string IgnoreMissingFiles = nameof(IgnoreMissingFiles);

        // Optional 
        public const string DeleteZippedFiles = nameof(DeleteZippedFiles); 
        public const string MoveZippedFiles = nameof(MoveZippedFiles);
        public const string MoveZippedFilesToPath = nameof(MoveZippedFilesToPath);
    }
}