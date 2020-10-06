using System;

public enum ItemType
{
    SWORD,
    POTION,
    SHIELD
}

public class Item
{
    public Guid Id { get; set; }
    public int Level { get; set; }
    public ItemType Type { get; set; }

}