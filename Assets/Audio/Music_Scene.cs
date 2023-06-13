using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Music_Scene : MonoBehaviour
{
    public AudioClip sceneMenu;
    public AudioClip sceneFarm;
    public AudioClip sceneVillage;
    public AudioSource audioChange;

    float volume;
    public string volumeName;
    public AudioMixer _audio;
    [SerializeField] private string sceneName = "";
    private string sceneNameVerif;
    // Start is called before the first frame update
    private void Start()
    {
        LoadVolumeSystem();
        _audio.SetFloat(volumeName, Mathf.Log(volume) * 20f);
    }

    // Update is called once per frame
    void Update()
    {

        sceneName = SceneManager.GetActiveScene().name;
        if (sceneNameVerif != sceneName)
        {
            sceneNameVerif = sceneName;
            verifSceneAudio();
        }
    }
    void verifSceneAudio()
    {
        sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Menu")
        {
            audioChange.clip = sceneMenu;
            audioChange.Play();

        }
        else if (sceneName == "FarmScene")
        {
            audioChange.clip = sceneFarm;
            audioChange.Play();
        }
        else if (sceneName == "TradingVillage")
        {
            audioChange.clip = sceneVillage;
            audioChange.Play();
        }
        else
        {
            audioChange.clip = sceneVillage;
            audioChange.Play();
        }
    }
    void LoadVolumeSystem()
    {
        //Audio_Data data = SaveSystem.LoadVolume();
        Audio_Data data = (Audio_Data)SaveSystem.Load(SaveSystem.SaveType.Save_Volume);

        if (data == null) return;

        volume = data.volume;
    }

    public void UpdateVolume(float newVolume)
    {
        volume = newVolume;

        _audio.SetFloat(volumeName, Mathf.Log(volume) * 20f);
    }
}
