using System;
using System.Collections.Generic;

public class PlayerList 
{
    public List<Player> playerList = new List<Player>();
}
public class Player
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
    public int Level { get; set; }
    public bool IsBanned { get; set; }
    public DateTime CreationTime { get; set; }
    public List<Item> items = new List<Item>();
    
    public bool ScoreLessThan (Player p)
    {
        return (Score < p.Score);
    }
}