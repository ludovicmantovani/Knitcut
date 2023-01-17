using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private List<int> captureGameIndex;

    void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Keypad1)) //Menu
            SceneManager.LoadScene(1);
        else if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Keypad2)) //Ferme
            SceneManager.LoadScene(2);
        else if (Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Keypad3)) //Village
            SceneManager.LoadScene(3);
        else if (Input.GetKeyUp(KeyCode.Alpha4) || Input.GetKeyUp(KeyCode.Keypad4)) //Capture
        {
            if (captureGameIndex.Count > 0)
                SceneManager.LoadScene(
                    captureGameIndex[UnityEngine.Random.Range(0, captureGameIndex.Count)]
                    );
            else
                Debug.LogWarning("Aucun index de scene configure pour le mini jeu de capture");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha5) || Input.GetKeyUp(KeyCode.Keypad5)) //Couture
            SceneManager.LoadScene(5);
        else if (Input.GetKeyUp(KeyCode.Alpha6) || Input.GetKeyUp(KeyCode.Keypad6)) //Cuisine
            SceneManager.LoadScene(6);
        else if (Input.GetKeyUp(KeyCode.Alpha7) || Input.GetKeyUp(KeyCode.Keypad7)) //Memoire
            SceneManager.LoadScene(7);
        else if (Input.GetKeyUp(KeyCode.Alpha8) || Input.GetKeyUp(KeyCode.Keypad8)) //Blocking
            SceneManager.LoadScene(8);
    }
}
