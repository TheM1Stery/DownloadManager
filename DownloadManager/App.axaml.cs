using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DownloadManager.Models;
using DownloadManager.Services;
using DownloadManager.ViewModels;
using DownloadManager.Views;
using Microsoft.Data.SqlClient;
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

        
        // because i'm using service provider in this class, i decided to make it private nested class,
        // so that it wouldn't be exposed to other classes
        private partial class ViewModelFactory<TBase> : IViewModelFactory<TBase>
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
                .AddSingleton<MainViewModel>()
                .AddSingleton<IViewModelFactory<ViewModelBase>, ViewModelFactory<ViewModelBase>>()
                .AddScoped<SqlConnection>()
                .AddSingleton<INavigationStore<ViewModelBase>, NavigationStore>()
                .AddSingleton<INavigationService<ViewModelBase>, NavigationService>()
                .BuildServiceProvider();
        }
    }
}