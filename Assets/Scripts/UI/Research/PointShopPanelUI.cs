using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(-90)] // Chạy sau SaveManager và InventoryManager
public class PointShopPanelUI : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform contentParent;
    private List<ItemStackData> lItem = new List<ItemStackData>();
    private List<ShopItemConfig> shopItemConfigs = new List<ShopItemConfig>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {

    }
    private void OnEnable()
    {
        if (SaveManager.Instance != null)
        {
            GameEvents.OnResearchPointChanged += HandlePointChanged;
            GameEvents.OnItemChanged += HandleItemChanged;
        }
    }

    private void HandleItemChanged(string arg1, int arg2)
    {
        foreach (Transform child in contentParent)
        {
            ItemSlotButton button = child.GetComponent<ItemSlotButton>();
            int index = button.ItemIndex;
            Transform itemAmountTranform = child.GetChild(4);
            TextMeshProUGUI itemAmount = itemAmountTranform.GetComponent<TextMeshProUGUI>();
            itemAmount.text = "Hiện sở hữu: " + InventoryManager.Instance.GetAllItems()[index].amount;
        }
    }

    private void HandlePointChanged(int obj)
    {
        Transform pointText = transform.GetChild(1).GetChild(1).GetChild(0);
        pointText.GetComponent<TextMeshProUGUI>().text = obj.ToString();

        UpdateButton();

    }

    private void OnDisable()
    {
        if (PlayerManager.Instance != null)
        {
            GameEvents.OnResearchPointChanged -= HandlePointChanged;
            GameEvents.OnItemChanged -= HandleItemChanged;
        }
    }

    private void Start()
    {
        shopItemConfigs = InventoryManager.GetShopItemConfigs();
        lItem = InventoryManager.Instance.GetAllItems();
        Debug.Log("Số lượng item trong kho: " + lItem.Count);
        Debug.Log("Số lượng item trong shop config: " + shopItemConfigs.Count);
        if (lItem != null && lItem.Count > 0)
        {
            ShowUI();
        }
        else
        {
            Debug.LogWarning("Dữ liệu vẫn rỗng. Kiểm tra InventoryManager.Initialize!");
        }
    }

    private void ShowUI()
    {

        //Get Research Point
        Transform pointText = transform.GetChild(1).GetChild(1).GetChild(0);
        pointText.GetComponent<TextMeshProUGUI>().text = PlayerManager.Instance.GetResearchPoint().ToString();

        for (int i = 0; i < lItem.Count; i++)
        {
            ItemStackData item = lItem[i];
            Debug.Log($"Item ID: {item.itemID}, Quantity: {item.amount}");
            GameObject obj = Instantiate(itemPrefab, contentParent);
            SetUpPrefabs(obj, i);
        }
        UpdateButton();
        SetUpIndex();
    }
    private void SetUpIndex()
    {
        for (int i = 0; i < contentParent.childCount; i++)
        {
            Transform Prefabtransform = contentParent.GetChild(i);
            ItemSlotButton button = Prefabtransform.GetComponent<ItemSlotButton>();
            button.ItemIndex = i;
        }
    }

    private void SetUpPrefabs(GameObject obj, int index)
    {
        Transform itemImageTranform = obj.transform.GetChild(0).GetChild(0);
        Image itemImage = itemImageTranform.GetComponent<Image>();
        itemImage.sprite = shopItemConfigs[index].Icon;

        Transform itemNameTranform = obj.transform.GetChild(1).GetChild(0);
        TextMeshProUGUI itemName = itemNameTranform.GetComponent<TextMeshProUGUI>();
        itemName.text = shopItemConfigs[index].itemName;

        Transform itemDescripTransform = obj.transform.GetChild(2);
        TextMeshProUGUI itemDescrip = itemDescripTransform.GetComponent<TextMeshProUGUI>();
        itemDescrip.text = shopItemConfigs[index].itemDescription;

        Transform itemCostTransform = obj.transform.GetChild(3).GetChild(1);
        TextMeshProUGUI itemCost = itemCostTransform.GetComponent<TextMeshProUGUI>();
        itemCost.text = "Giá:" + shopItemConfigs[index].tradePrice;

        Transform itemAmountTranform = obj.transform.GetChild(4);
        TextMeshProUGUI itemAmount = itemAmountTranform.GetComponent<TextMeshProUGUI>();
        itemAmount.text = "Hiện sở hữu: " + lItem[index].amount;

    }

    private void UpdateButton()
    {
        for (int i = 0; i < lItem.Count; i++)
        {
            Transform Prefabtransform = contentParent.GetChild(i);
            Transform buttonTransform = Prefabtransform.GetChild(3).GetChild(2);
            Transform greenImage = buttonTransform.GetChild(0);
            Transform grayImage = buttonTransform.GetChild(1);
            ItemStackData item = lItem[i];
            int itemCost = shopItemConfigs[i].tradePrice;
            if (PlayerManager.Instance.GetResearchPoint() >= itemCost)
            {
                greenImage.gameObject.SetActive(true);
                grayImage.gameObject.SetActive(false);
                buttonTransform.GetComponent<Button>().interactable = true;
            }
            else
            {
                greenImage.gameObject.SetActive(false);
                grayImage.gameObject.SetActive(true);
                buttonTransform.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void OnCloseClick()
    {

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        this.gameObject.SetActive(false);
    }

    public void OnItemClick(ItemSlotButton itemSlotButton)
    {

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        int index = itemSlotButton.ItemIndex;
        UIManager.Instance.ShowConfirmPointShopPanel(shopItemConfigs[index], lItem[index]);
    }

}