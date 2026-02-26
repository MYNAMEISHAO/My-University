using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterListUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject characterItemPrefab;
    public Transform contentParent;   // Viewport -> Content
    public Button closeButton;        // Nút đóng (nếu bạn có)

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);

    }
    private void OnEnable()
    {
        StartCoroutine(WaitAndPopulate());
    }

    private IEnumerator WaitAndPopulate()
    {
        yield return new WaitForEndOfFrame();
        PopulateCharacterList();
    }

    private void PopulateCharacterList()
    {
        if (TeacherManager.Instance == null) return;
        List<TeacherConfig> allConfigs = TeacherManager.Instance.GetAllTeacherConfigs();

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var teacherData in allConfigs)
        {
            GameObject item = Instantiate(characterItemPrefab, contentParent);

            var itemController = item.GetComponent<CharacterItemUI>();
            if (itemController != null)
            {
                itemController.Setup(teacherData, this);
            }
        }
    }
    public void OnTeacherSelected(TeacherConfig config)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowDetailTeacherPanel(config);
        }
    }
    private void ClosePanel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        UIManager.Instance.CloseTeacherUI();
    }
}