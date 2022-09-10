using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DownloadManager.Services;
using DownloadManager.ViewModels;
using DownloadManager.Views;
using Microsoft.Extensions.DependencyInjection;

namespace DownloadManager
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }


        private IServiceProvider? _serviceProvider;


        private partial class ViewModelFactory : IViewModelFactory
        {
        }
        
        public override void OnFrameworkInitializationCompleted()
        {
            _serviceProvider = ConfigureServices();
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainView
                {
                    DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
                };
            }
            base.OnFrameworkInitializationCompleted();
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddHttpClient()
                .AddSingleton<MainViewModel>()
                .AddSingleton<IViewModelFactory, ViewModelFactory>()
                .AddSingleton<DownloadViewModel>()
                .AddSingleton<SettingsViewModel>()
                .AddTransient<DownloadableItemViewModel>()
                .AddSingleton<IFolderPicker, FolderPicker>()
                .AddTransient<IFileDownloader, FileDownloader>()
                .AddTransient<AddTagViewModel>()
                .BuildServiceProvider();
        }
    }
}