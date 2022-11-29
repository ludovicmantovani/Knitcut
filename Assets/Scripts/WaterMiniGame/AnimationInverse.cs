using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInverse : MonoBehaviour
{
    public Animator animator;
    public GameObject BotDetect;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            BotDetect.SetActive(false);
            animator.SetBool("ContactInverse", true);
        }
    }
}
