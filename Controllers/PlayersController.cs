using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("apiproject/players")]
public class PlayersController : ControllerBase
{
    private readonly IRepository repo;
    public PlayersController(IRepository repository) 
    {
        repo = repository;
    }

    [Route("{id}")]
    [HttpGet]
    public Task<Player> Get(Guid id) {
        try
        {
            return repo.GetPlayer(id);
        } 
        catch(NotFoundException m) {
            Console.WriteLine("NotFoundException: " + m);
            return null;
        }
    }

    [Route("")]
    [HttpGet]
    public Task<Player[]> GetAll() {
        return repo.GetAllPlayers();
    }

    [Route("leaderboard")]
    [HttpGet]
    public Task<Player[]> GetFullLeaderboard() {
        return repo.GetFullLeaderboard();
    }

    [Route("new/{name}")]
    [HttpPost]
    public Task<Player> Create(string name) {
        Player newPlayer = new Player();

        newPlayer.Id = Guid.NewGuid();
        newPlayer.IsBanned = false;
        newPlayer.Level = 1;
        newPlayer.Name = name;
        newPlayer.Score = 0;
        newPlayer.CreationTime = DateTime.Now;

        return repo.CreatePlayer(newPlayer);
    }

    [Route("modify/{id}/modified")]
    [HttpPost]
    public Task<Player> Modify(Guid id, [FromBody] ModifiedPlayer player) {
        return repo.UpdatePlayer(id, player);
    }

    [HttpPost]
    [Route("scorechange/{id}")]
    public async Task<Player> UpdatePlayerScore(Guid id, [FromBody] int AddToScore)
    {
        await repo.UpdatePlayerScore(id, AddToScore);
        return null;
    }

    [Route("delete/{id}")]
    [HttpDelete]
    public Task<Player> Delete(Guid id) {
        return repo.DeletePlayer(id);
    }

    [Route("top/{x}")]
    [HttpGet]
    public Task<Player[]> GetTopXPlayers(int x) {
        return repo.GetTopXPlayers(x);
    }

    [Route("xth/{x}")]
    [HttpGet]
    public Task<Player> GetXthPlayer(int x) {
        return repo.GetXthPlayer(x);
    }

    [Route("minScore/{min}")]
    [HttpGet]
    public Task<Player[]> GetPlayersWithAboveScore(int min) {
        return repo.GetPlayersWithAboveScore(min);
    }

    [HttpPost]
    [Route("{id}/updateItemScore")]
    public async Task<Player> UpdatePlayerItemScore(Guid id)
    {
        await repo.UpdatePlayerItemScore(id);
        return null;
    }

    /*-------- DELETE DATABASE | DEBUG ONLY ---------*/

    [Route("deleteAll")]
    [HttpDelete]
    public void DeleteAll() {
        repo.DeleteAll();
    }
}