using System.Collections.ObjectModel;
using DownloadManager.Models;

namespace DownloadManager.ViewModels;

public partial class DownloadViewModel : ViewModelBase
{
    public ObservableCollection<DownloadableItemViewModel> Items { get; } = new();

    public DownloadViewModel()
    {
        Items.Add(new DownloadableItemViewModel()
        {
            DownloadableItem = new DownloadableItem()
            {
                Name = "salam.txt",
                Size = 1000000L,
                IsPaused = false
            }
        });
    }
}