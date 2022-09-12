using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DownloadManager.Models;

namespace DownloadManager.Services;

public class DownloadDbClient : IDownloadDbClient
{
    private readonly DbProviderFactory _factory;
    private readonly string _connectionString;

    public DownloadDbClient(DbProviderFactory factory, string connectionString)
    {
        _factory = factory;
        _connectionString = connectionString;
    }

    private DbConnection CreateConnection()
    {
        var connection = _factory.CreateConnection();
        connection!.ConnectionString = _connectionString;
        return connection;
    }
    
    
    public async Task InitializeAsync()
    {
        await using var connection = CreateConnection();
        var list = 
            await connection.QueryAsync("SELECT name FROM sqlite_master WHERE type='table' AND name='Downloads'");
        if (list.Any())
        {
            return;
        }
        await connection.ExecuteAsync(await File.ReadAllTextAsync("downloaddb.sql"));
    }

    public async Task AddDownloadAsync(DownloadableItem downloadableItem)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync("INSERT INTO Downloads(Name, Size, InstalledPath, LinkToDownload, IsFinished) " +
                                      "VALUES(@Name, @Size, @InstalledPath, @LinkToDownload, 0)", 
            new {downloadableItem.Name, downloadableItem.Size, downloadableItem.InstalledPath, 
                downloadableItem.LinkToDownload});
        var item = await connection.QueryFirstAsync<DownloadableItem>("SELECT * FROM Downloads ORDER BY ID DESC LIMIT 1");
        var items = downloadableItem.Tags?.Select(x => new Tag{Name = x.Name, DownloadId = item.Id})
            .ToList();
        await connection.ExecuteAsync("INSERT INTO Tags(Name, DownloadId) VALUES(@Name, @DownloadId)", items);
    }


    public List<DownloadableItem> GetAllDownloads()
    {
        using var connection = CreateConnection();
        var tags = connection.Query<Tag>("SELECT * FROM Tags");
        var downloads = connection.Query<DownloadableItem>("SELECT * FROM Downloads").ToList();
        if (downloads.Count == 0)
            return Enumerable.Empty<DownloadableItem>().ToList();
        downloads.ForEach(x => x.Tags = tags.Where(y => y.DownloadId == x.Id).ToList());
        return downloads;
    }

    public async Task<DownloadableItem> GetLatestDownload()
    {
        await using var connection = CreateConnection();
        
        return await connection.QueryFirstAsync<DownloadableItem>("SELECT * FROM Downloads ORDER BY ID DESC LIMIT 1");
    }

    public async Task RemoveDownloadAsync(DownloadableItem downloadableItem)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync("DELETE FROM Downloads WHERE ID = @Id", new {downloadableItem.Id});
    }

    public async Task EditDownloadAsync(DownloadableItem downloadableItem)
    {
        await using var connection = CreateConnection();
        await connection.ExecuteAsync("UPDATE Downloads SET Name = @Name, Size = @Size, " +
                                      "InstalledPath = @InstalledPath, LinkToDownload = @LinkToDownload, " +
                                      "IsFinished = @IsFinished WHERE Id = @Id",
            new
            {
                downloadableItem.Name, downloadableItem.Size, downloadableItem.InstalledPath,
                downloadableItem.LinkToDownload,downloadableItem.IsFinished,
                downloadableItem.Id
            });
    }

    public async Task<List<DownloadableItem>> GetAllDownloadsAsync()
    {
        await using var connection = CreateConnection();
        var tags = await connection.QueryAsync<Tag>("SELECT * FROM Tags");
        var downloads = (await connection.QueryAsync<DownloadableItem>("SELECT * FROM Downloads")).ToList();
        if (downloads.Count == 0)
            return Enumerable.Empty<DownloadableItem>().ToList();
        downloads.ForEach(x => x.Tags = tags.Where(y => y.DownloadId == x.Id).ToList());
        return downloads;
    }
}