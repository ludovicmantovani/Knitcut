using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyBinding_Data 
{
    public string moveForward;
    public string moveBackward;
    public string moveLeft;
    public string moveRight;
    public string interract;
    public string hydratePlante;
    public string healPlante;
    public string inventory;

    public KeyBinding_Data(UI_Menu ui_menu)
    {
        moveForward = ui_menu.moveForward.text;
        moveBackward = ui_menu.moveBackward.text;
        moveLeft = ui_menu.moveLeft.text;
        moveRight = ui_menu.moveRight.text;
        interract = ui_menu.interract.text;
        hydratePlante = ui_menu.hydratePlante.text;
        healPlante = ui_menu.healPlante.text;
        inventory = ui_menu.inventory.text;
    }
}
[System.Serializable]
public class Audio_Data
{
    public float volume;
    public Audio_Data(UI_Menu vol)
    {
        volume = vol.volume.value;
    }
}
