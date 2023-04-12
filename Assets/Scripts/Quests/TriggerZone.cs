using Gameplay.Quests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    [SerializeField] private QuestCompletion questCompletion;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (questCompletion)
            {
                questCompletion.CompleteObjective();
            }
            Destroy(gameObject);
        }
    }
}
