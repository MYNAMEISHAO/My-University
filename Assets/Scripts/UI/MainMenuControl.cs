using UnityEngine;
using UnityEngine.UI;

public class MainMenuControl : MonoBehaviour
{

    [Header("Currency UI")]
    public TMPro.TextMeshProUGUI coinText;
    public TMPro.TextMeshProUGUI diamondText;   
    public TMPro.TextMeshProUGUI fameText;

    [Header("Buttons")]
    public GameObject footer;
    public Button missionButton;
    public Button giftButton;
    public Button settingsButton;
    public Button spawnerButton;

    [Header("Panels")]
    public GameObject mainMenuPanel;        // Panel chính (MainMenuPanel)
    public GameObject header;

    private void Start()
    {
        OpenIconButton();

        GameEvents.OnCoinChanged += UpdateCoinUI;
        GameEvents.OnDiamondChanged += UpdateDiamondUI;
        GameEvents.OnFameChanged += UpdateFameUI;

        if (PlayerManager.Instance != null)
        {
            UpdateCoinUI(PlayerManager.Instance.GetCoin());
            UpdateDiamondUI(PlayerManager.Instance.GetDiamond());
            UpdateFameUI(PlayerManager.Instance.GetFame());
        }
        OpenIconButton();
    }
    private void OnDestroy()
    {
        GameEvents.OnCoinChanged -= UpdateCoinUI;
        GameEvents.OnDiamondChanged -= UpdateDiamondUI;
        GameEvents.OnEnergyChanged -= UpdateFameUI;
    }

    void UpdateCoinUI(int amount)
    {
        if (coinText != null) coinText.text = amount.ToString();
    }

    void UpdateDiamondUI(int amount)
    {
        if (diamondText != null) diamondText.text = amount.ToString();
    }
    
    void UpdateFameUI(int amount)
    {
        if (fameText != null && PlayerManager.Instance != null) 
            fameText.text = amount.ToString();
    }
    public void OnCharacterButtonClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowTeacherListPanel();
        }
    }

    public void OnSubjectButtonClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowSubjectsListPanel();
        }
    }

    public void OnShopButtonClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowShopPanel();
        }
    }

    public void OnResearchButtonClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowResearchUI();
        }
    }

    public void OnMissionButtonClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMissionPanel();
        }
    }

    public void OnGiftButtonClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGiftPanel();
        }
    }
    public void OnSpawnerButtonClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowSpawnerPanel();
        }
    }
    public void OnSettingButtonClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowSettingPanel();
        }
    }
    public void CloseIconButton()
    {
        missionButton.gameObject.SetActive(false);
        giftButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        spawnerButton.gameObject.SetActive(false);
        footer.SetActive(false);
    }
    public void OpenIconButton()
    {
        missionButton.gameObject.SetActive(true);
        giftButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        spawnerButton.gameObject.SetActive(true);
        footer.SetActive(true);
    }
    public void CloseHeader()
    {
        header.SetActive(false);
    }
    public void OpenHeader()
    {
        header.SetActive(true);
    }
}
