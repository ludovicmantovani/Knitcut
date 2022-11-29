using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Menu : MonoBehaviour
{
    [SerializeField]private TMP_InputField moveForward;
    [SerializeField] private TMP_InputField moveBackward;
    [SerializeField] private TMP_InputField moveRight;
    [SerializeField] private TMP_InputField moveLeft;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
