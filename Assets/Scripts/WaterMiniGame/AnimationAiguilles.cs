using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AnimationAiguilles : MonoBehaviour
{
    public Animator animator;
    public GameObject Top;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Top.SetActive(false);
            animator.SetBool("ContactAiguille", true);
        }
    }
}
