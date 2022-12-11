using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SceneSwitcher : MonoBehaviour
{
    void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Keypad1))
        {
            SceneManager.LoadScene(1);
        }
        else if (Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Keypad2))
        {
            SceneManager.LoadScene(2);
        }
        else if (Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Keypad3))
        {
            SceneManager.LoadScene(3);
        }
        else if (Input.GetKey(KeyCode.Alpha4) || Input.GetKey(KeyCode.Keypad4))
        {
            SceneManager.LoadScene(4);
        }
        else if (Input.GetKey(KeyCode.Alpha5) || Input.GetKey(KeyCode.Keypad5))
        {
            SceneManager.LoadScene(5);
        }
        else if (Input.GetKey(KeyCode.Alpha6) || Input.GetKey(KeyCode.Keypad6))
        {
            SceneManager.LoadScene(6);
        }
        else if (Input.GetKey(KeyCode.Alpha7) || Input.GetKey(KeyCode.Keypad7))
        {
            SceneManager.LoadScene(7);
        }
    }
}
