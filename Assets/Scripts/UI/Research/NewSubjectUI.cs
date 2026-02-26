using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewSubjectUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]private Button exitButton;
    [SerializeField]private Image subjectImage;
    [SerializeField]private TextMeshProUGUI subjectNameText;
    [SerializeField]private TextMeshProUGUI subjectDescriptionText;
    [SerializeField]private Transform newText;
    SubjectConfig subjectConfig;
    [SerializeField] private ParticleSystem particle;

    private void OnEnable()
    {
        GameEvents.OnSubjectUnlocked += HandleSubjectUnlocked;
        // Lấy một SubjectConfig ngẫu nhiên mỗi khi bảng được bật
        subjectConfig = SubjectManager.Instance.GetRandomSubjectConfig();
        CheckSubjectNew(subjectConfig.ID);
        ShowSubjectInfo();
    }

    private void OnDisable()
    {
        GameEvents.OnSubjectUnlocked -= HandleSubjectUnlocked;
    }

    private void Start()
    {
        ShowEffect();
    }

    private void HandleSubjectUnlocked(string obj)
    {
        newText.gameObject.SetActive(true);
    }
    public void OnExitClick()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        gameObject.SetActive(false);
    }

    public void ShowEffect()
    {
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        ParticleSystem ps = Instantiate(particle, subjectImage.transform.position, rotation ,subjectImage.transform);
        ps.Play();
    }
    void ShowSubjectInfo()
    {
        subjectImage.sprite = subjectConfig.Image;
        subjectNameText.text = subjectConfig.Name;
        subjectDescriptionText.text = subjectConfig.Description;
    }
    void CheckSubjectNew(string id)
    {
        SubjectData data = SubjectManager.Instance.GetSubject(id);
        if (data.Status == false)
        {
            newText.gameObject.SetActive(true);
            SubjectManager.Instance.UnlockSubject(id);

        }
        else
        {
            newText.gameObject.SetActive(false);
            PlayerManager.Instance.AddResearchPoint(50);
        }
    }
}
