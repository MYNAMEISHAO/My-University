using System;
using UnityEngine;
[Serializable]
public class MissionData
{
    // 1-based ID theo code hiện tại; sửa nếu bạn dùng 0-based
    public int ID = 1;
    public MissionData(int id)
    {
        this.ID= id;
    }  
}