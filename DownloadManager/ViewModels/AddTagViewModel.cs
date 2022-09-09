using CommunityToolkit.Mvvm.ComponentModel;

namespace DownloadManager.ViewModels;

public partial class AddTagViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _tag;
}