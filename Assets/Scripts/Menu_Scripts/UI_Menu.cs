using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    [SerializeField] private GameObject waitingBinding;
    [SerializeField] private float speedY;
    private float yStartPos;

    public Slider volume;
    public VolumeCTRL audioVolume;

    private Rebinding rebinding;

    public Rebinding KeyRebinding
    {
        get { return rebinding; }
        set { rebinding = value; }
    }

    public GameObject WaitingBinding
    {
        get { return waitingBinding; }
        set { waitingBinding = value; }
    }

    public GameObject MenuKeybinding
    {
        get { return menuKeybinding; }
        set { menuKeybinding = value; }
    }

    public bool startCredits = false;

    void Start()
    {
        rebinding = GetComponent<Rebinding>();

        yStartPos = 0f;
        speedY = yStartPos;

        LoadVolumeLevel();
    }

    void Update()
    {
        if(startCredits == true)
        {
            ScrollCredits();
        }

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
        if (menuKeybinding.activeSelf) rebinding.Save();

        menuInterface.SetActive(false);
        menuKeybinding.SetActive(false);
        menuOptions.SetActive(true);
    }
    public void KeyBinding()
    {
        LoadBindings();

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
        SaveVolumeSystem();
    }
    public void MenuSettings()
    {
        menuInterface.SetActive(true);
        menuKeybinding.SetActive(false);
        menuOptions.SetActive(false);
    }
    public void NewGame()
    {
        SceneManager.LoadScene("FarmScene");
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

    private void LoadBindings()
    {
        rebinding.Load();

        for (int i = 0; i < MenuKeybinding.transform.childCount; i++)
        {
            if (MenuKeybinding.transform.GetChild(i).GetComponent<KeyBindingRefs>())
            {
                KeyBindingRefs keyBindingRefs = MenuKeybinding.transform.GetChild(i).GetComponent<KeyBindingRefs>();

                InputAction action = rebinding.PlayerInput.FindAction(keyBindingRefs.InputActionName);

                rebinding.RebindComplete(keyBindingRefs, action);
            }
        }
    }

    public void SaveVolumeLevel()
    {
        if(lastVolume != volume.value)
        {
            lastVolume = volume.value;
            //SaveSystem.SaveVolume(this);
            SaveSystem.Save(SaveSystem.SaveType.Save_Volume, this);
        }
    }

    public void LoadVolumeLevel()
    {
        //Audio_Data data = SaveSystem.LoadVolume();
        Audio_Data data = (Audio_Data)SaveSystem.Load(SaveSystem.SaveType.Save_Volume, this);

        if (data == null) return;

        volume.value = data.volume;
    }

    void SaveVolumeSystem()
    {
        //SaveSystem.SaveVolume(this);
        SaveSystem.Save(SaveSystem.SaveType.Save_Volume, this);
    }
}