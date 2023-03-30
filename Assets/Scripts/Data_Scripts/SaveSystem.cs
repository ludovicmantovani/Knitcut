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

    public static void DeleteAllSaves()
    {
        string path = Application.persistentDataPath;

        string[] files = Directory.GetFiles(path);

        foreach (string file in files)
        {
            File.Delete(file);
            Debug.Log($"{file} is deleted");
        }
    }
}