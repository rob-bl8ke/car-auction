using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionSvcHttpClient
{
    private readonly HttpClient httpClient;
    private readonly IConfiguration config;

    public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
    {
        this.httpClient = httpClient;
        this.config = config;
    }

    public async Task<List<Item>> GetItemsForSearchDb()
    {
        // Find the last updated date in our Search DB
        var lastUpdated = await DB.Find<Item, string>()
            .Sort(x => x.Descending(x => x.UpdatedAt))
            .Project(x => x.UpdatedAt.ToString())
            .ExecuteFirstAsync();

        // Fetch all items that have a date larger than the last updated date.
        return await httpClient.GetFromJsonAsync<List<Item>>(config["AuctionServiceUrl"] + 
            "/api/auctions?date=" + lastUpdated);
    }
}
