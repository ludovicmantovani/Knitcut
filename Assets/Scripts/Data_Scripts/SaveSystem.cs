using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem 
{
    //player save
    /*public static void SavePlayerInventory (PlayerInventory playerInventory)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/PlayerInventory.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        Player_Data data = new Player_Data(playerInventory);

        formatter.Serialize(stream, data);
        stream.Close();
    }*/

    public static void SavePlayerMoney(playerController playerController)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playercontroller.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        Player_Data data = new Player_Data(playerController);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SavePlayerPosition(playerInput playerInput)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerinput.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        Player_Data data = new Player_Data(playerInput);

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
    /*public static Player_Data LoadPlayerInventory()
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
    }*/
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
    public static Player_Data LoadPlayerController()
    {
        string path = Application.persistentDataPath + "/playercontroller.save";
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
    // menu save
    public static void SaveKeys(UI_Menu ui_menu)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/UI_Menu.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        KeyBinding_Data data = new KeyBinding_Data(ui_menu);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static KeyBinding_Data LoadKeys()
    {
        string path = Application.persistentDataPath + "/UI_Menu.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            KeyBinding_Data data = formatter.Deserialize(stream) as KeyBinding_Data;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    // audio save
    public static void SaveVolume(UI_Menu vol)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Volume.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        Audio_Data data = new Audio_Data(vol);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static Audio_Data LoadVolume()
    {
        string path = Application.persistentDataPath + "/Volume.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Audio_Data data = formatter.Deserialize(stream) as Audio_Data;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }



    // inventory
    #region Player
    public static void SavePlayerInventory(List_Slots LS)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/PlayerInventory.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerInventory_Data data = new PlayerInventory_Data(LS);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerInventory_Data LoadPlayerInventory()
    {
        string path = Application.persistentDataPath + "/PlayerInventory.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PlayerInventory_Data data = formatter.Deserialize(stream) as PlayerInventory_Data;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
    #endregion

    #region Container
    public static void SaveContainerInventory(List_Slots LS)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/ContainerInventory.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        ContainerInventory_Data data = new ContainerInventory_Data(LS);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static ContainerInventory_Data LoadContainerInventory()
    {
        string path = Application.persistentDataPath + "/ContainerInventory.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            ContainerInventory_Data data = formatter.Deserialize(stream) as ContainerInventory_Data;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
    #endregion
}
