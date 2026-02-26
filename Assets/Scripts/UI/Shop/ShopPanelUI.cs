using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanelUI : MonoBehaviour
{
    public static ShopPanelUI Instance { get; private set; }

    [Header("Item Panels")]
    public GameObject itemPanel_StudyTools;
    public GameObject itemPanel_MissionItems;
    public GameObject itemPanel_VIPItems;

    [Header("Buttons")]
    public Button buttonShop_StudyTools;
    public Button buttonShop_MissionItems;
    public Button buttonShop_VIPItems;
    public Button buttonClose;

    [Header("Button Images")]
    public Image image_StudyTools;
    public Image image_MissionItems;
    public Image image_VIPItems;

    private List<ShopSlotUI> missionSlotList = new List<ShopSlotUI>();
    private List<ShopSlotUI> vipSlotList = new List<ShopSlotUI>();
    private List<ShopSlotUI> toolSlotList = new List<ShopSlotUI>();

    [Header("Effect Settings")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(1.2f, 1.2f, 1.2f);
    public float scaleUp = 1.1f;
    public float transitionSpeed = 6f;

    private Button currentButton;
    private Coroutine activeAnimateCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject); 
        }

        Instance = this;

        if (itemPanel_MissionItems != null)
            missionSlotList.AddRange(itemPanel_MissionItems.GetComponentsInChildren<ShopSlotUI>(true));

        if (itemPanel_VIPItems != null)
            vipSlotList.AddRange(itemPanel_VIPItems.GetComponentsInChildren<ShopSlotUI>(true));

        if (itemPanel_StudyTools != null)
            toolSlotList.AddRange(itemPanel_StudyTools.GetComponentsInChildren<ShopSlotUI>(true));
    }

    private void OnEnable()
    {
        GameEvents.OnShopRefreshed += RefreshUI;
    }

    private void OnDisable()
    {
        GameEvents.OnShopRefreshed -= RefreshUI;
    }

    private void Start()
    {
        if (buttonShop_StudyTools != null) buttonShop_StudyTools.onClick.AddListener(ShowStudyTools);
        if (buttonShop_MissionItems != null) buttonShop_MissionItems.onClick.AddListener(ShowMissionItems);
        if (buttonShop_VIPItems != null) buttonShop_VIPItems.onClick.AddListener(ShowVIPItems);
        if (buttonClose != null) buttonClose.onClick.AddListener(CloseShop);

        if (buttonShop_MissionItems != null) ShowMissionItems();
    }


    private void ResetButtons()
    {
        if (image_StudyTools != null) image_StudyTools.color = normalColor;
        if (image_MissionItems != null) image_MissionItems.color = normalColor;
        if (image_VIPItems != null) image_VIPItems.color = normalColor;

        if (buttonShop_StudyTools != null) buttonShop_StudyTools.transform.localScale = Vector3.one;
        if (buttonShop_MissionItems != null) buttonShop_MissionItems.transform.localScale = Vector3.one;
        if (buttonShop_VIPItems != null) buttonShop_VIPItems.transform.localScale = Vector3.one;
    }

    
    private void HighlightButton(Button btn, Image img)
    {
        ResetButtons();
        currentButton = btn;

        if (activeAnimateCoroutine != null) StopCoroutine(activeAnimateCoroutine);
        activeAnimateCoroutine = StartCoroutine(AnimateButton(btn.transform, img));
    }

    private IEnumerator AnimateButton(Transform btnTransform, Image img)
    {
        Vector3 targetScale = Vector3.one * scaleUp;
        Vector3 startScale = btnTransform.localScale;
        Color startColor = img.color;
        Color targetColor = selectedColor;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;
            btnTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            img.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
    }

    public void RefreshUI()
    {
        if (itemPanel_MissionItems != null && itemPanel_MissionItems.activeInHierarchy)
        {
            RefreshMissionItemsUI();
        }
        else if (itemPanel_VIPItems != null && itemPanel_VIPItems.activeInHierarchy)
        {
            RefreshVIPItemsUI();
        }
        else if (itemPanel_StudyTools != null && itemPanel_StudyTools.activeInHierarchy)
        {
            RefreshStudyToolsUI();
        }
    }

    private void RefreshMissionItemsUI()
    {
        if (itemPanel_MissionItems == null) return;
        itemPanel_MissionItems.SetActive(true);

        List<ShopItemData> dailyItems = ShopManager.Instance.GetDailyMissionItems();

        ShopSlotUI[] slots = itemPanel_MissionItems.GetComponentsInChildren<ShopSlotUI>(true);

        for (int i = 0; i < slots.Length; i++)
        {
            ShopItemData data = (dailyItems != null && i < dailyItems.Count) ? dailyItems[i] : null;
            ShopItemConfig config = (data != null) ? ShopManager.Instance.GetItemConfig(data.IdConfig) : null;

            if (data != null && config != null)
            {
                slots[i].DisplayItem(data, config);

                slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }

    private void RefreshVIPItemsUI() 
    {
        if (itemPanel_VIPItems == null) return;
        itemPanel_VIPItems.SetActive(true);

        List<ShopItemData> dailyItems = ShopManager.Instance.GetVipItems();

        ShopSlotUI[] slots = itemPanel_VIPItems.GetComponentsInChildren<ShopSlotUI>(true);

        for (int i = 0; i < slots.Length; i++)
        {
            ShopItemData data = (dailyItems != null && i < dailyItems.Count) ? dailyItems[i] : null;
            ShopItemConfig config = (data != null) ? ShopManager.Instance.GetItemConfig(data.IdConfig) : null;

            if (data != null && config != null)
            {
                slots[i].DisplayItem(data, config);

                slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }
    private void RefreshStudyToolsUI()
    {
        if (itemPanel_StudyTools == null) return;
        itemPanel_StudyTools.SetActive(true);

        List<ShopItemData> dailyItems = ShopManager.Instance.GetStudyToolItems();

        ShopSlotUI[] slots = itemPanel_StudyTools.GetComponentsInChildren<ShopSlotUI>(true);

        for (int i = 0; i < slots.Length; i++)
        {
            ShopItemData data = (dailyItems != null && i < dailyItems.Count) ? dailyItems[i] : null;
            ShopItemConfig config = (data != null) ? ShopManager.Instance.GetItemConfig(data.IdConfig) : null;

            if (data != null && config != null)
            {
                slots[i].DisplayItem(data, config);

                slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowStudyTools()
    {
        if (itemPanel_StudyTools != null) itemPanel_StudyTools.SetActive(true);
        if (itemPanel_MissionItems != null) itemPanel_MissionItems.SetActive(false);
        if (itemPanel_VIPItems != null) itemPanel_VIPItems.SetActive(false);
        HighlightButton(buttonShop_StudyTools, image_StudyTools);
        RefreshStudyToolsUI();
    }

    public void ShowMissionItems()
    {
        if (itemPanel_StudyTools != null) itemPanel_StudyTools.SetActive(false);
        if (itemPanel_MissionItems != null) itemPanel_MissionItems.SetActive(true);
        if (itemPanel_VIPItems != null) itemPanel_VIPItems.SetActive(false);
        HighlightButton(buttonShop_MissionItems, image_MissionItems);
        RefreshMissionItemsUI();
    }

    public void ShowVIPItems()
    {
        if (itemPanel_StudyTools != null) itemPanel_StudyTools.SetActive(false);
        if (itemPanel_MissionItems != null) itemPanel_MissionItems.SetActive(false);
        if (itemPanel_VIPItems != null) itemPanel_VIPItems.SetActive(true);
        HighlightButton(buttonShop_VIPItems, image_VIPItems);
        RefreshVIPItemsUI();
    }

    public void CloseShop()
    {
        UIManager.Instance.CloseShopPanel();
    }
}