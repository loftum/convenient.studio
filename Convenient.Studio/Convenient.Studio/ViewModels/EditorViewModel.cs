using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;

namespace Convenient.Studio.ViewModels;

public class EditorViewModel : ViewModelBase
{
    private const string Folder = "SaveFiles";
    private static readonly string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Folder);
    private static string IndexFile => Path.Combine(BasePath, "index.txt");
        
    private ScriptFileViewModel _currentFile;
        
    public ObservableCollection<ScriptFileViewModel> Files { get; } = new();

    public ICommand NewFile { get; }
    public ICommand Save { get; }
    public ICommand CloseCurrentFile { get; }
    public ICommand CloseAllButThis { get; }
        
    public string Input
    {
        get => CurrentFile.Input;
        set
        {
            CurrentFile.Input = value;
            this.RaisePropertyChanged();
        }
    }
        
    public string Output
    {
        get => CurrentFile.Output;
        set
        {
            CurrentFile.Output = value;
            this.RaisePropertyChanged();
        }
    }
        
    private bool _inputEnabled;
    public bool InputEnabled
    {
        get => _inputEnabled;
        set => this.RaiseAndSetIfChanged(ref _inputEnabled, value);
    }
        
    public ScriptFileViewModel CurrentFile
    {
        get => _currentFile;
        private set
        {
            _currentFile = value;
            Input = value.Input;
            Output = value.Output;
            this.RaisePropertyChanged();
        }
    }

    public EditorViewModel()
    {
        NewFile = ReactiveCommand.Create(AddNewFile);
        Save = ReactiveCommand.Create(SaveFiles);
        CloseCurrentFile = ReactiveCommand.Create(DoCloseCurrentFile);
        CloseAllButThis = ReactiveCommand.Create(DoCloseAllButThis);
    }

    public void Load()
    {
        Directory.CreateDirectory(BasePath);
        var files = Directory.EnumerateFiles(BasePath, "*.json").Select(ScriptFileViewModel.Load).ToList();
        var current = File.Exists(IndexFile) ? File.ReadAllText(IndexFile) : null;

        if (files.Any())
        {
            foreach (var file in files)
            {
                file.Deleted += File_Deleted;
            }
            Files.AddRange(files);
            CurrentFile = files.FirstOrDefault(f => f.Name == current) ?? files.First();
        }
        else
        {
            AddNewFile();
        }
    }

    public void SaveFiles()
    {
        foreach (var file in Files)
        {
            file.Save();
        }
        File.WriteAllText(IndexFile, CurrentFile.Name);
    }

    private void AddNewFile()
    {
        var index = 1;
        var fullPath = ToFullPath($"new-{index}");
        while (File.Exists(fullPath))
        {
            index++;
            fullPath = ToFullPath($"new-{index}");
        }

        var file = ScriptFileViewModel.Load(fullPath);
        file.Deleted += File_Deleted;
        Files.Add(file);
        CurrentFile = file;
    }

    private void File_Deleted(object sender, EventArgs e)
    {
        if (sender is ScriptFileViewModel file)
        {
            Files.Remove(file);
        }
    }

    private static string ToFullPath(string filename)
    {
        return Path.Combine(BasePath, $"{filename}.json");
    }

    private void DoCloseAllButThis()
    {
        var otherFiles = Files.Where(f => f != CurrentFile).ToList();
        foreach (var otherFile in otherFiles)
        {
            File.Delete(otherFile.FullPath);
            Files.Remove(otherFile);
        }
    }

    private void DoClose(ScriptFileViewModel file)
    {
        var newIndex = Files.GetIndexAfterRemovalOf(file);
            
        Files.Remove(file);
        File.Delete(file.FullPath);
            
        if (Files.Count == 0)
        {
            AddNewFile();
        }
        else
        {
            CurrentFile = Files[newIndex];
        }
    }

    private void DoCloseCurrentFile() => DoClose(CurrentFile);
}