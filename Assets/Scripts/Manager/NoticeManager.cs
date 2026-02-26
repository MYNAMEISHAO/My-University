using System;
using UnityEngine;

public class NoticeManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject NoticePanel;
    [SerializeField] private Transform NoticePanelParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {

    }

    private void OnEnable()
    {
        GameEvents.OnItemChanged += HandleNotice;
    }

    private void HandleNotice(string arg1, int arg2)
    {
        if(arg1=="Shop_Item_2")
        {
            NoticeTrainingBook(arg2);
        }
        else if(arg1=="Shop_Item_4")
        {
            NoticeInk(arg2);
        }
    }

    private void NoticeTrainingBook(int amount)
    {
        if(amount > 0)
            Instantiate(NoticePanel, NoticePanelParent).GetComponent<NoticeUI>().InitializeAndShow(amount,"trainingbook");
    }

    private void NoticeInk(int amount)
    {
        if(amount > 0)
            Instantiate(NoticePanel, NoticePanelParent).GetComponent<NoticeUI>().InitializeAndShow(amount,"ink");
    }


    private void OnDisable()
    {
       GameEvents.OnItemChanged -= HandleNotice;
    }
}
