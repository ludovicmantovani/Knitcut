using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] private Quest quest;
        [SerializeField] private bool giveAtStart = false;

        private void Start()
        {
            if (quest != null && giveAtStart)
            {
                GiveQuest();
            }
        }

        public void GiveQuest()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.AddQuest(quest);
        }
    }
}
