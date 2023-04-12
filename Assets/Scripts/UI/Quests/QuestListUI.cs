using Gameplay.Quests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestListUI : MonoBehaviour
{
    [SerializeField] private QuestItemUI questPrefab;

    private QuestList _questList;

    void Start()
    {
        _questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
        _questList.onUpdate += Redraw;
        Redraw();
    }

    private void Redraw()
    {
        transform.DetachChildren();
        foreach (QuestStatus status in _questList.GetStatuses())
        {
            QuestItemUI uiInstance = Instantiate<QuestItemUI>(questPrefab, transform);
            uiInstance.Setup(status);
        }
    }
}
