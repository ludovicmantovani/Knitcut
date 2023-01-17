using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UI_Menu : MonoBehaviour
{
    [SerializeField] private TMP_InputField moveForward;
    [SerializeField] private TMP_InputField moveBackward;
    [SerializeField] private TMP_InputField moveRight;
    [SerializeField] private TMP_InputField moveLeft;
    [SerializeField] private TMP_InputField interract;
    [SerializeField] private TMP_InputField hydratePlante;
    [SerializeField] private TMP_InputField healPlante;
    [SerializeField] private TMP_InputField inventory;

    [SerializeField] private GameObject menuInterface;
    [SerializeField] private GameObject menuOptions;
    [SerializeField] private GameObject menuKeybinding;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void UpperCaseVerif()
    {
        moveForward.text = moveForward.text.ToUpper();
        moveBackward.text = moveBackward.text.ToUpper();
        moveRight.text = moveRight.text.ToUpper();
        moveLeft.text = moveLeft.text.ToUpper();
        interract.text = interract.text.ToUpper();
        hydratePlante.text = hydratePlante.text.ToUpper();
        healPlante.text = healPlante.text.ToUpper();
        inventory.text = inventory.text.ToUpper();
    }
    public void OptionsMenu()
    {
        menuInterface.SetActive(false);
        menuKeybinding.SetActive(false);
        menuOptions.SetActive(true);
    }
    public void KeyBinding()
    {
        menuInterface.SetActive(false);
        menuKeybinding.SetActive(true);
        menuOptions.SetActive(false);
    }
    public void MenuSettings()
    {
        menuInterface.SetActive(true);
        menuKeybinding.SetActive(false);
        menuOptions.SetActive(false);
    }
    public void NewGame()
    {
        SceneManager.LoadScene(2);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
