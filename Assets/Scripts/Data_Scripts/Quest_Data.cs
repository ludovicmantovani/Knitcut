using Gameplay.Quests;
using Gameplay.UI.Quests;
using System.Collections.Generic;

[System.Serializable]
public class Quest_Data
{
    public Dictionary<string, int> questStatusDic;

    public Quest_Data(QuestManager questManager)
    {
        questStatusDic = new Dictionary<string, int>();

        foreach (QuestStatus qStatus in questManager.GetStatuses())
        {
            questStatusDic.Add(
                qStatus.GetQuest().GetTitle(),
                qStatus.CompletedObjectives.Count);
        }
    }
}
