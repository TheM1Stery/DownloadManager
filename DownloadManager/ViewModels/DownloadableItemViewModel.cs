using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DownloadManager.Models;

namespace DownloadManager.ViewModels;

public partial class DownloadableItemViewModel : ObservableObject
{
    [ObservableProperty]
    private DownloadableItem _downloadableItem = new();


    [RelayCommand]
    private void Pause()
    {
        DownloadableItem.IsPaused = !DownloadableItem.IsPaused;
    }
}