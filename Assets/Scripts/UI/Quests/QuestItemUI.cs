using Gameplay.Quests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI progress;

    private Quest _quest;

    public void Setup(Quest quest)
    {
        _quest = quest;
        title.text = quest.GetTitle();
        progress.text = "0/" + quest.GetObjectiveCount();
    }

    public Quest GetQuest()
    {
        return _quest;
    }
}
