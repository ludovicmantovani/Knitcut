using UnityEngine;

public class AnimalPenRef : MonoBehaviour
{
    [SerializeField] private GameObject surface;
    [SerializeField] private GameObject feeder;
    [SerializeField] private GameObject bell;
    [SerializeField] private GameObject panel;

    #region Getters / Setters

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

    public GameObject Panel
    {
        get { return panel; }
        set { panel = value; }
    }

    #endregion
}