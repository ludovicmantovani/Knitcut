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
            SetData();
        }

        public int GetQuestCount()
        {
            return quests.Count;
        }

        public void SetData()
        {
            Quest_Data data = (Quest_Data)SaveSystem.Load(SaveSystem.SaveType.Save_Quest, this);
            _statuses.Clear();
            _questIndex = 0;
            if (data == null)
            {
                foreach (Quest quest in quests)
                {
                    _statuses.Add(new QuestStatus(quest));
                }
            }
            else
            {
                foreach (KeyValuePair<string, int> item in data.questStatusDic)
                {
                    Quest q = FindQuestByName(item.Key);
                    if (q != null)
                    {
                        QuestStatus qs = new QuestStatus(q);
                        qs.SetCompleted(item.Value);
                        _statuses.Add(qs);
                        if (qs.IsComplete())
                            _questIndex++;
                    }
                }
            }
            if (onUpdate != null)
                onUpdate();
        }

        private Quest FindQuestByName(string key)
        {
            foreach (Quest quest in quests)
            {
                if (quest.GetTitle() == key)
                {
                    return quest;
                }
            }
            return null;
        }

        public QuestStatus GetCurrentQuestStatus()
        {
            if (_questIndex < _statuses.Count && _statuses[_questIndex] != null)
            {
                return _statuses[_questIndex];
            }
            return null;
        }

        public bool CompleteObjective(string objective)
        {
            if (quests.Count > _questIndex)
            {
                return CompleteObjective(quests[_questIndex], objective);
            }
            return false;
        }

        public bool CompleteObjective(Quest quest, string objective)
        {
            bool ret = false;
            QuestStatus status = GetQuestStatus(quest);
            ret = status.CompleteObjective(objective);
            if (ret)
            {
                if (status.IsComplete())
                {
                    if (onUpdate != null)
                        onUpdate();
                    _questIndex++;
                }
                if (onUpdate != null)
                    onUpdate();

                SaveSystem.Save(SaveSystem.SaveType.Save_Quest, this);
            }
            return ret;
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