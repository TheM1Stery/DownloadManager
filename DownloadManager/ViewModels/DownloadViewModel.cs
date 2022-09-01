using System.Collections.ObjectModel;
using DownloadManager.Models;

namespace DownloadManager.ViewModels;

public partial class DownloadViewModel : ViewModelBase
{
    public ObservableCollection<DownloadableItem> Items { get; } = new();
}