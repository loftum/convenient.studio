using System.Text.Json;
using System.Windows.Input;
using ReactiveUI;

namespace Convenient.Studio.ViewModels;

public class ScriptFileViewModel : ViewModelBase
{
    private string _name;
    public event EventHandler Deleted;

    public string FullPath => ToFullPath(Name);
    public string BasePath { get; }
    public string Extension { get; }
    public ICommand Close { get; }

    public string Name
    {
        get => _name;
        set
        {
            if (_name == value || string.IsNullOrWhiteSpace(value) || File.Exists(ToFullPath(value)))
            {
                this.RaisePropertyChanged();
                return;
            }

            var oldFullPath = FullPath;
            if (File.Exists(oldFullPath))
            {
                var newFullPath = ToFullPath(value);
                File.Move(oldFullPath, newFullPath);
            }

            _name = value;
            this.RaisePropertyChanged();
        }
    }

    public string Input { get; set; }
    public string Output { get; set; }
        
    public ScriptFileViewModel(string fullPath, string input, string output)
    {
        BasePath = Path.GetDirectoryName(fullPath);
        Name = Path.GetFileNameWithoutExtension(fullPath);
        Extension = Path.GetExtension(fullPath);
        Input = input;
        Output = output;
        Close = ReactiveCommand.Create(DoDelete);
    }
        
    private string ToFullPath(string name)
    {
        return Path.Combine(BasePath, $"{name}{Extension}");
    }
        
    public void Save()
    {
        var data = new Dictionary<string, string>
        {
            ["input"] = Input,
            ["output"] = Output
        };
        File.WriteAllText(FullPath, JsonSerializer.Serialize(data));
    }

    private void DoDelete()
    {
        File.Delete(FullPath);
        Deleted?.Invoke(this, EventArgs.Empty);
    }

    public static ScriptFileViewModel Load(string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            var file = new ScriptFileViewModel(fullPath, "", "");
            file.Save();
            return file;
        }

        var json = File.ReadAllText(fullPath);
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            
        return new ScriptFileViewModel(fullPath,
            dictionary.GetValueOrDefault("input", ""),
            dictionary.GetValueOrDefault("output", ""));
    }
}