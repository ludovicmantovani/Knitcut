using Gameplay.UI.Quests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTriggerZone : MonoBehaviour
{
    [SerializeField] private string objectif_ref = "";
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (objectif_ref.Length > 0)
            {
                bool completeObjective = QuestManager.Instance.CompleteObjective(objectif_ref);
                if (completeObjective) Destroy(gameObject);
            }
        }
    }
}
