using System;
using Microsoft.Extensions.DependencyInjection;

namespace DownloadManager;

public partial class App
{
    private partial class ViewModelFactory<TBase>
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TBase Create<TViewModel>() where TViewModel : notnull, TBase
        {
            return _serviceProvider.GetRequiredService<TViewModel>();
        }
    }
}