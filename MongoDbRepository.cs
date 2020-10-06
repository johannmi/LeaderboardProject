using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

public class MongoDbRepository : IRepository
{
    private readonly IMongoCollection<Player> _playerCollection;
    private readonly IMongoCollection<BsonDocument> _bsonDocumentCollection;

    public MongoDbRepository()
    {
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var database = mongoClient.GetDatabase("LeaderboardProject");
        _playerCollection = database.GetCollection<Player>("players");

        _bsonDocumentCollection = database.GetCollection<BsonDocument>("players");
    }

    public async Task<Player> CreatePlayer(Player player)
    {
        await _playerCollection.InsertOneAsync(player);
        return player;
    }

    public async Task<Player> DeletePlayer(Guid id)
    {
        FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, id);
        return await _playerCollection.FindOneAndDeleteAsync(filter);
    }

    public async Task<Player> GetPlayer(Guid id)
    {
        try {
            var filter = Builders<Player>.Filter.Eq(player => player.Id, id);
            return await _playerCollection.Find(filter).FirstAsync();
        } catch {
            throw new NotFoundException("404 Not Found");
        }
    }

    public async Task<Player[]> GetAllPlayers()
    {
        var filter = Builders<Player>.Filter.Empty;
        var players = await _playerCollection.Find(filter).ToListAsync();
        return players.ToArray();
    }

    public async Task<Player> UpdatePlayer(Guid id, ModifiedPlayer player)
    {
        FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, id);
        Player returnPlayer = await _playerCollection.Find(filter).FirstAsync();
        returnPlayer.Score = player.Score;
        returnPlayer.IsBanned = player.IsBanned;
        returnPlayer.Level = player.Level;
        await _playerCollection.ReplaceOneAsync(filter, returnPlayer);
        return returnPlayer;
    }

    public async Task<UpdateResult> UpdatePlayerScore(Guid id, int AddToScore){
        FilterDefinition<Player> filter = Builders<Player>.Filter.Eq("Id", id);
        var update = Builders<Player>.Update.Inc("Score", AddToScore);
        return await _playerCollection.UpdateOneAsync(filter, update);
    }

    /* BUBBLE SORT !!
    public async Task<Player[]> GetFullLeaderboard()
    {
        var filter = Builders<Player>.Filter.Empty;
        var players = await _playerCollection.Find(filter).ToListAsync();
        players = players.Where(x => x.IsBanned == false).ToList();

        for (int j = 0; j <= players.Count - 2; j++) {
            for (int i = 0; i <= players.Count - 2; i++) {
                if (players[i].ScoreLessThan(players[i + 1])) {
                    Player temp = players[i + 1];
                    players[i + 1] = players[i];
                    players[i] = temp;
                }
            }
        }

        return players.ToArray();
    }
    */

    public async Task<Player[]> GetFullLeaderboard()
    {
        var filter = Builders<Player>.Filter.Empty;
        SortDefinition<Player> sortDef = Builders<Player>.Sort.Descending("Score");
        List<Player> players = await _playerCollection.Find(filter).Sort(sortDef).ToListAsync();
        players = players.Where(x => x.IsBanned == false).ToList();

        return players.ToArray();
    }

    public async Task<Player[]> GetTopXPlayers(int x)
    {
        Player[] players = await GetFullLeaderboard();
        players.ToList();
        return players.Take(x).ToArray();
    }

    public async Task<Player> GetXthPlayer(int Xth)
    {
        Player[] players = await GetFullLeaderboard();
        players.ToList();
        return players[Xth - 1];
    }

    public async Task<Player[]> GetPlayersWithAboveScore(int min)
    {
        FilterDefinition<Player> filter = 
            Builders<Player>.Filter.Gte("Score", min) & Builders<Player>.Filter.Eq(p=>p.IsBanned, false);
        SortDefinition<Player> sortDef = Builders<Player>.Sort.Descending("Score");

        List<Player> players = await _playerCollection.Find(filter).Sort(sortDef).ToListAsync();

        return players.ToArray();
    }

    /*------- ITEMS --------*/

    public async Task<Item> CreateItem(Guid playerId, Item item)
    {
        Player player = await GetPlayer(playerId);
        player.items.Add(item);
        var filter = Builders<Player>.Filter.Eq(player => player.Id, playerId);
        await _playerCollection.ReplaceOneAsync(filter, player);
        await UpdatePlayerItemScore(player.Id);
        return item;
    }
    public async Task<Item> GetItem(Guid playerId, Guid itemId)
    {
        Player player = await GetPlayer(playerId);

        for (int i = 0; i < player.items.Count; i++)
        {
            if (player.items[i].Id == itemId)
                return player.items[i];
        }

        return null;
    }
    public async Task<Item[]> GetAllItems(Guid playerId)
    {
        Player player = await GetPlayer(playerId);
        return player.items.ToArray();
    }

    public async Task<UpdateResult> UpdatePlayerItemScore(Guid playerId)
    {
        int itemScore = 0;
        Player player = await GetPlayer(playerId);
        
        for(int i = 0; i < player.items.Count; i++) {
            itemScore += player.items[i].Level;
        }

        FilterDefinition<Player> filter = Builders<Player>.Filter.Eq("Id", playerId);
        var update = Builders<Player>.Update.Set("ItemScore", itemScore);
        return await _playerCollection.UpdateOneAsync(filter, update);
    }
    public async Task<Item> UpdateItem(Guid playerId, Item item)
    {
        Player player = await GetPlayer(playerId);

        foreach (var i in player.items)
        {
            if (i.Id == item.Id)
            {
                i.Level = item.Level;
                var filter_player = Builders<Player>.Filter.Eq(player => player.Id, playerId);
                await _playerCollection.ReplaceOneAsync(filter_player, player);
                await UpdatePlayerItemScore(player.Id);
                return i;
            }
        }

        return null;
    }
    public async Task<Item> DeleteItem(Guid playerId, Item item)
    {
        Player player = await GetPlayer(playerId);

        for (int i = 0; i < player.items.Count; i++)
        {
            if (player.items[i].Id == item.Id)
            {
                player.items.RemoveAt(i);
                var filter_player = Builders<Player>.Filter.Eq(player => player.Id, playerId);
                await _playerCollection.ReplaceOneAsync(filter_player, player);
                await UpdatePlayerItemScore(player.Id);
                return item;
            }
        }

        return null;
    }

    /*-------- DELETE DATABASE | DEBUG ONLY ---------*/

    public void DeleteAll()
    {
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        mongoClient.DropDatabase("LeaderboardProject");
    }
}