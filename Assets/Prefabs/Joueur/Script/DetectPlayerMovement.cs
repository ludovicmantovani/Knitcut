using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private float movementThreshold = 0.01f;
    [SerializeField] private float walkAnimationSpeed = 1f;
    private Vector3 _lastPosition;
    private bool _isWalking = false;

    public bool IsWalking { get => _isWalking;}

    void Start()
    {
        _lastPosition = transform.position;
        if (animator) animator.SetFloat("WalkAnimationSpeed", walkAnimationSpeed);
    }


    void Update()
    {
        if (animator == null) return;

        animator.SetFloat("WalkAnimationSpeed", walkAnimationSpeed);

        float movement = Vector3.Magnitude(transform.position - _lastPosition);
        _isWalking = movement > movementThreshold;
        animator.SetBool("IsWalking", _isWalking);
        _lastPosition = transform.position;
    }
}
