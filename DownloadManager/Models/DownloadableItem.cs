using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DownloadManager.Models;

public partial class DownloadableItem : ObservableObject
{
    public string? Name { get; set; }
    
    public long Size { get; set; }
    
    [ObservableProperty]
    private bool _isPaused;
}