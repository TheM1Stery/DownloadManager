using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DownloadManager.Messages;
using DownloadManager.Models;
using DownloadManager.Services;
using FluentAvalonia.UI.Controls;

namespace DownloadManager.ViewModels;

public partial class DownloadViewModel : ViewModelBase
{
    private readonly IFolderPicker _folderPicker;
    private readonly IViewModelFactory _factory;
    public ObservableCollection<DownloadableItemViewModel> Downloads { get; } = new();

    public ObservableCollection<int> ThreadNumbers { get; } = new(Enumerable.Range(1, 6));

    public ObservableCollection<string> Tags { get; } = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadCommand))]
    private string? _link;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadCommand))]
    private string? _path;

    [ObservableProperty]
    private int _numberOfThreads = 1;
    

    public bool CanExecuteDownload =>
        !string.IsNullOrWhiteSpace(Link) && !string.IsNullOrWhiteSpace(Path) && Tags.Count != 0;

    public DownloadViewModel(IFolderPicker folderPicker, IViewModelFactory factory)
    {
        _folderPicker = folderPicker;
        _factory = factory;
        Tags.CollectionChanged += TagsOnCollectionChanged;
    }

    private void TagsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        DownloadCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private async Task SelectPathAsync()
    {
        var path = await _folderPicker.GetPathAsync();
        if (path is null)
        {
            var dialog = new ContentDialog()
            {
                Title = "Error", Content = "Error getting the file", PrimaryButtonText = "OK"
            };
            await dialog.ShowAsync();
            return;
        }
        Path = path;
    }

    [RelayCommand]
    private async Task AddTagAsync()
    {
        var viewModel = _factory.Create<AddTagViewModel>();
        var dialog = new ContentDialog
        {
            Title = "Enter your tag",
            Content = viewModel,
            PrimaryButtonText = "Add",
            SecondaryButtonText = "Cancel"
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary && viewModel.Tag is not null)
        {
            Tags.Add(viewModel.Tag);
        }
    }

    [RelayCommand]
    private void DeleteItemFromTags(string tag)
    {
        Tags.Remove(tag);
    }

    [RelayCommand]
    private void ResetOptions()
    {
        Tags.Clear();
        Path = string.Empty;
        Link = string.Empty;
        NumberOfThreads = 1;
    }

    [RelayCommand(CanExecute = nameof(CanExecuteDownload))]
    private void Download()
    {
        Downloads.Add(_factory.Create<DownloadableItemViewModel>());
        WeakReferenceMessenger.Default.Send(new DownloadItemMessage((new DownloadableItem()
        {
            InstalledPath = Path,
            Name = "Seymur.txt",
            LinkToDownload = Link,
            Tags = Tags.ToList()
        }, _numberOfThreads)));
        ResetOptions();
    }
}