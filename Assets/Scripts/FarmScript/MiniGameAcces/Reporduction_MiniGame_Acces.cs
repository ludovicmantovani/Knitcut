using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class Reporduction_MiniGame_Acces : MonoBehaviour
{
    public PlayerInput pI;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && pI.actions["Intercation_Environnements"].triggered)
        {
            SceneManager.LoadScene("Flowers Game");
        }
    }
}
