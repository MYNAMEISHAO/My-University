using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-90)]
public class SpawnerUI : MonoBehaviour
{
    [SerializeField] private SpawnerConfig spawnerConfig;
    private SpawnerData spawnerData;

    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI spawnrateText;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI priceText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        GameEvents.OnSpawnerLevelChanged += UpdateSpawnerLevel;
        GameEvents.OnCoinChanged += UpdateSpawnerButton;
    }
    private void OnDisable()
    {
        GameEvents.OnSpawnerLevelChanged -= UpdateSpawnerLevel;
        GameEvents.OnCoinChanged -= UpdateSpawnerButton;
    }

    private void Awake()
    {
        spawnerData = SpawnerManager.Instance.GetSpawnerData();
        spawnerConfig = SpawnerManager.Instance.GetSpawnerConfig();
    }
    void Start()
    {
        UpdateSpawnerLevel(spawnerData.currentLevel);
        UpdateSpawnerButton(PlayerManager.Instance.GetCoin());
    }
    // Update is called once per frame
    private void UpdateSpawnerLevel(int level)
    {
        levelText.text = "Level " + level;
        int price = (int)(spawnerConfig.upgradeConfig.costCurve.Evaluate(level) * 1000);
        if(level == spawnerConfig.maxLevel)
        {
            priceText.text = "MAX";
            upgradeButton.interactable = false;
            upgradeButton.transform.GetChild(0).gameObject.SetActive(false);
            upgradeButton.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
            priceText.text = price.ToString("N0");
        int spawnRate = (int)spawnerConfig.upgradeConfig.valueCurve.Evaluate(level);
        spawnrateText.text = "Spawn Rate: " + spawnRate + " / min";
        progressBar.fillAmount = (float)level / spawnerConfig.maxLevel;
    }
    private void UpdateSpawnerButton(int currentCoins)
    {
        if (currentCoins >= spawnerConfig.upgradeConfig.costCurve.Evaluate(spawnerData.currentLevel) * 1000 && spawnerData.currentLevel != spawnerConfig.maxLevel)
        {
            upgradeButton.interactable = true;
            upgradeButton.transform.GetChild(0).gameObject.SetActive(true);
            upgradeButton.transform.GetChild(1).gameObject.SetActive(false);
        }
        else 
        {
            upgradeButton.interactable = false;
            upgradeButton.transform.GetChild(0).gameObject.SetActive(false);
            upgradeButton.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void OnClickUpgradeSpawner()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        SpawnerManager.Instance.UpgradeSpawnerData();

    }

    public void OnClickCloseSpawnerUI()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        UIManager.Instance.CloseSpawnerPanel();
    }
}
