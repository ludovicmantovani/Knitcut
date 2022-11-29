using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    private playerInput PlayerInput;
    private Vector3 PlayerSpeed;
    private float _horizontal;
    private float _vertical;
    private Vector3 direction;

    [SerializeField] private Camera cam;
    private float _turnSmoothVelocity;
    [SerializeField] private float turnSmoothTime;
    [SerializeField] private float movespeed;
    private float _targetAngle;
    private float _angle;
    private CharacterController  cc ;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        PlayerInput = GetComponent<playerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!PlayerInput)return;
        _horizontal = PlayerInput.Movement.x;
        _vertical = PlayerInput.Movement.y;
        direction.Set(_horizontal,0,_vertical);

        if(direction.normalized.magnitude >= 0.1f){
            _targetAngle = Mathf.Atan2(direction.x, direction.z)* Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,_targetAngle, ref _turnSmoothVelocity,turnSmoothTime);
            transform.rotation =Quaternion.Euler(0,_angle,0);
            direction = Quaternion.Euler(0,_targetAngle,0)*Vector3.forward;
        }
        PlayerSpeed = direction.normalized * movespeed * Time.deltaTime;
        cc.Move(PlayerSpeed);

    }
}
