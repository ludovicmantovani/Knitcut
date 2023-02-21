using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player_Data 
{
    public int Money ;


    public float[] position;
    public int scene;

    public int LevelEnclo1;
    public int LevelEnclo2;
    public int LevelEnclo3;
   
    public Player_Data (playerInput playerInput)
    {
        position = new float[3];
        position[0] = playerInput.transform.position.x;
        position[1] = playerInput.transform.position.y;
        position[2] = playerInput.transform.position.z;
    }
    
    public Player_Data(Scene_verification scene_Verification)
    {
        scene = scene_Verification.sceneIndex;
    }
    public Player_Data(Shop_Enclos shop_Enclos)
    {
        LevelEnclo1 = shop_Enclos.levelEnclo1;
        LevelEnclo2 = shop_Enclos.levelEnclo2;
        LevelEnclo3 = shop_Enclos.levelEnclo3;
    }
}
