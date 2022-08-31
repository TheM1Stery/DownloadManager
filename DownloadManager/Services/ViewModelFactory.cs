using System;
using DownloadManager.ViewModels;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace DownloadManager;

public partial class App
{
    private partial class ViewModelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ViewModelBase Create<T>() where T : ViewModelBase
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        public ViewModelBase Create(Type viewModelType)
        {
            return (ViewModelBase) _serviceProvider.GetRequiredService(viewModelType);
        }
    }
}