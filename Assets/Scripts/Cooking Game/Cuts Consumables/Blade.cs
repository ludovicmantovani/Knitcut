using UnityEngine;

public class Blade : MonoBehaviour
{
    [SerializeField] private GameObject bladeTrailPrefab;
    [SerializeField] private float minCuttingVelocity = 0.001f;
    [SerializeField] private float delayBeforeDestroyingBladeTrail = 2f;
    [SerializeField] private bool isCutting = false;

    private Vector2 previousPosition;
    private GameObject currentBladeTrail;

    Rigidbody2D _rigidbody;
    CircleCollider2D _circleCollider;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        CutInArea();
    }

    private void CutInArea()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            // If is in Area
            if (hit.collider.CompareTag("Area"))
            {
                HandleCut();
            }
        }
    }

    private void HandleCut()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCutting();
        }
        else if (Input.GetMouseButtonUp(0) && isCutting)
        {
            StopCutting();
        }

        if (isCutting)
        {
            UpdateCut();
        }
    }

    private void UpdateCut()
    {
        Vector2 newPosition =Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _rigidbody.position = newPosition;
        transform.position = _rigidbody.position;

        float velocity = (newPosition - previousPosition).magnitude / Time.deltaTime;

        if (velocity > minCuttingVelocity)
        {
            _circleCollider.enabled = true;
        }
        else
        {
            _circleCollider.enabled = false;
        }

        previousPosition = newPosition;
    }

    private void StartCutting()
    {
        isCutting = true;
        currentBladeTrail = Instantiate(bladeTrailPrefab, transform);
        previousPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void StopCutting()
    {
        isCutting = false;
        currentBladeTrail.transform.SetParent(null);

        Destroy(currentBladeTrail, delayBeforeDestroyingBladeTrail);
    }
}