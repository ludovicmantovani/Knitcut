using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObject/Gameplay/Quest", order =0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private string titre = "";
        [SerializeField] private List<Objective> objectives = new List<Objective>();
        [SerializeField] private List<Reward> rewards = new List<Reward>();
        [SerializeField] private bool canGiveRewards = true;
        [SerializeField] private bool canSeeRewards = false;

        public bool CanGiveRewards { get => canGiveRewards;}
        public bool CanSeeRewards { get => canSeeRewards;}

        [System.Serializable]
        public class Reward
        {
            [Min(1)]
            public int number;
            public Item item;
            public bool inInventory = true;
        }

        [System.Serializable]
        public class Objective
        {
            public string reference;
            public string description;
        }


        public string GetTitle()
        {
            return titre.Length > 0 ? titre : name;
        }

        public int GetObjectiveCount()
        {
            return objectives.Count;
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }

        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }

        public int GetRewardsCount()
        {
            return rewards.Count;
        }

        public bool HasObjective(string objectiveRef)
        {
            foreach (Quest.Objective objective in objectives)
            {
                if (objective.reference == objectiveRef)
                    return true;
            }
            return false;
        }
    }
}
