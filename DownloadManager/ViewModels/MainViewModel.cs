using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace DownloadManager.ViewModels;

public partial class MainViewModel : ViewModelBase
{

    [ObservableProperty]
    private string _title = "Download Manager";

    
    public ObservableCollection<ViewModelBase> Pages { get; }

    [ObservableProperty]
    private ViewModelBase? _currentPage;
    
    
    public MainViewModel(IEnumerable<ViewModelBase> viewModels)
    {
        Pages = new ObservableCollection<ViewModelBase>(viewModels);
    }
}