using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Globalization;

[ExecuteAlways]
public class UI_Menu : MonoBehaviour
{
    [Header("Launcher Mode")]
    [SerializeField] private LauncherMode launcherMode;

    [Header("References Objects")]
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
    public InputField volumeValue;
    public VolumeCTRL audioVolume;

    private Music_Scene musicScene;

    public Slider cameraSensibilityX;
    public InputField cameraSensibilityXValue;
    public Slider cameraSensibilityY;
    public InputField cameraSensibilityYValue;

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
        musicScene = FindObjectOfType<Music_Scene>();

        yStartPos = 0f;
        speedY = yStartPos;

        LoadVolumeAndCameraSensibility();
        SaveVolumeAndCameraSensibility();
    }

    void Update()
    {
        if(startCredits == true)
        {
            ScrollCredits();
        }
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

        SaveVolumeAndCameraSensibility();
    }

    public void MenuSettings()
    {
        menuInterface.SetActive(true);
        menuKeybinding.SetActive(false);
        menuOptions.SetActive(false);
    }

    public void NewGame()
    {
        launcherMode.launchMode = LauncherMode.LaunchMode.New;

        launcherMode.Launch();
    }

    public void ContinueGame()
    {
        launcherMode.launchMode = LauncherMode.LaunchMode.Continue;

        launcherMode.Launch();
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

    public void ResetAllBindings()
    {        
        for (int i = 0; i < MenuKeybinding.transform.childCount; i++)
        {
            if (MenuKeybinding.transform.GetChild(i).GetComponent<KeyBindingRefs>())
            {
                KeyBindingRefs keyBindingRefs = MenuKeybinding.transform.GetChild(i).GetComponent<KeyBindingRefs>();

                InputAction action = rebinding.PlayerInput.FindAction(keyBindingRefs.InputActionName);

                rebinding.ResetSingleBinding(keyBindingRefs, action);
            }
        }
    }

    #region OnChanged

    public void OnVolumeSliderChanged()
    {
        volumeValue.text = volume.value.ToString();

        musicScene.UpdateVolume(volume.value);
    }

    public void OnVolumeInputChanged()
    {
        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";
        if (float.TryParse(volumeValue.text, NumberStyles.Any, ci, out float value)) volume.value = value;

        musicScene.UpdateVolume(volume.value);
    }

    public void OnSensibilityXSliderChanged()
    {
        cameraSensibilityXValue.text = cameraSensibilityX.value.ToString();
    }

    public void OnSensibilityXInputChanged()
    {
        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";
        if (float.TryParse(cameraSensibilityXValue.text, NumberStyles.Any, ci, out float value)) cameraSensibilityX.value = value;

        cameraSensibilityXValue.text = cameraSensibilityX.value.ToString();
    }

    public void OnSensibilityYSliderChanged()
    {
        cameraSensibilityYValue.text = cameraSensibilityY.value.ToString();
    }

    public void OnSensibilityYInputChanged()
    {
        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";
        if (float.TryParse(cameraSensibilityYValue.text, NumberStyles.Any, ci, out float value)) cameraSensibilityY.value = value;
        //cameraSensibilityY.value = float.Parse(cameraSensibilityYValue.text, NumberStyles.Any, ci);

        cameraSensibilityYValue.text = cameraSensibilityY.value.ToString();
    }

    #endregion

    public void SaveVolumeAndCameraSensibility()
    {
        SaveSystem.Save(SaveSystem.SaveType.Save_Volume, this);
    }

    public void LoadVolumeAndCameraSensibility()
    {
        Audio_Data data = (Audio_Data)SaveSystem.Load(SaveSystem.SaveType.Save_Volume);

        if (data == null) return;

        if (data.volume != 0)
        {
            volume.value = data.volume;
            volumeValue.text = volume.value.ToString();
        }

        if (data.cameraSensibilityX != 0)
        {
            cameraSensibilityX.value = data.cameraSensibilityX;
            cameraSensibilityXValue.text = cameraSensibilityX.value.ToString();
        }

        if (data.cameraSensibilityY != 0)
        {
            cameraSensibilityY.value = data.cameraSensibilityY;
            cameraSensibilityYValue.text = cameraSensibilityY.value.ToString();
        }
    }

    private void OnApplicationQuit()
    {
        SaveVolumeAndCameraSensibility();
    }
}