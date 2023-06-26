using UnityEngine;

public static class StaticCoroutine
{
    public class StaticCoroutineMB : MonoBehaviour {}
    
    private static StaticCoroutineMB instance;

    private static void Init()
    {
        if (instance == null)
        {
            //Create an empty object called MyStatic
            GameObject gameObject = new GameObject("MyStatic");

            //Add this script to the object
            instance = gameObject.AddComponent<StaticCoroutineMB>();
        }
    }

    public static void StartStaticCoroutine()
    {
        Init();
        instance.StartCoroutine(GameManager.SwitchingScene());
    }
}