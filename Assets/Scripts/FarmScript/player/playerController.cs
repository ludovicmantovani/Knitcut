using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//rajout nouveau system input
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{

    // rajout pour rotation dans village
    public GameObject playerBody;

    // money
    public int money = 0;
    // rajout pou desactiver dans village
    public GameObject cameraFerme;
    public PlayerInput pI;
    public bool TalkingShop = false;
    public bool farm = false;
    private playerInput PlayerInput;
    private Shop_Enclos shop_Enclos;
    private Vector3 PlayerSpeed;
    private float _horizontal;
    private float _vertical;
    private Vector3 direction;

    [SerializeField] private Camera cam;
    // rajout pou desactiver dans village
    [SerializeField] private GameObject cameraFermeCinemachine;
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


    void Awake()
    {
        //rajout component PlayerInput
        pI = GetComponent<PlayerInput>();
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
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            //modif
            if (verifVillage == true)
            {
                cc.enabled = false;
                cc.transform.position = new Vector3(4.85f, 5.09f, -87.45f);
                // reinitialise position et rotation du corps
                playerBody.transform.position = cc.transform.position;
                playerBody.transform.rotation = cc.transform.rotation;
                cc.enabled = true;
                verifVillage = false;
                cameraFermeCinemachine.SetActive(true);
            }
            //modif
            //PlayerMovementFarm();
            PlayerMovementFarmV2();


        }
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            //modif
            if (verifVillage == false)
            {
                cc.enabled = false;
                cc.transform.position = new Vector3(-19.07f, 1.8f, 0);
                //modifie position du player
                cc.transform.rotation = Quaternion.Euler(0, 0, 0);
                cc.enabled = true;
                verifVillage = true;
                //descative camera cinemachine et change position camera
                cameraFermeCinemachine.SetActive(false);
                cameraFerme.transform.position = new Vector3(0f, 2f, -7f);
                cameraFerme.transform.rotation = Quaternion.Euler(0, 0, 0);


            }
            
            PlayerMovementVillage();
        }
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            //modif change input
            if (pI.actions["QuickSave"].triggered)
            {
                SavePlayerPos();
                Debug.Log("Save");
            }
            //modif change input
            if (pI.actions["QuickLoad"].triggered)
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
    // nouveau system de deplacement
    void PlayerMovementFarmV2()
    {

        Vector2 input = pI.actions["Move"].ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0,input.y);
        move = move.x * cam.transform.right + move.z * cam.transform.forward;
        move.y = 0f;
        cc.Move(move * Time.deltaTime * movespeed);

        if(input != Vector2.zero)
        {
            _targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            //Quaternion rotation = Quaternion.Euler(0, _targetAngle, 0);
            transform.rotation = Quaternion.Euler(0, _angle, 0);
            //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnSmoothTime);
            
            move = Quaternion.Euler(0, _targetAngle, 0) * Vector3.forward;
        }
        PlayerSpeed = move;
    }
    //desactive ancie systeme
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
    // synchro avec nouveau input system
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
        //modif
        if (TalkingShop == false)
        {
            //ancien system
            /*if (!PlayerInput) return;
            _horizontal = PlayerInput.Movement.x;
            direction.Set(_horizontal, 0, 0);
            PlayerSpeed = direction.normalized * movespeed * Time.deltaTime;
            
            cc.Move(PlayerSpeed);*/
            //nouveau system
            Vector2 input = pI.actions["Move"].ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0, 0);
            move = move.x * cam.transform.right + move.z * cam.transform.forward;
            move.y = 0f;
            cc.Move(move * Time.deltaTime * movespeed);
            PlayerSpeed = move;
            //modif rotation corps
            if (PlayerSpeed.x > 0 && Right == false)
            {
                cc.enabled = false;
                playerBody.transform.rotation = Quaternion.Euler(0, 90, 0);
                cc.enabled = true;
                Right = true;
                Left = false;
                Shop = false;
            }
            //modif rotation corps
            if (PlayerSpeed.x < 0 && Left == false)
            {
                
                cc.enabled = false;
                playerBody.transform.rotation = Quaternion.Euler(0, -90, 0);
                cc.enabled = true;
                Left = true;
                Right = false;
                Shop = false;
                

            }
            //modif rotation corps
            if (verifVillage == false)
            {
                cc.enabled = false;
                playerBody.transform.rotation = Quaternion.Euler(0,0,0) ;
                verifVillage = true;
            }
            
        }
        else return;
        

    }
    /*void SavePlayerPos()
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
    }*/

    public void SavePlayerPos()
    {
        SaveSystem.SavePlayerPosition(PlayerInput);
    }
    private void LoadPlayerPos()
    {
        Vector3 playerPosition = Vector3.zero;

        if (SceneManager.GetActiveScene().name.Contains("Farm"))
        {
            Player_Data data = SaveSystem.LoadPlayerInput();

            if (data.playerPosition == null) return;

            playerPosition = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);

            Debug.Log($"Go to farm : {playerPosition}");
        }
        else if (SceneManager.GetActiveScene().name.Contains("Village"))
        {
            playerPosition = FindObjectOfType<Voiture_Interaction>().transform.position;
            //playerPosition = new Vector3(data.villagePosition[0], data.villagePosition[1], data.villagePosition[2]);

            Debug.Log($"Go to village : {playerPosition}");
        }

        cc.enabled = false;
        transform.position = playerPosition; 
        cc.enabled = true;
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            LoadPlayerPos();
        }
    }
}}
