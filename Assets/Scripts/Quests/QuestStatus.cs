using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Quests
{
    public class QuestStatus
    {
        private Quest _quest;
        private List<string> _completedObjectives = new List<string>();

        public QuestStatus(Quest quest)
        {
            this._quest = quest;
        }

        public Quest GetQuest()
        {
            return _quest;
        }

        public int GetCompletedCount()
        {
            return _completedObjectives.Count;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return _completedObjectives.Contains(objective);
        }

        public void CompleteObjective(string objective)
        {
            if (_quest.HasObjective(objective))
                _completedObjectives.Add(objective);
        }

        public bool IsComplete()
        {
            foreach (Quest.Objective objective in _quest.GetObjectives())
            {
                if (!_completedObjectives.Contains(objective.reference))
                    return false;
            }
            return true;
        }
    }
}
