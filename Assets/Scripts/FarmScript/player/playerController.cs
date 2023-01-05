using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    public bool TalkingShop = false;
    public bool farm = false;
    private playerInput PlayerInput;
    private Shop_Enclos shop_Enclos;
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
    private bool verifVillage = true;
    private Scene_verification sv;
    private float tpssave = 0.2f;
    private bool tpssav = false;
    private bool Right = false;
    private bool Left = false;
    private bool Shop = false;

    // Start is called before the first frame update
    void Awake()
    {
        cc = GetComponent<CharacterController>();
        PlayerInput = GetComponent<playerInput>();
        sv = GetComponent<Scene_verification>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(shop_Enclos == null && SceneManager.GetActiveScene().buildIndex == 2)
        {
            shop_Enclos = FindObjectOfType<Shop_Enclos>();
        }
        if(tpssav == true)
        {

            tpssave = tpssave - Time.deltaTime;
            if (tpssave <= 0)
            {
                LoadPlayerPos();
                tpssave = 0.2f;
                tpssav = false;
            }
        }
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (verifVillage == true)
            {
                cc.enabled = false;
                cc.transform.position = new Vector3(4.85f, 5.09f, -87.45f);
                cc.enabled = true;
                verifVillage = false;
            }
            
            PlayerMovementFarm();
            
        }
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (verifVillage == false)
            {
                cc.enabled = false;
                cc.transform.position = new Vector3(-19.07f, 1.8f, 0);
                cc.enabled = true;
                verifVillage = true;
            }
            
            PlayerMovementVillage();
        }
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SavePlayerPos();
                Debug.Log("Save");
            }
            
            if (Input.GetKeyDown(KeyCode.F9))
            {
                tpssav = true;
                if (SceneManager.GetActiveScene().buildIndex != sv.sceneIndexSave)
                {
                    sv.LoadPlayerSc();
                }
                
                
                Debug.Log("Load");
            }
        }

    }

    void PlayerMovementFarm()
    {
        if (!PlayerInput) return;
        _horizontal = PlayerInput.Movement.x;
        _vertical = PlayerInput.Movement.y;
        direction.Set(_horizontal, 0, _vertical);

        if (direction.normalized.magnitude >= 0.1f)
        {
            _targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, _angle, 0);
            direction = Quaternion.Euler(0, _targetAngle, 0) * Vector3.forward;
        }
        PlayerSpeed = direction.normalized * movespeed * Time.deltaTime;
        cc.Move(PlayerSpeed);
    }

    void PlayerMovementVillage()
    {
        if (TalkingShop == true && Shop == false)
        {

            cc.enabled = false;
            cc.transform.rotation = Quaternion.Euler(0, 0, 0);
            cc.enabled = true;
            Left = false;
            Right = false;
            Shop = true;


        }
        if (TalkingShop == false)
        {
            if (!PlayerInput) return;
            _horizontal = PlayerInput.Movement.x;
            direction.Set(_horizontal, 0, 0);
            PlayerSpeed = direction.normalized * movespeed * Time.deltaTime;
            
            cc.Move(PlayerSpeed);
            
            if(PlayerSpeed.x > 0 && Right == false)
            {
                cc.enabled = false;
                cc.transform.rotation = Quaternion.Euler(0, 90, 0);
                cc.enabled = true;
                Right = true;
                Left = false;
                Shop = false;
            }
            if (PlayerSpeed.x < 0 && Left == false)
            {
                
                cc.enabled = false;
                cc.transform.rotation = Quaternion.Euler(0, -90, 0);
                cc.enabled = true;
                Left = true;
                Right = false;
                Shop = false;
                

            }
            
            if (verifVillage == false)
            {
                cc.enabled = false;
                cc.transform.rotation = Quaternion.Euler(0,0,0) ;
                verifVillage = true;
            }
            
        }
        else return;
        

    }
    void SavePlayerPos()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SaveSystem.SavePlayerPosition(PlayerInput);
        }

    }
    void LoadPlayerPos()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {

            Player_Data data = SaveSystem.LoadPlayerInput();
            Vector3 position;
            position.x = data.position[0];
            position.y = data.position[1];
            position.z = data.position[2];
            cc.enabled = false;
            transform.position = position;
            cc.enabled = true;
        }
    }
    
}
