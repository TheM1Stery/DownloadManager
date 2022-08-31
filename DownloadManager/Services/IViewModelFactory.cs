using System;
using DownloadManager.ViewModels;

namespace DownloadManager.Services;

public interface IViewModelFactory
{
    public ViewModelBase Create<T>() where T: ViewModelBase;

    public ViewModelBase Create(Type viewModelType);
}