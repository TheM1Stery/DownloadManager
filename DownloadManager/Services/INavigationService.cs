namespace DownloadManager.Services;

public interface INavigationService<in TBase>
{
    public void Navigate<T>() where T: notnull, TBase;
}