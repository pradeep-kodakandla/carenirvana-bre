namespace carenirvanabre.codegenerator
{
    public interface IFileSystemSavePathInfo
    {
        string SavePath { get; }

        bool FileExists(string fileName, string folderNameToAppend);

        bool DeleteFile(string fileName, string folderNameToAppend);
    }
}
