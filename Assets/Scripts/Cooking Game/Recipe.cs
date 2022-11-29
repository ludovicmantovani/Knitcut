using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObject/Cooking/Recipe")]
public class Recipe : ScriptableObject
{
    [Serializable]
    public class ConsumablesRequired
    {
        public Consumable consumable;
        public int quantity;
    }

    public string recipeName;
    public List<ConsumablesRequired> consumablesRequired;
    public GameObject finalProduct;
    public bool canBeCooked;
}