using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LauncherMode : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "FarmScene";

    public enum LaunchMode
    {
        New,
        Continue
    }

    public LaunchMode launchMode;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoaded;
    }

    public void Launch()
    {
        if (launchMode == LaunchMode.New) SaveSystem.DeleteAllSaves();

        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnLevelFinishedLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (SceneManager.GetActiveScene().name.Contains("Farm") || SceneManager.GetActiveScene().name.Contains("Village"))
        {
            if (launchMode == LaunchMode.New)
            {
                PlayerController playerController = FindObjectOfType<PlayerController>();

                playerController.ListSlots.CreateTempItems();

                Debug.Log($"Initialization New Game OK");
            }
            else if (launchMode == LaunchMode.Continue)
            {
                PlayerController playerController = FindObjectOfType<PlayerController>();

                playerController.LoadPlayerPositionInScene();

                Debug.Log($"Initialization Continue Game OK");
            }

            Destroy(gameObject);
        }
    }
}