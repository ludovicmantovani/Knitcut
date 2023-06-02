using UnityEngine;

public class DetectPlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private float movementThreshold = 0.01f;
    [SerializeField] private float walkAnimationSpeed = 0f;
    private Vector3 _lastPosition;
    private bool _isWalking = false;
    private float velocity;

    private PlayerController playerController;

    public bool IsWalking { get => _isWalking;}

    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        _lastPosition = transform.position;

        if (animator) animator.SetFloat("WalkAnimationSpeed", walkAnimationSpeed);
    }

    private void Update()
    {
        if (animator == null) return;

        HandleAnimatorMovement();
    }

    private void HandleAnimatorMovement()
    {
        if (playerController.CanMove)
            walkAnimationSpeed = playerController.CurrentSpeed;
        else
            walkAnimationSpeed = 0f;

        HandleAnimator(walkAnimationSpeed);
    }

    private void HandleAnimator(float speed)
    {
        speed = Mathf.Clamp(speed, 0f, 1f);
        speed = Mathf.SmoothDamp(animator.GetFloat("WalkAnimationSpeed"), speed, ref velocity, 0.1f);

        animator.SetFloat("WalkAnimationSpeed", speed);

        float movement = Vector3.Magnitude(transform.position - _lastPosition);
        _isWalking = movement > movementThreshold;
        animator.SetBool("IsWalking", _isWalking);
        _lastPosition = transform.position;
    }
}