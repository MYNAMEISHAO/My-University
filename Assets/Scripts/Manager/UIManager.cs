using System;
using UnityEngine;
using static FlowManager;

[DefaultExecutionOrder(-100)]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject homePanel;
    // Module Classroom
    public GameObject roomPanel;

    //Module teacher
    public GameObject characterListPanel;
    public CharacterDetailPanel characterDetailPanel;

    // Module Shop
    public GameObject shopPanel;

    //Module Missson
    public GameObject missionPanel;

    // Module Gift
    public GameObject giftPanel;

    // Module Môn học
    public SubjectDetailPanel detailPanel;   // Panel hien chi tiet subject
    public GameObject subjectsListPanel;     // Panel danh sách môn (gán GameObject chứa SimpleSubjectsPanel)

    //Module Research Point Shop
    public GameObject researchUI;
    public GameObject newSubjectUI;
    public GameObject PointShopUI;
    public GameObject ConfirmPointShopUI;

    // Module Offline Reward
    public OfflineRewardUI offlineRewardUI;

    //Module Spawner
    public GameObject spawnerUI;

    //Modue Setting
    public GameObject settingUI;

    public void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnOfflineGoldCalculated += ShowOfflineReward;
    }

    private void OnDisable()
    {
        GameEvents.OnOfflineGoldCalculated -= ShowOfflineReward;
    }

    public void ShowHomePanel()
    {
        if (homePanel != null)
        {
            homePanel.SetActive(true);
        }
        homePanel.GetComponent<MainMenuControl>().OpenHeader();
        homePanel.GetComponent<MainMenuControl>().OpenIconButton();
    }

    // --- Classroom: quản lý chuyển panel ---

    public void ShowRoomPanel(RoomConfig roomConfig, RoomData data)
    {
        // Logic to display room information in the UI
        Debug.Log($"Showing panel for room: {roomConfig.RoomName}");
        roomPanel.SetActive(true);
        roomPanel.GetComponent<UpgradeClassroomUI>().DisplayRoom(roomConfig, data);
        homePanel.GetComponent<MainMenuControl>().CloseIconButton();
    }

    public void CloseRoomPanel()
    {
        roomPanel.SetActive(false);
        homePanel.GetComponent<MainMenuControl>().OpenIconButton();
    }

    // --- Teacher: quản lý chuyển panel ---
    public void ShowTeacherListPanel()
    {
        if (characterListPanel != null)
        {
            characterListPanel.SetActive(true);
            homePanel.GetComponent<MainMenuControl>().CloseIconButton();
        }
    }

    public void ShowDetailTeacherPanel(TeacherConfig config)
    {
        characterDetailPanel.gameObject.SetActive(true);
        characterDetailPanel.ShowDetail(config);
    }

    public void CloseTeacherUI()
    {
        characterListPanel.SetActive(false);
        characterDetailPanel.gameObject.SetActive(false);
        homePanel.GetComponent<MainMenuControl>().OpenIconButton();

    }

    // --- Shop: quản lý chuyển panel ---
    public void ShowShopPanel()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            homePanel.GetComponent<MainMenuControl>().CloseIconButton();
        }

    }
    public void CloseShopPanel()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            homePanel.GetComponent<MainMenuControl>().OpenIconButton();
        }
    }

    // --- Môn học: quản lý chuyển panel ---
    public void ShowSubjectDetailPanel(SubjectConfig config, SubjectData data)
    {
        if (detailPanel != null)
        {
            detailPanel.gameObject.SetActive(true);
            detailPanel.ShowDetail(config, data);
        }
        else
        {
            Debug.LogWarning("UIManager: detailPanel chưa được gán!", this);
        }
    }
    public void ShowSubjectsListPanel()
    {
        if (subjectsListPanel != null)
        {
            subjectsListPanel.SetActive(true);
            homePanel.GetComponent<MainMenuControl>().CloseIconButton();
        }
        if (detailPanel != null)
        {
            detailPanel.gameObject.SetActive(false);
        }
    }
    public void CloseSubjectDetailPanel()
    {
        if (detailPanel != null) detailPanel.gameObject.SetActive(false);
        if (subjectsListPanel != null) subjectsListPanel.SetActive(true);
    }

    public void CloseSubjectsListPanel()
    {
        if (subjectsListPanel != null)
        {
            subjectsListPanel.SetActive(false);
            homePanel.GetComponent<MainMenuControl>().OpenIconButton();
        }
    }

    //Module Research và Point Shop
    public void ShowPointShopPanel()
    {
        if (PointShopUI != null)
        {
            PointShopUI.SetActive(true);
        }
    }

    public void ShowConfirmPointShopPanel(ShopItemConfig config, ItemStackData data)
    {
        if (ConfirmPointShopUI != null)
        {
            ConfirmPointShopUI.SetActive(true);
            ConfirmPointShopUI.GetComponent<PointShopConfirm>().Setup(config, data);
        }
    }

    public void ShowResearchUI()
    {
        if( researchUI != null)
        {
            researchUI.SetActive(true);
            homePanel.GetComponent<MainMenuControl>().CloseIconButton();
            homePanel.GetComponent<MainMenuControl>().CloseHeader();
        }
    }

    public void CloseResearchUI()
    {
        if (researchUI != null)
        {
            researchUI.SetActive(false);
            homePanel.GetComponent<MainMenuControl>().OpenIconButton();
            homePanel.GetComponent<MainMenuControl>().OpenHeader();
        }
    }

    public void ShowNewSubjectPanel()
    {
        if (newSubjectUI != null)
        {
            newSubjectUI.SetActive(true);
        }
    }

    // Module Offline Reward
    public void ShowOfflineReward(int coin)
    {
        if (offlineRewardUI != null)
        {
            offlineRewardUI.Show(coin);
            homePanel.GetComponent<MainMenuControl>().CloseIconButton();
        }
    }

    public void CloseOfflineReward()
    {
        if (offlineRewardUI != null)
        {
            offlineRewardUI.gameObject.SetActive(false);
            homePanel.GetComponent<MainMenuControl>().OpenIconButton();
        }
    }

    //Module Mission
    public void ShowMissionPanel()
    {
        if (missionPanel != null)
        {
            missionPanel.SetActive(true);
            homePanel.GetComponent<MainMenuControl>().CloseIconButton();
        }
    }

    public void CloseMissionPanel()
    {
        if (missionPanel != null)
        {
            missionPanel.SetActive(false);
            homePanel.GetComponent<MainMenuControl>().OpenIconButton();
        }
    }
    //Module Gift
    public void ShowGiftPanel()
    {
        if (giftPanel != null)
        {
            giftPanel.SetActive(true);
            homePanel.GetComponent<MainMenuControl>().CloseIconButton();
        }
    }
    public void CloseGiftPanel()
    {
        if (giftPanel != null)
        {
            giftPanel.SetActive(false);
            homePanel.GetComponent<MainMenuControl>().OpenIconButton();
        }
    }

    //Module Spawner
    public void ShowSpawnerPanel()
    {
        if (spawnerUI != null)
        {
            spawnerUI.SetActive(true);
            homePanel.GetComponent<MainMenuControl>().CloseIconButton();
        }
    }
    public void CloseSpawnerPanel()
    {
        if (spawnerUI != null)
        {
            spawnerUI.SetActive(false);
            homePanel.GetComponent<MainMenuControl>().OpenIconButton();
        }
    }
    //Module Setting
    public void ShowSettingPanel()
    {
        if (settingUI != null)
        {
            settingUI.SetActive(true);
            homePanel.GetComponent<MainMenuControl>().CloseIconButton();
        }
    }
    public void CloseSettingPanel()
    {
        if (settingUI != null)
        {
            settingUI.SetActive(false);
            homePanel.GetComponent<MainMenuControl>().OpenIconButton();
        }
    }
}