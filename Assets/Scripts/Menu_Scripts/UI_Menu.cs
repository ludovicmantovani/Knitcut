using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
[ExecuteAlways]
public class UI_Menu : MonoBehaviour
{
    public TMP_InputField moveForward;
    public TMP_InputField moveBackward;
    public TMP_InputField moveRight;
    public TMP_InputField moveLeft;
    public TMP_InputField interract;
    public TMP_InputField hydratePlante;
    public TMP_InputField healPlante;
    public TMP_InputField inventory;

    public KeyCode moveF;
    public KeyCode moveB;
    public KeyCode moveL;
    public KeyCode moveR;
    public KeyCode inter;
    public KeyCode hydrateP;
    public KeyCode healP;
    public KeyCode invent;

    private string lastKey;
    private float lastVolume;

    [SerializeField] private GameObject menuInterface;
    [SerializeField] private GameObject menuOptions;
    [SerializeField] private GameObject menuKeybinding;
    [SerializeField] private GameObject menuCredits;
    [SerializeField] private GameObject menuAudio;
    [SerializeField] private Vector3 creditsVector;
    [SerializeField] private GameObject creditsObj;
    [SerializeField] private float speedY;
    private float yStartPos;

    public Slider volume;

    public bool startCredits = false;
    void Start()
    {
        yStartPos = -520f;
        speedY = yStartPos;
        KeysBinding();
        LoadKeybindingsTouch();
        LoadVolumeLevel();

    }

    void Update()
    {
        if(startCredits == true)
        {
            ScrollCredits();
        }

        SavesKeybindingsTouch();
        KeysBinding();
        SaveVolumeLevel();
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
    public void MenuCredits()
    {
        menuOptions.SetActive(false);
        menuCredits.SetActive(true);
        startCredits = true;
    }
    public void MenuCreditsBack()
    {
        menuOptions.SetActive(true);
        menuCredits.SetActive(false);
        startCredits = false;
    }
    public void MenuAudio()
    {
        menuOptions.SetActive(false);
        menuAudio.SetActive(true);
    }
    public void MenuAudioBack()
    {
        menuOptions.SetActive(true);
        menuAudio.SetActive(false);
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
    public void ScrollCredits()
    {
        if(creditsObj.transform.position.y >= 360f)
        {
            creditsVector = new Vector3(creditsObj.transform.position.x, yStartPos, creditsObj.transform.position.z);
            creditsObj.transform.position = creditsVector;
            speedY = yStartPos;
            startCredits = false;
        }
        else
        {
            speedY = speedY + Time.deltaTime*15;
            creditsVector = new Vector3(creditsObj.transform.position.x, speedY, creditsObj.transform.position.z);
            creditsObj.transform.position = creditsVector;
        }
         
    }
    public void KeysBinding()
    {
        moveF = (KeyCode) System.Enum.Parse(typeof(KeyCode), moveForward.text);
        moveB = (KeyCode) System.Enum.Parse(typeof(KeyCode), moveBackward.text);
        moveL = (KeyCode) System.Enum.Parse(typeof(KeyCode), moveLeft.text);
        moveR = (KeyCode) System.Enum.Parse(typeof(KeyCode), moveRight.text);
        inter = (KeyCode) System.Enum.Parse(typeof(KeyCode), interract.text);
        hydrateP = (KeyCode) System.Enum.Parse(typeof(KeyCode), hydratePlante.text);
        healP = (KeyCode) System.Enum.Parse(typeof(KeyCode), healPlante.text);
        invent = (KeyCode) System.Enum.Parse(typeof(KeyCode), inventory.text);
    }

    public void SavesKeybindingsTouch()
    {
        if(lastKey != moveForward.text || 
            lastKey != moveBackward.text || 
            lastKey != moveLeft.text || 
            lastKey != moveRight.text || 
            lastKey != interract.text || 
            lastKey != hydratePlante.text || 
            lastKey != healPlante.text || 
            lastKey != inventory.text)
        {
            SaveSystem.SaveKeys(this);
        }
    }
    public void LoadKeybindingsTouch()
    {
        KeyBinding_Data data = SaveSystem.LoadKeys();
        moveForward.text = data.moveForward;
        moveBackward.text = data.moveBackward;
        moveLeft.text = data.moveLeft;
        moveRight.text = data.moveRight;
        interract.text = data.interract;
        hydratePlante.text = data.hydratePlante;
        healPlante.text = data.healPlante;
        inventory.text = data.inventory;
    }
    public void SaveVolumeLevel()
    {
        if(lastVolume != volume.value)
        {
            lastVolume = volume.value;
            SaveSystem.SaveVolume(this);
        }
    }
    public void LoadVolumeLevel()
    {
        Audio_Data data = SaveSystem.LoadVolume();
        volume.value = data.volume;
    }
}
