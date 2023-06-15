using Gameplay.Quests;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gameplay.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Transform objectiveContainer;
        [SerializeField] private GameObject objectivePrefab;
        [SerializeField] private GameObject objectiveIncompletePrefab;
        [SerializeField] private GameObject[] rewardUISection;
        [SerializeField] private TextMeshProUGUI rewardText;
        public void Setup(QuestStatus status)
        {
            Quest quest = status.GetQuest();
            title.text = quest.GetTitle();
            
            foreach (Transform item in objectiveContainer)
                Destroy(item.gameObject);
            
            foreach (Quest.Objective objective in quest.GetObjectives())
            {
                GameObject prefab = objectiveIncompletePrefab;
                if (status.IsObjectiveComplete(objective.reference))
                    prefab = objectivePrefab;
                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);
                TextMeshProUGUI objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();
                objectiveText.text = objective.description;
            }
            
            if (rewardUISection != null && quest.GetRewardsCount() > 0)
            {
                foreach (GameObject go in rewardUISection)
                {
                    go.SetActive(true);
                }
                rewardText.text = GetRewardText(quest);
            }
            else
            {
                foreach (GameObject go in rewardUISection)
                {
                    go.SetActive(false);
                }
            }
        }

        private string GetRewardText(Quest quest)
        {
            string rewardText = "";
            foreach (Quest.Reward reward in quest.GetRewards())
            {
                if (rewardText != "")
                    rewardText += ", ";
                if (reward.number > 1)
                    rewardText += reward.number + " ";
                
                if (reward.item == null)
                    Debug.LogWarning("No item for this reward !");
                else
                    rewardText += reward.item.itemName;
            }
            return rewardText;
        }
    }
}
