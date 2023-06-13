[System.Serializable]
public class KeyBinding_Data
{
    public string rebinds;

    public KeyBinding_Data(UI_Menu ui_menu)
    {
        rebinds = ui_menu.KeyRebinding.Rebinds;
    }
}

[System.Serializable]
public class Audio_Data
{
    public float volume;

    public float cameraSensibilityX;
    public float cameraSensibilityY;

    public Audio_Data(UI_Menu uiMenu)
    {
        volume = uiMenu.volume.value;

        cameraSensibilityX = uiMenu.cameraSensibilityX.value;
        cameraSensibilityY = uiMenu.cameraSensibilityY.value;
    }

    /*public Audio_Data(UI_Menu uiMenu)
    {
        volume = uiMenu.volume.value;
    }

    public Audio_Data(float sensibilityX, float sensibilityY)
    {
        cameraSensibilityX = sensibilityX;
        cameraSensibilityY = sensibilityY;
    }*/
}