using NUnit.Framework.Interfaces;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private TextMeshProUGUI txtQuantity;
    [SerializeField] private TextMeshProUGUI txtPrice;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject purchasedMask;

    private ShopItemData currentData;
    private ShopItemConfig currentItemConfig;

    private void Awake()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(OnBuyClicked);
    }

    private void OnEnable()
    {
        GameEvents.OnDiamondChanged += HandleOnDiamondChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnDiamondChanged -= HandleOnDiamondChanged;
    }

    private void HandleOnDiamondChanged(int diamond)
    {
        UpdateBuyButtonState();
    }

    public void DisplayItem(ShopItemData data, ShopItemConfig config)
    {
        if (data == null || config == null) return; 
        currentData = data;
        currentItemConfig = config;

        if (itemIcon != null) itemIcon.sprite = config != null ? config.Icon : null;
        if (txtName != null) txtName.text = config != null ? config.itemName : data.itemName;
        if (txtQuantity != null) txtQuantity.text = $"x{data.DisplayQuantity}";
        if (txtPrice != null) txtPrice.text = data.FinalDiamondCost.ToString();
        
        RefreshUI();
    }

    private void OnBuyClicked()
    {
        if (currentData == null || currentData.IsPurchased) return;

        bool success = PlayerManager.Instance.SpendDiamond(currentData.FinalDiamondCost);
        if (!success) return;
        if (currentItemConfig.itemName == "Vàng") PlayerManager.Instance.AddCoin(currentData.DisplayQuantity);
        else InventoryManager.Instance.AddItem(currentData.IdConfig, currentData.DisplayQuantity);

        currentData.IsPurchased = true;
        SaveManager.Instance?.SaveGame();
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (currentData == null) return;

        if (purchasedMask != null)
            purchasedMask.SetActive(currentData.IsPurchased);
        UpdateBuyButtonState();
    }
    private void UpdateBuyButtonState()
    {
        if (buyButton == null || currentData == null) return;
        bool isPurchased = currentData.IsPurchased;
        bool canAfford = PlayerManager.Instance.GetDiamond() >= currentData.FinalDiamondCost;

        buyButton.interactable = !isPurchased && canAfford;
    }
}
