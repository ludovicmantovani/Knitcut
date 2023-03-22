using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Scene_verification : MonoBehaviour
{

    public PlayerInput pI;
    public int sceneIndex;
    public int sceneIndexSave;
    private void Awake()
    {

        pI = FindObjectOfType<PlayerInput>();
    }
    private void Update()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (pI.QuickSaveAction.triggered)
            {
                SaveplayerSc();
            }
            if (pI.QuickLoadAction.triggered)
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
