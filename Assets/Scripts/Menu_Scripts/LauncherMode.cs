using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LauncherMode : MonoBehaviour
{
    public enum LaunchMode
    {
        New,
        Continue
    }

    public LaunchMode launchMode;

    public void Launch()
    {
        if (launchMode == LaunchMode.New) SaveSystem.DeleteAllSaves();

        SceneManager.LoadScene("FarmScene");
    }

    private void OnLevelWasLoaded()
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