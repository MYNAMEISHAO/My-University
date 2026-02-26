using System;
using UnityEngine;

public class OfflineRevenueManager : MonoBehaviour
{
    [Header("Thu nhap ngoai tuyen")]
    public int goldPerSecond;
    public float minSeconds = 900f;
    public float maxSeconds = 14400f;

    private void Start()
    {
        Invoke(nameof(CalculateOfflineReward), 0.5f);
    }

    private void OnApplicationQuit() => SaveExitTime();
    private void OnApplicationPause(bool pauseStatus) { if (pauseStatus) SaveExitTime(); }

    private void SaveExitTime()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.UpdateLogoutTime();
        }
    }
    private void CalculateOfflineReward()
    {
        long lastLogout = PlayerManager.Instance.GetLastLogoutTicks();
        if (lastLogout == 0) return;

        DateTime logoutTime = new DateTime(lastLogout);
        TimeSpan difference = DateTime.Now - logoutTime;

        double secondsAway = difference.TotalSeconds;

        if (secondsAway < minSeconds) return;
        if (secondsAway > maxSeconds) secondsAway = maxSeconds;

        int rewardGold = (int)Math.Floor(secondsAway * goldPerSecond * 0.05);

        if (rewardGold > 0)
        {
            GameEvents.OnOfflineGoldCalculated?.Invoke(rewardGold);
            PlayerManager.Instance.UpdateLogoutTime();
            Debug.Log($"<color=green>OFFLINE GOLD CALCULATED: {rewardGold}</color>");
        }
    }
}
