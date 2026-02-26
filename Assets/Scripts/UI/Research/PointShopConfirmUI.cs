using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointShopConfirm : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private TextMeshProUGUI totalPriceText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    [SerializeField] private Image ItemImage;
    [SerializeField] private Button confirmButton;
    private ItemStackData item;
    private ShopItemConfig shopItemConfig;

    int Totalprice;
    int quantity;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void Setup(ShopItemConfig config, ItemStackData data)
    {
        item = data;
        shopItemConfig = config;
        quantity = 1;
        quantityText.text = quantity.ToString();
        Totalprice = shopItemConfig.tradePrice;
        totalPriceText.text = "Tiêu hao: " +Totalprice.ToString();
        itemNameText.text = shopItemConfig.itemName;
        itemDescriptionText.text = shopItemConfig.itemDescription;
        ItemImage.sprite = shopItemConfig.Icon;
        if (shopItemConfig.tradePrice > PlayerManager.Instance.GetResearchPoint())
        {
            totalPriceText.color = Color.red;
            confirmButton.interactable = false;
            confirmButton.transform.GetChild(0).gameObject.SetActive(false);
            confirmButton.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            totalPriceText.color = Color.white;
            confirmButton.interactable = true;
            confirmButton.transform.GetChild(0).gameObject.SetActive(true);
            confirmButton.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void OnInCreaseClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        quantity += 1;
        quantityText.text = (quantity).ToString();
        Totalprice = quantity * shopItemConfig.tradePrice;
        totalPriceText.text = "Tiêu hao: " + (Totalprice).ToString();
        if (Totalprice > PlayerManager.Instance.GetResearchPoint())
        {
            totalPriceText.color = Color.red;
            confirmButton.interactable = false;
            confirmButton.transform.GetChild(0).gameObject.SetActive(false);
            confirmButton.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            totalPriceText.color = Color.white;
            confirmButton.interactable = true;
            confirmButton.transform.GetChild(0).gameObject.SetActive(true);
            confirmButton.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void OnDecreaseClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        if (quantity > 1)
        {
            quantity -= 1;
        }
        quantityText.text = quantity.ToString();
        Totalprice = quantity * shopItemConfig.tradePrice;
        totalPriceText.text = "Tiêu hao: " + (Totalprice).ToString();
        if (Totalprice > PlayerManager.Instance.GetResearchPoint())
        {
            totalPriceText.color = Color.red;
            confirmButton.interactable = false;
            confirmButton.transform.GetChild(0).gameObject.SetActive(false);
            confirmButton.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            totalPriceText.color = Color.white;
            confirmButton.interactable = true;
            confirmButton.transform.GetChild(0).gameObject.SetActive(true);
            confirmButton.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void OnConfirmClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        if (PlayerManager.Instance.GetResearchPoint() >= Totalprice)
        {
            InventoryManager.Instance.AddItem(item.itemID, quantity);
            PlayerManager.Instance.SpendResearchPoint(Totalprice);
            this.gameObject.SetActive(false);
        }
    }

    public void OnCancelClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        this.gameObject.SetActive(false);
    }
}
