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

        public List<string> CompletedObjectives { get => _completedObjectives;}

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

        public bool CompleteObjective(string objective)
        {
            if (_quest.HasObjective(objective) && !_completedObjectives.Contains(objective))
            {
                _completedObjectives.Add(objective);
                return true;
            }
            return false;
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

        public void SetCompleted(int value)
        {
            List<Quest.Objective> qo = (List<Quest.Objective>)_quest.GetObjectives();
            for (int index = 0; index < value; index++)
            {
                CompleteObjective(qo[index].reference);
            }
        }
    }
}
