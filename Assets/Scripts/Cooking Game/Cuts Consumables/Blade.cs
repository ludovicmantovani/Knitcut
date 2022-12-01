using UnityEngine;

public class Blade : MonoBehaviour
{
    public GameObject bladeTrailPrefab;
    public float minCuttingVelocity = 0.001f;

    private bool isCutting = false;

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
        if (Input.GetMouseButtonDown(0))
        {
            StartCutting();
        }
        else if (Input.GetMouseButtonUp(0))
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

        _circleCollider.enabled = true;
    }

    private void StopCutting()
    {
        isCutting = false;
        currentBladeTrail.transform.SetParent(null);

        Destroy(currentBladeTrail, 2f);

        _circleCollider.enabled = false;
    }
}