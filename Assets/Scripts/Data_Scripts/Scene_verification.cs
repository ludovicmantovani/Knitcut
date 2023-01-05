using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_verification : MonoBehaviour
{
    public int sceneIndex;
    public int sceneIndexSave;
    private void Awake()
    {
        
    }
    private void Update()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveplayerSc();
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                LoadPlayerSc();
            }
        }
        
    }
    void SaveplayerSc()
    {
        SaveSystem.SaveScene(this);
    }
    public void LoadPlayerSc()
    {
        Player_Data data = SaveSystem.LoadPlayerScene();
        sceneIndexSave = data.scene;

        SceneManager.LoadScene(sceneIndexSave);
    }
}
