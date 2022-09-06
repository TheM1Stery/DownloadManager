using CommunityToolkit.Mvvm.ComponentModel;

namespace DownloadManager.Models;

public partial class DownloadableItem : ObservableObject
{
    public string? Name { get; set; }
    
    public long Size { get; set; }
    
    [ObservableProperty]
    private bool _isPaused;
}