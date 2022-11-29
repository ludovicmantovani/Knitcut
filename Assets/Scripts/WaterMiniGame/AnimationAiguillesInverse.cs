using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AnimationAiguillesInverse : MonoBehaviour
{
    public Animator animator;
    public GameObject Bot;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Bot.SetActive(false);
            animator.SetBool("ContactAiguilleInverse", true);
        }
    }
}
