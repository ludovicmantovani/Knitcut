using UnityEngine;

public class AnimalPenRef : MonoBehaviour
{
    [SerializeField] private GameObject surface;

    public GameObject Surface
    {
        get { return surface; }
        set { surface = value; }
    }
}