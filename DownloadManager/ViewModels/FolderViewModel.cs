using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DownloadManager.Models;
using DownloadManager.Services;
using FluentAvalonia.UI.Controls;

namespace DownloadManager.ViewModels;

public partial class FolderViewModel : ViewModelBase
{
    private readonly IFolderPicker _picker;
    private readonly IDialog _dialog;
    private readonly IDownloadDbClient _dbClient;
    private readonly IViewModelFactory _factory;

    [ObservableProperty]
    private DownloadableItem? _selectedItem;

    [ObservableProperty]
    private string? _searchText;

    [ObservableProperty]
    private string? _selectedSearchParameter;

    public ObservableCollection<string> SearchParameters { get; } = new();
    
    
    

    [ObservableProperty] 
    private ObservableCollection<DownloadableItem> _items = new();


    public FolderViewModel(IFolderPicker picker, IDialog dialog, IDownloadDbClient dbClient, IViewModelFactory factory)
    {
        _picker = picker;
        _dialog = dialog;
        _dbClient = dbClient;
        _factory = factory;
        SearchParameters.Add("Name");
        SearchParameters.Add("Tag");
    }

    [RelayCommand]
    private async Task Initialized()
    {
        Items = new ObservableCollection<DownloadableItem>(await _dbClient.GetAllDownloadsAsync());
    }


    [RelayCommand]
    private void Search()
    {
        if (SelectedSearchParameter is null || SearchText is null)
            return;
        if (SelectedSearchParameter is "Name")
        {
            Items = new ObservableCollection<DownloadableItem>
                (Items.Where(x => x.Name != null && x.Name.StartsWith(SearchText)));
        }
        if (SelectedSearchParameter is "Tag")
        {
            Items = new ObservableCollection<DownloadableItem>(Items.Where(x =>
                x.Tags != null && x.Tags.Any(y => y.Name != null && y.Name.StartsWith(SearchText))));
        }
    }
    
    

    [RelayCommand]
    private async Task RemoveAsync()
    {
        if (SelectedItem is null)
            return;
        var itemToBeRemoved = SelectedItem;
        Items.Remove(itemToBeRemoved);
        await _dbClient.RemoveDownloadAsync(itemToBeRemoved);
    }
    
    [RelayCommand]
    private async Task MoveAsync()
    {
        if (SelectedItem?.InstalledPath is null)
            return;
        var newPath = await _picker.GetPathAsync();
        if (newPath is null)
        {
            await _dialog.ShowMessageAsync("Error", "There was no path provided");
            return;
        }
        newPath += newPath + $@"\{SelectedItem.Name}";
        try
        {
            File.Move(SelectedItem.InstalledPath + $@"\{SelectedItem.Name}", newPath);
            SelectedItem.InstalledPath = newPath;
            await _dbClient.EditDownloadAsync(SelectedItem);
        }
        catch (IOException)
        {
            await _dialog.ShowMessageAsync("Error", "There is already a file with that name in the selected folder");
        }
        catch (Exception)
        {
            await _dialog.ShowMessageAsync("Error", "There was something wrong while moving the file");
        }
    }
    
    [RelayCommand]
    private async Task RenameAsync()
    {
        if (SelectedItem?.InstalledPath is null)
            return;
        var contentVm = _factory.Create<AddTagViewModel>();
        var result = await _dialog.ShowContentAsync("Enter file's new name", contentVm, 
            "OK", "Cancel");
        if (result != ContentDialogResult.Primary || contentVm.Tag is null)
            return;
        var directory = SelectedItem.InstalledPath;
        var extension = Path.GetExtension(SelectedItem.Name);
        var newDirectory = $@"{directory}\{contentVm.Tag}{extension}";
        try
        {
            File.Move(SelectedItem.InstalledPath + $@"\{SelectedItem.Name}", newDirectory);
        }
        catch (Exception)
        {
            await _dialog.ShowMessageAsync("Error", "There was something wrong while renaming the file");
        }
        SelectedItem.InstalledPath = newDirectory;
        SelectedItem.Name = Path.GetFileName(newDirectory);
        await _dbClient.EditDownloadAsync(SelectedItem);
    }
}