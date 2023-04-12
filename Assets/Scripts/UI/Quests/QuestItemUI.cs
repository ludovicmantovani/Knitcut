using Gameplay.Quests;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI progress;

    private QuestStatus _status;

    public void Setup(QuestStatus status)
    {
        _status = status;
        title.text = _status.GetQuest().GetTitle();
        progress.text = _status.GetCompletedCount() + "/" + _status.GetQuest().GetObjectiveCount();
    }

    public QuestStatus GetQuestStatus()
    {
        return _status;
    }
}
