using UnityEngine;

public class PetalInput : MonoBehaviour
{
    public delegate void OnChangeEvent(string value);
    public event OnChangeEvent OnChange;

    private Animator _animator = null;
    private Rigidbody2D _rigidBody = null;

    private bool _isFalling = false;
    private float _time = 0f;
    private float _randomRoot;

    [SerializeField]
    private float scale = 500f;

    [SerializeField]
    private bool wind = true;

    private void Start()
    {
        _randomRoot = Random.Range(0f, 100f);
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();
        if (_rigidBody) _rigidBody.isKinematic = true;
    }

    private void Update()
    {
        if (_isFalling && _rigidBody && wind)
        {
            _time += Time.deltaTime;

            float perlinValue = Mathf.PerlinNoise(_time, _randomRoot);
            Vector2 windForce = new Vector2((perlinValue - 0.5f) / scale, 0);

            _rigidBody.AddForce(windForce);
        }
    }

    public void Change()
    {
        if (_animator) _animator.SetTrigger("BalanceTrigger");
        if (OnChange != null)
            OnChange(gameObject.name);
    }

    public void Fall()
    {
        if (_isFalling == false)
        {
            if (_animator) _animator.enabled = !enabled;
            if (_rigidBody) _rigidBody.isKinematic = false;
            _isFalling = true;
        }
    }

}
