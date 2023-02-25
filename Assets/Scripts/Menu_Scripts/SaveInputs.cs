using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveInputs : MonoBehaviour
{
    public InputActionAsset actions;

    public void OnEnable()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
        {
            actions.LoadFromJson(rebinds);
        }
    }
    public void OnDisable()
    {
        var rebinds = actions.ToJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
}




