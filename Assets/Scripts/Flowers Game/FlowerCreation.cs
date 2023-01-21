using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerCreation : MonoBehaviour
{
    [SerializeField] private GameObject petalPrefab;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private int minPetals = 5;
    [SerializeField] private int maxPetals = 10;
    [SerializeField] private int colorVersion = 5;

    private string _pathFlowersResources = "Flower/Petal/color";
    private List<Transform> _randomPetals;

    public int TotalPetals { get => _randomPetals.Count; }
    public List<Transform> GetRandomPetals() { return _randomPetals; }

    #region UNITY_METHOD
    private void Start()
    {
        _randomPetals = new List<Transform>();
    }
    #endregion

    #region SPECIFIC_METHOD
    private void CreatePetal(int totalPetals, int index, Sprite sprite)
    {
        Transform petal = Instantiate(petalPrefab, transform).transform;
        _randomPetals.Add(petal.transform);

        petal.SetAsFirstSibling();

        petal.GetComponent<Image>().sprite = sprite;

        PlacePetal(petal, totalPetals, index);
    }

    private void PlacePetal(Transform petale, int totalPetales, int index)
    {
        int zRotation = index * (360 / totalPetales);

        Vector3 localEulerAngle = new Vector3(0, 0, zRotation);

        petale.localEulerAngles = localEulerAngle;
    }

    public int MakeFlower()
    {
        int totalPetals = Random.Range(minPetals, maxPetals);

        string colorPath = _pathFlowersResources + Random.Range(1, colorVersion + 1);

        Object[] sameColorPetalSprites = Resources.LoadAll(colorPath, typeof(Sprite));

        for (int i = 0; i < totalPetals; i++)
            CreatePetal(
                totalPetals, i,
                (Sprite)sameColorPetalSprites[Random.Range(0, sameColorPetalSprites.Length)]
                );

        _randomPetals.Shuffle();
        for (int i = 0; i < totalPetals; i++) { _randomPetals[i].name = i.ToString(); }
        Instantiate(buttonPrefab, transform);

        return TotalPetals;
    }

    public void ShowSequence(float seconds = 1f, int endIndex = 0, bool reset = true)
    {
        if (_randomPetals == null) return;

        endIndex = endIndex <= 0 || endIndex > TotalPetals ? TotalPetals : endIndex;
        seconds = seconds <= 0 || seconds > 5f ? 5f : seconds;

        if (reset)
        {
            foreach (Transform item in _randomPetals)
            {
                item.GetComponent<Button>().interactable = false;
                item.gameObject.SetActive(false);
            }
        }

        StartCoroutine(DisplayPetals(seconds, endIndex));
    }

    IEnumerator DisplayPetals(float seconds, int endIndex)
    {
        for (int i = 0; i < endIndex; i++)
        {
            _randomPetals[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(seconds);
        }

        for (int i = 0; i < endIndex; i++)
            _randomPetals[i].GetComponent<Button>().interactable = true;
    }
    #endregion
}