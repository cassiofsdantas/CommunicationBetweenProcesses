namespace ProcessesTestRunner.Shared.Managers;

public static class DirectoryManager
{
    private static string CreateIfNotExistsPathDir(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    private static string CreateIfNotExistsPathDir(string path, string fileName)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return $"{path}{fileName}";
    }

    public static string BasePath => CreateIfNotExistsPathDir($@"{Directory.GetCurrentDirectory}\..\..\..\..\");
    public static string BaseDataMockPath => CreateIfNotExistsPathDir($@"{BasePath}DataMock\");
    public static string GetPath(string dirName) => CreateIfNotExistsPathDir($@"{BasePath}{dirName}");
    public static string GetFilePath(string fileName) => CreateIfNotExistsPathDir(BasePath, fileName);
    public static string GetFilePath(string dirName, string fileName) => CreateIfNotExistsPathDir($@"{BasePath}{dirName}\", fileName);
}
