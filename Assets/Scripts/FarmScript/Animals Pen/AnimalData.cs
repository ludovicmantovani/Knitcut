using UnityEngine;

public class AnimalData : MonoBehaviour
{
    [SerializeField] private AnimalType animalType;

    public AnimalType AnimalType
    {
        get { return animalType; }
        set { animalType = value; }
    }
}