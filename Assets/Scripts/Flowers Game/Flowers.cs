using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Flowers : MonoBehaviour
{
    [Serializable]
    public class Flower
    {
        public GameObject flower;
        public bool isGoodFlower;
    }

    [SerializeField] private Flower[] flowers;

    private bool gameFinished;

    private void Start()
    {
        gameFinished = false;

        InitializeFlowers();

        SetGoodFlower();
    }

    private void InitializeFlowers()
    {
        flowers = new Flower[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            flowers[i] = new Flower();
            flowers[i].flower = transform.GetChild(i).gameObject;

            flowers[i].flower.GetComponent<Button>().onClick.AddListener(FlowerClicked);
        }
    }

    private void SetGoodFlower()
    {
        Flower randomFlower = flowers[UnityEngine.Random.Range(0, flowers.Length)];

        randomFlower.isGoodFlower = true;
    }

    public void FlowerClicked()
    {
        if (gameFinished) return;

        GameObject flowerClicked = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < flowers.Length; i++)
        {
            if (flowers[i].flower == flowerClicked)
            {
                gameFinished = true;

                StartCoroutine(ShowResult(flowers[i].isGoodFlower));
            }
        }
    }

    private void IsGoodFlower(bool isGoodFlower)
    {
        if (isGoodFlower)
        {
            Debug.Log($"Click on good flower");
        }
        else
        {
            Debug.Log($"Click on wrong flower");
        }
    }

    private IEnumerator ShowResult(bool isGoodFlower)
    {
        IsGoodFlower(isGoodFlower);

        for (int i = 0; i < flowers.Length; i++)
        {
            Transform flowerTransform = flowers[i].flower.transform;
            flowerTransform.GetComponent<Button>().interactable = false;
        }

        yield return new WaitForSeconds(3f);

        Debug.Log("Restart...");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}