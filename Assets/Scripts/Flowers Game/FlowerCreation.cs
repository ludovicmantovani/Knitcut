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

    [SerializeField] private string pathFlowersResources = "Flower/Petal/color6";
    [SerializeField] private Color baseColor = Color.blue;
    [SerializeField] private Color blinkColor = Color.red;
    
    [SerializeField] private ParticleSystem loveParticleSystem = null;


    private List<Transform> _randomPetals;
    private Animator _animator = null;

    public int TotalPetals { get => _randomPetals.Count; }
    public List<Transform> GetRandomPetals() { return _randomPetals; }

    #region UNITY_METHOD
    private void Start()
    {
        _randomPetals = new List<Transform>();
        _animator = GetComponent<Animator>();
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

    public int MakeFlower(int nbPetalMin = 1)
    {
        int totalPetals = Random.Range(
            Mathf.Max(nbPetalMin, minPetals),
            Mathf.Max(nbPetalMin + 1, maxPetals));

        Object[] sameColorPetalSprites = Resources.LoadAll(pathFlowersResources, typeof(Sprite));

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
                item.gameObject.GetComponent<Image>().color = baseColor;
                item.GetComponent<Button>().interactable = false;
            }
        }
        StartCoroutine(DisplayPetals(seconds, endIndex));
    }

    IEnumerator DisplayPetals(float seconds, int endIndex)
    {
        for (int i = 0; i < endIndex - 1; i++)
        {
            GameObject petal = null;
            petal = _randomPetals[i].gameObject;
            yield return new WaitForSeconds(seconds);
            petal.GetComponent<Animator>().SetTrigger("BalanceTrigger");
            petal.GetComponent<Image>().color = blinkColor;
            yield return new WaitForSeconds(seconds);
            petal.GetComponent<Image>().color = baseColor;
        }
        if (_randomPetals[endIndex - 1])
        {
            GameObject petal = null;
            yield return new WaitForSeconds(seconds);
            petal = _randomPetals[endIndex - 1].gameObject;
            petal.GetComponent<Animator>().SetTrigger("BalanceTrigger");
            petal.GetComponent<Image>().color = blinkColor;
            yield return new WaitForSeconds(seconds);
            petal.GetComponent<Image>().color = baseColor;
        }

        for (int i = 0; i < _randomPetals.Count; i++)
            _randomPetals[i].gameObject.GetComponent<Button>().interactable = true;
    }

    public void FallPetals()
    {
        foreach (Transform item in _randomPetals) { item.gameObject.GetComponent<PetalInput>().Fall(); }
    }

    public void CelebrateLove()
    {
        if (_animator != null)
            _animator.SetTrigger("PopLove");
    }

    public void StartHeart()
    {
        if (loveParticleSystem != null)
            loveParticleSystem.Play();
    }
    #endregion
}