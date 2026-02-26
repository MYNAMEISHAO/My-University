using System.Collections.Generic;

[System.Serializable]
public class RoomData
{
    public string roomConfigID;
    public bool isUnlocked;
    public int currentLevel;
    public int SeatCount;

    public string currentTeacherID;
    public RoomData() { }

    public RoomData(string id, bool unlocked, int level, int seatCount, string idTeacher)
    {
        this.roomConfigID = id;
        this.isUnlocked = unlocked;
        this.currentLevel = level;
        this.SeatCount = seatCount;

        this.currentTeacherID = idTeacher;
    }
}