using System;

[Serializable]
public class TeacherData
{
    public string teacherConfigID;
    public bool teacherStatus;
    public int teacherLevel;

    public TeacherData() { }
    public TeacherData(string teacherID, bool teacherStatus, int teacherLevel)
    {
        this.teacherConfigID = teacherID;
        this.teacherStatus = teacherStatus;
        this.teacherLevel = teacherLevel;
    }
}
