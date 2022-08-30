namespace DownloadManager.Services;

public interface IViewModelFactory<TBaseViewModel>
{
    public TBaseViewModel Create<TViewModel>() where TViewModel : notnull, TBaseViewModel;
}