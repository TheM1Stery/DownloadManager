using CommunityToolkit.Mvvm.ComponentModel;
using DownloadManager.Services;

namespace DownloadManager.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    protected readonly INavigationService<ViewModelBase> NavigationService;

    protected ViewModelBase(INavigationService<ViewModelBase> navigationService)
    {
        NavigationService = navigationService;
    }
}

