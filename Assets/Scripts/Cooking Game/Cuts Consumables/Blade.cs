using UnityEngine;

public class Blade : MonoBehaviour
{
    [SerializeField] private GameObject bladeTrailPrefab;
    //[SerializeField] private float minCuttingVelocity = 0.001f;
    [SerializeField] private float delayBeforeDestroyingBladeTrail = 2f;
    [SerializeField] private bool canCut;
    [SerializeField] private bool cutDetect;
    [SerializeField] private bool isCutting;

    public bool CanCut
    {
        get { return canCut; }
        set { canCut = value; }
    }

    public bool CutDetect
    {
        get { return cutDetect; }
        set { cutDetect = value; }
    }

    public bool IsCutting
    {
        get { return isCutting; }
        set { isCutting = value; }
    }

    private Vector2 previousPosition;
    private GameObject currentBladeTrail;

    public GameObject CurrentBladeTrail
    {
        get { return currentBladeTrail; }
        set { currentBladeTrail = value; }
    }

    Rigidbody2D _rigidbody;
    CircleCollider2D _circleCollider;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();

        canCut = false;
        cutDetect = false;
        isCutting = false;
    }

    private void Update()
    {
        if (cutDetect && canCut)
            HandleCut();

        if (canCut && !cutDetect)
            StopCutting();
    }

    private void HandleCut()
    {
        if (Input.GetMouseButtonDown(0))
            StartCutting();
        else if (Input.GetMouseButtonUp(0) && isCutting)
            StopCutting();

        if (isCutting)
            UpdateCut();
    }

    private void UpdateCut()
    {
        if (!cutDetect) return;

        Vector2 newPosition =Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _rigidbody.position = newPosition;
        transform.position = _rigidbody.position;

        /*float velocity = (newPosition - previousPosition).magnitude / Time.deltaTime;

        if (velocity > minCuttingVelocity)
        {
            _circleCollider.enabled = true;
        }
        else
        {
            _circleCollider.enabled = false;
        }*/

        previousPosition = newPosition;
    }

    private void StartCutting()
    {
        _circleCollider.enabled = true;
        isCutting = true;

        currentBladeTrail = Instantiate(bladeTrailPrefab, transform);

        previousPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void StopCutting()
    {
        _circleCollider.enabled = false;
        isCutting = false;

        if (currentBladeTrail == null) return;
         
        currentBladeTrail.transform.SetParent(null);

        Destroy(currentBladeTrail, delayBeforeDestroyingBladeTrail);
    }
}