using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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

    [Route("modify/{id}")]
    [HttpPost]
    public Task<Item> UpdateItem(Guid playerId, Item item)
    {
        return repo.UpdateItem(playerId, item);
    }

    [Route("delete/{id}")]
    [HttpDelete]
    public Task<Item> DeleteItem(Guid playerId, Item item)
    {
        return repo.DeleteItem(playerId, item);
    }
}