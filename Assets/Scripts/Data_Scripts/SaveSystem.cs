using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem 
{
    public static void SavePlayerInventory (PlayerInventory playerInventory)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/PlayerInventory.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        Player_Data data = new Player_Data(playerInventory);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static void SavePlayerPosition(playerInput playerinput)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerinput.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        Player_Data data = new Player_Data(playerinput);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static void SaveScene(Scene_verification scene_Verification)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/scene_Verification.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        Player_Data data = new Player_Data(scene_Verification);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static void SaveEnclosLevel(Shop_Enclos shop_Enclos)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/shop_Enclos.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        Player_Data data = new Player_Data(shop_Enclos);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static Player_Data LoadPlayerInventory()
    {
        string path = Application.persistentDataPath + "/PlayerInventory.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Player_Data data = formatter.Deserialize(stream) as Player_Data;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
    public static Player_Data LoadPlayerInput()
    {
        string path = Application.persistentDataPath + "/playerinput.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Player_Data data = formatter.Deserialize(stream) as Player_Data;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
    public static Player_Data LoadPlayerScene()
    {
        string path = Application.persistentDataPath + "/scene_Verification.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Player_Data data = formatter.Deserialize(stream) as Player_Data;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
    public static Player_Data LoadEnclosLevel()
    {
        string path = Application.persistentDataPath + "/shop_Enclos.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Player_Data data = formatter.Deserialize(stream) as Player_Data;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
