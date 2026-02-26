using System;
using UnityEngine;
[DefaultExecutionOrder(-96)]

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    private PlayerSaveData playerData;
    [Header("Cấu hình Năng lượng")]
    public int maxEnergy = 100;
    [SerializeField] int currentEnergy;
    [SerializeField] float restoreTimeInSeconds = 10f; // 2 phút = 120 giây
    public float timer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.gameData != null)
        {
            PlayerManager.Instance.Initialize(SaveManager.Instance.gameData.player);
            currentEnergy = playerData.Energy;
            timer = restoreTimeInSeconds;
        }
        else
        {
            Debug.LogError("SaveManager chưa sẵn sàng hoặc gameData null!");
        }
    }

    private void Update()
    {
        if (currentEnergy < maxEnergy)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                PlayerManager.Instance.AddEnergy(1);
                timer = restoreTimeInSeconds; // Reset lại 2 phút
            }
        }
        else
        {
            timer = 0; // Reset timer khi đầy
        }
    }

    public void Initialize(PlayerSaveData data)
    {
        playerData = data;
    }

    public int GetCoin() => playerData.playerCoin;
    public int GetDiamond() => playerData.playerDiamond;
    public int GetFame() => playerData.playerFame;
    public int GetResearchPoint() => playerData.researchPoints;
    public int GetEnergy() => playerData.Energy;
    public bool SpendCoin(int amount)
    {
        if (amount < 0 || playerData.playerCoin < amount)
            return false;

        playerData.playerCoin -= amount;
        SaveManager.Instance.SaveGame();
        GameEvents.OnCoinChanged?.Invoke(playerData.playerCoin);
        return true;
    }

    public void AddCoin(int amount)
    {
        if (amount < 0) return;
        playerData.playerCoin += amount;
        SaveManager.Instance.SaveGame();
        GameEvents.OnCoinChanged?.Invoke(playerData.playerCoin);
    }

    public void AddFame(int amount)
    {
        if (amount < 0) return;
        playerData.playerFame += amount;
        SaveManager.Instance.SaveGame();
        GameEvents.OnFameChanged?.Invoke(playerData.playerFame);
    }

    public void AddDiamond(int amount)
    {
        if (amount < 0) return;
        playerData.playerDiamond += amount;
        SaveManager.Instance.SaveGame();
        GameEvents.OnDiamondChanged?.Invoke(playerData.playerDiamond);
    }

    public bool SpendDiamond(int amount)
    {
        if (amount < 0) return false;
        if (playerData.playerDiamond >= amount)
        {
            playerData.playerDiamond -= amount;
            SaveManager.Instance.SaveGame();
            GameEvents.OnDiamondChanged?.Invoke(playerData.playerDiamond);
        }
        return true;
    }

    public void AddResearchPoint(int amount)
    {
        if (amount < 0) return;
        playerData.researchPoints += amount;
        SaveManager.Instance.SaveGame();
        GameEvents.OnResearchPointChanged?.Invoke(playerData.researchPoints);
    }

    public void SpendResearchPoint(int amount)
    {
        if (amount < 0) return;
        playerData.researchPoints -= amount;
        SaveManager.Instance.SaveGame();
        GameEvents.OnResearchPointChanged?.Invoke(playerData.researchPoints);
    }

    public void AddEnergy(int amount)
    {
        if (amount < 0) return;
        playerData.Energy += amount;
        currentEnergy = playerData.Energy;
        SaveManager.Instance.SaveGame();
        GameEvents.OnEnergyChanged?.Invoke(playerData.Energy);
    }

    public void SpendEnergy(int amount)
    {
        if (amount <= 0) return;
        playerData.Energy -= amount;
        currentEnergy = playerData.Energy;
        SaveManager.Instance.SaveGame();
        GameEvents.OnEnergyChanged?.Invoke(playerData.Energy);
    }

    public long GetLastLogoutTicks() => playerData.lastLogoutTimeTicks;
    public void UpdateLogoutTime()
    {
        playerData.lastLogoutTimeTicks = DateTime.Now.Ticks;
        SaveManager.Instance.SaveGame();
    }

}
