using CommunityToolkit.Mvvm.ComponentModel;
using DownloadManager.ViewModels;

namespace DownloadManager.Models;

public partial class NavigationStore : ObservableObject, INavigationStore<ViewModelBase>
{
    [ObservableProperty]
    private ViewModelBase? _currentViewModel;
}