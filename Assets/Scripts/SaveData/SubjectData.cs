using System;

[Serializable]
public class SubjectData
{
    public string SubjectConfigID;
    public bool Status;
    public int currentLevel;
    public string roomID;
    public SubjectData() { }

    public SubjectData(string id, bool status, int level, string roomID)
    {
        this.SubjectConfigID = id;
        this.Status = status;
        this.currentLevel = level;
        this.roomID = roomID;
    }
}