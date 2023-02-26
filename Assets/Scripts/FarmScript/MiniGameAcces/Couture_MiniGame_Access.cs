using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Couture_MiniGame_Access : MonoBehaviour
{
    public PlayerInput pI;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && pI.actions["Intercation_Environnements"].triggered)
        {
            SceneManager.LoadScene("Recognition");
        }
    }
}
