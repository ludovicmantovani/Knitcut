using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeCTRL : MonoBehaviour
{
    public AudioMixer _audio;
    public string volumeName;
    Slider slider
    {
        get { return GetComponent<Slider>(); }
    }
    public void UpdateValueOnChange(float value)
    {
        _audio.SetFloat(volumeName, Mathf.Log(value) * 20f);
    }
}
