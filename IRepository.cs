using System;
using System.Threading.Tasks;
using MongoDB.Driver;
public interface IRepository
{
    Task<Player> CreatePlayer(Player player);
    Task<Player> GetPlayer(Guid playerId);
    Task<Player[]> GetAllPlayers();
    Task<Player[]> GetFullLeaderboard();
    Task<Player> UpdatePlayer(Guid id, ModifiedPlayer player);
    Task<UpdateResult> UpdatePlayerScore(Guid id, int AddToScore);
    Task<Player> DeletePlayer(Guid playerId);
    Task<Player[]> GetTopXPlayers(int x);
    Task<Player> GetXthPlayer(int Xth);

    Task<Item> CreateItem(Guid playerId, Item item);
    Task<Item> GetItem(Guid playerId, Guid itemId);
    Task<Item[]> GetAllItems(Guid playerId);
    Task<Item> UpdateItem(Guid playerId, Item item);
    Task<Item> DeleteItem(Guid playerId, Item item);

    void DeleteAll();
}