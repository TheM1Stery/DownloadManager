using DownloadManager.Models;
using DownloadManager.ViewModels;

namespace DownloadManager.Services;

public class NavigationService : INavigationService<ViewModelBase>
{
    private INavigationStore<ViewModelBase> _navigationStore;
    private readonly IViewModelFactory<ViewModelBase> _viewModelFactory;


    public NavigationService(INavigationStore<ViewModelBase> navigationStore, 
        IViewModelFactory<ViewModelBase> viewModelFactory)
    {
        _navigationStore = navigationStore;
        _viewModelFactory = viewModelFactory;
    }

    public void Navigate<T>() where T : ViewModelBase
    {
        _navigationStore.CurrentViewModel = _viewModelFactory.Create<T>();
    }
}