using System.Text.Json;

namespace Convenient.Studio.Scripting.Commandline;

internal class FileManager
{
    private const string Folder = "Files";
    public static readonly string AppliactionFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Folder);

    private readonly string _basePath;


    public FileManager(string basePath)
    {
        _basePath = basePath;
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }
    }

    public FileManager() : this(AppliactionFilesPath)
    {
    }

    public void SaveJson(object item, string filename = null)
    {
        if (item == null)
        {
            return;
        }
        var path = filename == null ? GetPathFor(item.GetType()) : GetPathFor(filename);
        File.WriteAllBytes(path, JsonSerializer.SerializeToUtf8Bytes(item));
    }

    public T LoadJson<T>(string filename = null)
    {
        var path = filename == null ? GetPathFor<T>() : GetPathFor(filename);
        return File.Exists(path)
            ? JsonSerializer.Deserialize<T>(File.ReadAllText(path))
            : default(T);
    }

    public T LoadJsonOrDefault<T>(T defaultValue = default(T))
    {
        try
        {
            return LoadJson<T>();
        }
        catch
        {
            return defaultValue;
        }
    }

    private string GetPathFor<T>()
    {
        return GetPathFor(typeof(T));
    }

    private string GetPathFor(Type type)
    {
        var filename = $"{type.Name}.json";
        return GetPathFor(filename);
    }

    private string GetPathFor(string filename)
    {
        var path = Path.Combine(_basePath, filename);
        return path;
    }
}