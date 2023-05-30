using UnityEngine;

public class AnimalPenRef : MonoBehaviour
{
    [SerializeField] private GameObject surface;
    [SerializeField] private GameObject feeder;
    [SerializeField] private GameObject bell;

    public GameObject Surface
    {
        get { return surface; }
        set { surface = value; }
    }

    public GameObject Feeder
    {
        get { return feeder; }
        set { feeder = value; }
    }

    public GameObject Bell
    {
        get { return bell; }
        set { bell = value; }
    }
}