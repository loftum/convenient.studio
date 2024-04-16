using System.Collections.ObjectModel;

namespace Convenient.Studio.ViewModels;

public interface IEnvironmentVm
{
    string Environment { get; set; }
    ObservableCollection<string> Environments { get; }
}