using UnityEngine;
[System.Serializable]
public class BookData
{
    public int Level;
    public int Count;

    public BookData(int level, int count)
    {
        this.Level = level;
        this.Count = count;
    }
}
