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

        pI = GetComponent<PlayerInput>();
    }
    private void Update()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (pI.actions["QuickSave"].triggered)
            {
                SaveplayerSc();
            }
            if (pI.actions["QuickLoad"].triggered)
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
