using System;

[Serializable]
public class ItemStackData
{
    public string itemID;
    public int amount;

    public ItemStackData(string itemID, int amount)
    {
        this.itemID = itemID;
        this.amount = amount;
    }
}
