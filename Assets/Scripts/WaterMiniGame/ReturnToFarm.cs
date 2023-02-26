using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToFarm : MonoBehaviour
{
    public void LoadBackToFarm()
    {
        SceneManager.LoadScene("FarmScene");
    }
}
