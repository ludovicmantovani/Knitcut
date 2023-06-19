using Gameplay.Quests;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Gameplay.UI.Quests
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private List<Quest> quests = new List<Quest>();
        private List<QuestStatus> _statuses = new List<QuestStatus>();
        private int _questIndex = 0;

        private static QuestManager _instance = null;
        public static QuestManager Instance => _instance;

        public event Action onUpdate;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                _instance = this;
            }
            DontDestroyOnLoad(this.gameObject);
        }

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

        public void CompleteObjective(string objective)
        {
            if (quests.Count > _questIndex)
            {
                CompleteObjective(quests[_questIndex], objective);
            }
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            status.CompleteObjective(objective);
            if (status.IsComplete())
            {
                if (onUpdate != null)
                    onUpdate();
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