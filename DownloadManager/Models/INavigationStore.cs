namespace DownloadManager.Models;

public interface INavigationStore<T> where T: class
{
    public T? CurrentViewModel { get; set; } 
}