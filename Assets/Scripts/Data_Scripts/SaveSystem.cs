using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public enum SaveType
    {
        Save_PlayerController,
        Save_PlayerInput,
        Save_PlayerInventory,
        Save_PlayerRecipesInventory,
        Save_ContainerInventory,
        Save_AnimalPenLevel,
        Save_UIMenu,
        Save_Volume,
        Save_SceneVerification
    }

    public static SaveType saveType;

    private static string Path(string save)
    {
        return $"{Application.persistentDataPath}/{save}.save";
    }

    public static void Save(SaveType save, object data)
    {
        saveType = save;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Path(save.ToString()), FileMode.Create);

        switch (saveType)
        {
            case SaveType.Save_PlayerController:
                Player_Data data_PlayerController = new Player_Data((PlayerController)data);
                formatter.Serialize(stream, data_PlayerController);
                break;
            case SaveType.Save_PlayerInput:
                Player_Data data_PlayerInput = new Player_Data((PlayerInput)data);
                formatter.Serialize(stream, data_PlayerInput);
                break;
            case SaveType.Save_PlayerInventory:
                List_Slots LS_PlayerInventory = (List_Slots)data;
                PlayerInventory_Data data_PlayerInventory_Data = new PlayerInventory_Data(LS_PlayerInventory, 10);
                formatter.Serialize(stream, data_PlayerInventory_Data);
                break;
            case SaveType.Save_PlayerRecipesInventory:
                PlayerRecipesInventory data_PlayerRecipesInventory = (PlayerRecipesInventory)data;
                PlayerInventory_Data data_PlayerRecipesInventory_Data = new PlayerInventory_Data(data_PlayerRecipesInventory);
                formatter.Serialize(stream, data_PlayerRecipesInventory_Data);
                break;
            case SaveType.Save_ContainerInventory:
                List_Slots LS_ContainerInventory = (List_Slots)data;
                ContainerInventory_Data data_ContainerInventory_Data = new ContainerInventory_Data(LS_ContainerInventory, 100);
                formatter.Serialize(stream, data_ContainerInventory_Data);
                break;
            case SaveType.Save_AnimalPenLevel:
                Debug.Log($"Save Animal Pen Level");
                /*Player_Data data_AnimalPenLevel = new Player_Data((Shop_Enclos)data);
                formatter.Serialize(stream, data_AnimalPenLevel);*/
                break;
            case SaveType.Save_UIMenu:
                KeyBinding_Data data_KeyBinding_Data = new KeyBinding_Data((UI_Menu)data);
                formatter.Serialize(stream, data_KeyBinding_Data);
                break;
            case SaveType.Save_Volume:
                Audio_Data data_Audio_Data = new Audio_Data((UI_Menu)data);
                formatter.Serialize(stream, data_Audio_Data);
                break;
            case SaveType.Save_SceneVerification:
                Player_Data data_SceneVerification = new Player_Data((SceneVerification)data);
                formatter.Serialize(stream, data_SceneVerification);
                break;
            default:
                break;
        }

        stream.Close();
    }

    public static object Load(SaveType save, object data = null)
    {
        saveType = save;

        if (File.Exists(Path(save.ToString())))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(Path(save.ToString()), FileMode.Open);

            object dataLoaded = null;

            if (stream.Length == 0) return null;

            switch (saveType)
            {
                case SaveType.Save_PlayerController:
                    dataLoaded = formatter.Deserialize(stream) as Player_Data;
                    break;
                case SaveType.Save_PlayerInput:
                    dataLoaded = formatter.Deserialize(stream) as Player_Data;
                    break;
                case SaveType.Save_PlayerInventory:
                    dataLoaded = formatter.Deserialize(stream) as PlayerInventory_Data;
                    break;
                case SaveType.Save_PlayerRecipesInventory:
                    dataLoaded = formatter.Deserialize(stream) as PlayerInventory_Data;
                    break;
                case SaveType.Save_ContainerInventory:
                    dataLoaded = formatter.Deserialize(stream) as ContainerInventory_Data;
                    break;
                case SaveType.Save_AnimalPenLevel:
                    Debug.Log($"Load Animal Pen Level");
                    //dataLoaded = formatter.Deserialize(stream) as Player_Data;
                    break;
                case SaveType.Save_UIMenu:
                    dataLoaded = formatter.Deserialize(stream) as KeyBinding_Data;
                    break;
                case SaveType.Save_Volume:
                    dataLoaded = formatter.Deserialize(stream) as Audio_Data;
                    break;
                case SaveType.Save_SceneVerification:
                    dataLoaded = formatter.Deserialize(stream) as Player_Data;
                    break;
                default:
                    break;
            }

            stream.Close();

            return dataLoaded;
        }
        else
        {
            //Debug.LogError($"Save file not found in {Path(save.ToString())}");

            if (data == null) return null;

            switch (saveType)
            {
                case SaveType.Save_PlayerController:
                    break;
                case SaveType.Save_PlayerInput:
                    break;
                case SaveType.Save_PlayerInventory:
                    List_Slots LS_PlayerInventory = (List_Slots)data;
                    LS_PlayerInventory.HandleVerificationAndApplication();
                    break;
                case SaveType.Save_PlayerRecipesInventory:
                    break;
                case SaveType.Save_ContainerInventory:
                    List_Slots LS_ContainerInventory = (List_Slots)data;
                    LS_ContainerInventory.HandleVerificationAndApplication();
                    break;
                case SaveType.Save_AnimalPenLevel:
                    break;
                case SaveType.Save_UIMenu:
                    break;
                case SaveType.Save_Volume:
                    break;
                case SaveType.Save_SceneVerification:
                    break;
                default:
                    break;
            }

            Save(save, data);

            return null;
        }
    }

    #region OLD

    //player save
    /*public static void SavePlayerInventory (PlayerInventory playerInventory)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/PlayerInventory.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        Player_Data data = new Player_Data(playerInventory);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SavePlayerMoney(PlayerController playerController)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playercontroller.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        Player_Data data = new Player_Data(playerController);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SavePlayerPosition(PlayerInput playerInput)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerinput.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        Player_Data data = new Player_Data(playerInput);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SaveScene(SceneVerification scene_Verification)
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
    }
    public static Player_Data LoadPlayerInput(List_Slots LS, PlayerInput playerInput)
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

            LS.HandleVerificationAndApplication();

            SavePlayerPosition(playerInput);

            return null;
        }
    }
    public static Player_Data LoadPlayerController(PlayerController pC)
    {
        string path = Application.persistentDataPath + "/playercontroller.save";
        Debug.Log(Application.persistentDataPath);
        Debug.Log(path);
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

            SavePlayerMoney(pC);

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

        PlayerInventory_Data data = new PlayerInventory_Data(LS, 10);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerInventory_Data LoadPlayerInventory(List_Slots LS)
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

            LS.HandleVerificationAndApplication();

            SavePlayerInventory(LS);

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

        ContainerInventory_Data data = new ContainerInventory_Data(LS, 100);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static ContainerInventory_Data LoadContainerInventory(List_Slots LS)
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

            LS.HandleVerificationAndApplication();

            SaveContainerInventory(LS);

            return null;
        }
    }
    #endregion*/

    #endregion
}