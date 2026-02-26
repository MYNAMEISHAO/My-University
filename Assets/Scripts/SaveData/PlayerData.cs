using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public string ID;
    public string Name;
    public int playerCoin;
    public int playerDiamond;
    public int playerFame;
    public int Energy;
    public int researchPoints;
    public long lastLogoutTimeTicks;
}

