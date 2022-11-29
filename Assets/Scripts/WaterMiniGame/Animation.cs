using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Animation : MonoBehaviour
{
    public Animator animator;
    public GameObject TopDetect;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            TopDetect.SetActive(false);
            animator.SetBool("Contact", true);
        }
    }
}
