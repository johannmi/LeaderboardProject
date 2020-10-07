using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

[ApiController]
[Route("apiproject/players/{playerId}/items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository repo;
    public ItemsController(IRepository repository)
    {
        repo = repository;
    }

    [Route("{id}")]
    [HttpGet]
    public Task<Item> Get(Guid playerId, Guid itemId)
    {
        return repo.GetItem(playerId, itemId);
    }

    [Route("")]
    [HttpGet]
    public Task<Item[]> GetAllItems(Guid playerId)
    {
        return repo.GetAllItems(playerId);
    }

    [Route("new/newItem")]
    [HttpPost]
    public Task<Item> CreateItem(Guid playerId, [FromBody] NewItem item)
    {

        Item newItem = new Item();
        newItem.Id = Guid.NewGuid();
        newItem.Level = item.Level;
        newItem.Type = item.Type;

        return repo.CreateItem(playerId, newItem);
    }

    [Route("modify")]
    [HttpPost]
    public Task<Item> UpdateItem(Guid playerId, [FromBody] Item item)
    {
        return repo.UpdateItem(playerId, item);
    }

    [Route("delete")]
    [HttpDelete]
    public Task<Item> DeleteItem(Guid playerId, [FromBody] Item item)
    {
        return repo.DeleteItem(playerId, item);
    }

    [HttpPost] //{"Name":"yeet"}
        
        [Route("createSword")]
        public async Task<Item> CreateSword([FromBody] NewSword sword, Guid playerId)
        {
            DateTime localDate = DateTime.UtcNow;
            int ownerLvl = await repo.GetPlayerLevel(playerId);

            Sword new_sword = new Sword();
            LevelValidator validator = new LevelValidator();
            new_sword.Id = Guid.NewGuid();
            new_sword.OwnerLevel = ownerLvl;
            new_sword.Level = 1;
            new_sword.SwordType = sword.SwordType;
            validator.ValidateAndThrow(new_sword);

            await repo.CreateSword(playerId, new_sword);
            return new_sword;
        }
}