using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfflineRewardUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI txtMessage;
    [SerializeField] private Button btnClose;

    public int _rewardCoin;
    private void Awake()
    {
        btnClose.onClick.AddListener(OnCloseClicked);
    }
    private void OnDestroy()
    {
        GameEvents.OnOfflineGoldCalculated -= Show;
    }
    public void Show(int coin)
    {
        gameObject.SetActive(true);
        _rewardCoin = coin;
        txtMessage.text = $"Chào mừng bạn quay trở lại!\n"
            + $"Bạn đã nhận được {coin:N0} vàng";
        PlayerManager.Instance.AddCoin(coin);
    }

    private void OnCloseClicked()
    {
        _rewardCoin = 0;
        UIManager.Instance.CloseOfflineReward();
    }
}