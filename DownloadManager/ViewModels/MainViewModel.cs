using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DownloadManager.Models;
using DownloadManager.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace DownloadManager.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public INavigationStore<ViewModelBase> Store { get; }

    [ObservableProperty]
    private string _title = "Download Manager";


    public MainViewModel(INavigationService<ViewModelBase> navigationService, INavigationStore<ViewModelBase> store) 
        : base(navigationService)
    {
        Store = store;
    }
}