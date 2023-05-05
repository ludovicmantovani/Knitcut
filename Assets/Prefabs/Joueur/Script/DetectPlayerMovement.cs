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

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        _lastPosition = transform.position;

        if (animator) animator.SetFloat("WalkAnimationSpeed", walkAnimationSpeed);
    }

    void Update()
    {
        if (animator == null) return;

        walkAnimationSpeed = playerController.CurrentSpeed;

        walkAnimationSpeed = Mathf.Clamp(walkAnimationSpeed, 0f, 1f);
        walkAnimationSpeed = Mathf.SmoothDamp(animator.GetFloat("WalkAnimationSpeed"), walkAnimationSpeed, ref velocity, 0.1f);

        animator.SetFloat("WalkAnimationSpeed", walkAnimationSpeed);

        float movement = Vector3.Magnitude(transform.position - _lastPosition);
        _isWalking = movement > movementThreshold;
        animator.SetBool("IsWalking", _isWalking);
        _lastPosition = transform.position;
    }
}