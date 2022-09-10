using System;
using DownloadManager.ViewModels;

namespace DownloadManager.Services;

public interface IViewModelFactory
{
    public T Create<T>() where T: ViewModelBase;

    public ViewModelBase Create(Type viewModelType);
}