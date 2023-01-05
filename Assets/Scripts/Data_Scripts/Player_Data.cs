using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player_Data 
{
    public int Money ;

    public int Fruit1 ;
    public int Fruit2 ;
    public int Fruit3 ;

    public int Graine1 ;
    public int Graine2 ;
    public int Graine3 ;

    public int Wool1 ;
    public int Wool2 ;
    public int Wool3 ;

    public float[] position;
    public int scene;

    public int LevelEnclo1;
    public int LevelEnclo2;
    public int LevelEnclo3;
    public Player_Data (PlayerInventory playerInventory)
    {
        Money = playerInventory.Money;

        Fruit1 = playerInventory.Fruit1;
        Fruit2 = playerInventory.Fruit2;
        Fruit3 = playerInventory.Fruit3;

        Graine1 = playerInventory.Graine1;
        Graine2 = playerInventory.Graine2;
        Graine3 = playerInventory.Graine3;

        Wool1 = playerInventory.Wool1;
        Wool2 = playerInventory.Wool2;
        Wool3 = playerInventory.Wool3;
    }
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
