using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObject/Gameplay/Quest", order =0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private string[] objectives;

        public string GetTitle()
        {
            return name;
        }

        public int GetObjectiveCount()
        {
            return objectives.Length;
        }

        public IEnumerable<string> GetObjectives()
        {
            return objectives;
        }
    }
}
