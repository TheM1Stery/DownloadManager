using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using DownloadManager.Models;
using DownloadManager.Services;
using FluentAvalonia.UI.Controls;

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


    private void AddMenuPages()
    {
        
    }

    private void AddFooterPages()
    {
        
    }

    public MainViewModel(IViewModelFactory factory)
    {
        _factory = factory;
        Pages.Add(new NavMenuItem
        {
            ContentViewModelType = typeof(DownloadViewModel),
            Header = "Download list",
            Icon = Symbol.Download
        });
        FooterPages.Add(new NavMenuItem
        {
            ContentViewModelType = typeof(SettingsViewModel),
            Header = "Settings",
            Icon = Symbol.Settings
        });
    }
}