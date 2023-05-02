using UnityEngine;

public class DetectPlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private float movementThreshold = 0.01f;
    [SerializeField] private float walkAnimationSpeed = 1f;
    private Vector3 _lastPosition;
    private bool _isWalking = false;

    private PlayerController playerController;

    public bool IsWalking { get => _isWalking;}

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        _lastPosition = transform.position;

        if (animator) animator.SetFloat("WalkAnimationSpeed", playerController.Direction.magnitude);
    }


    void Update()
    {
        if (animator == null) return;

        animator.SetFloat("WalkAnimationSpeed", playerController.Direction.magnitude);

        float movement = Vector3.Magnitude(transform.position - _lastPosition);
        _isWalking = movement > movementThreshold;
        animator.SetBool("IsWalking", _isWalking);
        _lastPosition = transform.position;
    }
}
