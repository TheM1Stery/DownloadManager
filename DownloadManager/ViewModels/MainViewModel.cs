using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DownloadManager.Models;
using DownloadManager.Services;
using FluentAvalonia.UI.Controls;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace DownloadManager.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IViewModelFactory _factory;

    [ObservableProperty]
    private string _title = "Download Manager";


    public List<NavMenuItem> Pages { get; } = new();

    public List<NavMenuItem> FooterPages { get; } = new();

    [ObservableProperty]
    private ViewModelBase? _currentPage;

    [ObservableProperty]
    private NavMenuItem? _selectedMenuItem;
    

    partial void OnSelectedMenuItemChanged(NavMenuItem? value)
    {
        if (value?.ContentViewModelType is null)
            return;
        CurrentPage = _factory.Create(value.ContentViewModelType);
    }


    public MainViewModel(IViewModelFactory factory)
    {
        _factory = factory;
        Pages.Add(new NavMenuItem()
        {
            ContentViewModelType = typeof(DownloadViewModel),
            Header = "Download list",
            Icon = Symbol.Download
        });
        FooterPages.Add(new NavMenuItem()
        {
            ContentViewModelType = typeof(SettingsViewModel),
            Header = "Settings",
            Icon = Symbol.Settings
        });
    }
}