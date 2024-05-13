using System.Collections.ObjectModel;
using Convenient.Studio.ViewModels;

namespace Convenient.Studio;

public class StudioContext : ViewModelBase, IEnvironmentVm
{
    public CancellationTokenSource CancellationTokenSource { get; set; } = new();
    public CancellationToken CancellationToken => CancellationTokenSource.Token;
    
    public string Environment { get; set; }
    public ObservableCollection<string> Environments { get; } = [];
}