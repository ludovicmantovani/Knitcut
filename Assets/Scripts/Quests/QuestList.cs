using Gameplay.UI.Quests;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Quests
{
    public class QuestList : MonoBehaviour
    {
        [SerializeField] private List<Quest> quests = new List<Quest>();
        private List<QuestStatus> _statuses = new List<QuestStatus>();
        private int _questIndex = 0;

        public event Action onUpdate;

        private void Start()
        {
            foreach (Quest quest in quests)
            {
                _statuses.Add(new QuestStatus(quest));
            }
            if (onUpdate != null)
                onUpdate();
        }

        public QuestStatus GetCurrentQuestStatus()
        {
            if (_questIndex < _statuses.Count && _statuses[_questIndex] != null)
            {
                return _statuses[_questIndex];
            }
            return null;
        }

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            QuestStatus newStatus = new QuestStatus(quest);
            _statuses.Add(newStatus);
            if (onUpdate != null)
                onUpdate();
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            status.CompleteObjective(objective);
            if (status.IsComplete())
            {
                _questIndex++;
            }
            if (onUpdate != null)
                onUpdate();
        }

        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return _statuses;
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            foreach (QuestStatus status in _statuses)
            {
                if (status.GetQuest() == quest) return status;
            }
            return null;
        }
    }
}
