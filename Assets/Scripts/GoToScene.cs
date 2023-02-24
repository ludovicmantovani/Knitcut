using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
    [SerializeField] private string sceneName = "";

    private void Start()
    {
        if (sceneName.Length <= 0)
            sceneName = SceneManager.GetActiveScene().name;
    }
    public void SwitchScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
