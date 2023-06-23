using UnityEngine;

[CreateAssetMenu(fileName = "Plant", menuName = "ScriptableObject/Plant")]
[System.Serializable]
public class Plant : ScriptableObject
{
    [Header("Culture")] 
    public string plantName;
    public GameObject source;
    public GameObject seed;
    public GameObject sprout;
    public GameObject plant;
    public GameObject readyPlant;

    [Header("Cooking")]
    public GameObject fruit;
    public GameObject fruitSliced;

    [Header("Times")]
    public float timeOfGrowthSeed = 10f;
    public float timeOfGrowthSprout = 10f;
    public float timeOfGrowthFlower = 10f;
    public float timeOfGrowthFruit = 2f;

    public object[] CurrentPlantGrowthState(PlantGrowth.ProductGrowth growth)
    {
        object[] datas;

        switch (growth)
        {
            case PlantGrowth.ProductGrowth.Seed:
                datas = new object[] { seed, timeOfGrowthSeed };
                break;
            case PlantGrowth.ProductGrowth.Sprout:
                datas = new object[] { seed, timeOfGrowthSprout };
                break;
            case PlantGrowth.ProductGrowth.Flower:
                datas = new object[] { sprout, timeOfGrowthFlower };
                break;
            case PlantGrowth.ProductGrowth.FlowerFruit:
                datas = new object[] { plant, timeOfGrowthFruit };
                break;
            case PlantGrowth.ProductGrowth.End:
                datas = new object[] { readyPlant, 0f };
                break;
            default:
                Debug.LogError($"Error CurrentPlantGrowthState plant growth");
                return null;
        }

        return datas;
    }
}