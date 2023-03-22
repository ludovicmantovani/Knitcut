using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    List_Slots LS;
    PlayerInventory playerInventory;
    PlayerRecipesInventory pRInventory;

    [Header("References")]
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private string farmSceneName = "FarmScene";
    [SerializeField] private string villageSceneName = "TradingVillage";
    [SerializeField] private GameObject playerBody;
    [SerializeField] private int money = 0;

    public int Money
    {
        get { return money; }
        set { money = value; }
    }

    [Header("Cameras")]
    [SerializeField] private Camera cam;
    // rajout pou desactiver dans village
    [SerializeField] private GameObject cameraFerme;
    // rajout pou desactiver dans village
    [SerializeField] private GameObject cameraFermeCinemachine;

    public GameObject CameraFC
    {
        get { return cameraFermeCinemachine; }
    }

    [Header("Datas")]
    [SerializeField] private float turnSmoothVelocity;
    [SerializeField] private float turnSmoothTime;
    [SerializeField] private float movespeed;
    [SerializeField] private bool talkingShop = false;
    [SerializeField] private bool inFarm = false;

    public bool TalkingShop
    {
        get { return talkingShop; }
        set { talkingShop = value; }
    }

    public bool InFarm
    {
        get { return inFarm; }
        set { inFarm = value; }
    }

    private CharacterController  characterController;
    private Scene_verification sceneVerif;
    private PlayerInput playerInput;

    public PlayerInput PlayerInput
    {
        get { return playerInput; }
    }

    private Vector3 playerSpeed;

    private float _targetAngle;
    private float _angle;

    private bool verifVillage = true;
    private bool right = false;
    private bool left = false;
    private bool shop = false;
    private bool canMove = true;

    public bool CanMove
    { 
        get { return canMove; }
        set { canMove = value; }
    }

    private void Awake()
    {
        LS = FindObjectOfType<List_Slots>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        pRInventory = FindObjectOfType<PlayerRecipesInventory>();

        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        sceneVerif = GetComponent<Scene_verification>();
    }

    private void Update()
    {
        //HandleInventoriesInFarmAndVillage();
        HandleInventoriesInFarmAndVillageV2();

        //HandlePlayerInFarmScene();
        HandlePlayerInFarmSceneV2();
        //HandlePlayerInVillageScene();
        HandlePlayerInVillageSceneV2();

        ForceSaveAndLoad();
    }

    #region Handle Player in Farm Scene & Village Scene V1

    private void HandlePlayerInFarmScene()
    {
        if (SceneManager.GetActiveScene().name == farmSceneName)
        {
            if (verifVillage == true)
            {
                characterController.enabled = false;
                characterController.transform.position = spawnPosition.position;
                // reinitialise position et rotation du corps
                playerBody.transform.position = characterController.transform.position;
                playerBody.transform.rotation = characterController.transform.rotation;
                characterController.enabled = true;
                verifVillage = false;
                cameraFermeCinemachine.SetActive(true);
            }

            PlayerMovementFarm();
        }
    }

    private void HandlePlayerInVillageScene()
    {
        if (SceneManager.GetActiveScene().name == villageSceneName)
        {
            if (verifVillage == false)
            {
                characterController.enabled = false;
                characterController.transform.position = spawnPosition.position;
                //modifie position du player
                characterController.transform.rotation = Quaternion.Euler(0, 0, 0);
                characterController.enabled = true;
                verifVillage = true;
                //descative camera cinemachine et change position camera
                cameraFermeCinemachine.SetActive(false);
                //cameraFerme.transform.position = new Vector3(0f, 2f, -7f);
                //cameraFerme.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            PlayerMovementVillage();
        }
    }

    private void PlayerMovementFarm()
    {
        if (!canMove) return;

        Vector2 input = playerInput.MoveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);

        move = move.x * cam.transform.right + move.z * cam.transform.forward;
        move.y = 0f;

        characterController.Move(move * Time.deltaTime * movespeed);

        if (input != Vector2.zero)
        {
            _targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //Quaternion rotation = Quaternion.Euler(0, _targetAngle, 0);
            transform.rotation = Quaternion.Euler(0, _angle, 0);
            //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnSmoothTime);

            move = Quaternion.Euler(0, _targetAngle, 0) * Vector3.forward;
        }

        playerSpeed = move;
    }

    private void PlayerMovementVillage()
    {
        if (!canMove) return;

        if (talkingShop == true && shop == false)
        {

            characterController.enabled = false;
            characterController.transform.rotation = Quaternion.Euler(0, 0, 0);
            characterController.enabled = true;
            left = false;
            right = false;
            shop = true;
        }

        //modif
        if (talkingShop == false)
        {
            //nouveau system
            Vector2 input = playerInput.MoveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0, 0);
            move = move.x * cam.transform.right + move.z * cam.transform.forward;
            move.y = 0f;
            characterController.Move(move * Time.deltaTime * movespeed);
            playerSpeed = move;

            //modif rotation corps
            if (playerSpeed.x > 0 && right == false)
            {
                characterController.enabled = false;
                playerBody.transform.rotation = Quaternion.Euler(0, 90, 0);
                characterController.enabled = true;
                right = true;
                left = false;
                shop = false;
            }
            //modif rotation corps
            if (playerSpeed.x < 0 && left == false)
            {

                characterController.enabled = false;
                playerBody.transform.rotation = Quaternion.Euler(0, -90, 0);
                characterController.enabled = true;
                left = true;
                right = false;
                shop = false;


            }
            //modif rotation corps
            if (verifVillage == false)
            {
                characterController.enabled = false;
                playerBody.transform.rotation = Quaternion.Euler(0, 0, 0);
                verifVillage = true;
            }

        }
        else return;


    }

    private void HandleInventoriesInFarmAndVillage()
    {
        if (SceneManager.GetActiveScene().name.Contains("Farm") || SceneManager.GetActiveScene().name.Contains("Village"))
        {
            playerInventory.HandleInventoryUI();
            pRInventory.HandleInventoryUI();
        }
    }

    #endregion

    #region Handle Player in Farm Scene & Village Scene V2

    private void HandlePlayerInFarmSceneV2()
    {
        if (SceneManager.GetActiveScene().name.Equals(farmSceneName))
        {
            if (verifVillage == true)
            {
                characterController.enabled = false;
                characterController.transform.position = spawnPosition.position;
                // reinitialise position et rotation du corps
                playerBody.transform.position = characterController.transform.position;
                playerBody.transform.rotation = characterController.transform.rotation;
                characterController.enabled = true;
                verifVillage = false;
                cameraFermeCinemachine.SetActive(true);
            }

            PlayerMovementFarmV2();
        }
    }

    private void PlayerMovementFarmV2()
    {
        if (!canMove) return;

        Vector2 input = playerInput.MoveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);

        move = move.x * cam.transform.right + move.z * cam.transform.forward;
        move.y = 0f;

        characterController.Move(move * Time.deltaTime * movespeed);

        if (input != Vector2.zero)
        {
            _targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //Quaternion rotation = Quaternion.Euler(0, _targetAngle, 0);
            transform.rotation = Quaternion.Euler(0, _angle, 0);
            //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnSmoothTime);

            move = Quaternion.Euler(0, _targetAngle, 0) * Vector3.forward;
        }

        playerSpeed = move;
    }

    private void HandlePlayerInVillageSceneV2()
    {
        if (SceneManager.GetActiveScene().name == villageSceneName)
        {
            if (verifVillage == false)
            {
                characterController.enabled = false;
                characterController.transform.position = spawnPosition.position;
                //modifie position du player
                characterController.transform.rotation = Quaternion.Euler(0, 0, 0);
                characterController.enabled = true;
                verifVillage = true;
                //descative camera cinemachine et change position camera
                cameraFermeCinemachine.SetActive(false);
                //cameraFerme.transform.position = new Vector3(0f, 2f, -7f);
                //cameraFerme.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            PlayerMovementVillageV2();
        }
    }

    private void PlayerMovementVillageV2()
    {
        if (!canMove) return;

        if (talkingShop == true && shop == false)
        {

            characterController.enabled = false;
            characterController.transform.rotation = Quaternion.Euler(0, 0, 0);
            characterController.enabled = true;
            left = false;
            right = false;
            shop = true;
        }

        //modif
        if (talkingShop == false)
        {
            //nouveau system
            Vector2 input = playerInput.MoveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0, 0);
            move = move.x * cam.transform.right + move.z * cam.transform.forward;
            move.y = 0f;
            characterController.Move(move * Time.deltaTime * movespeed);
            playerSpeed = move;

            //modif rotation corps
            if (playerSpeed.x > 0 && right == false)
            {
                characterController.enabled = false;
                playerBody.transform.rotation = Quaternion.Euler(0, 90, 0);
                characterController.enabled = true;
                right = true;
                left = false;
                shop = false;
            }
            //modif rotation corps
            if (playerSpeed.x < 0 && left == false)
            {

                characterController.enabled = false;
                playerBody.transform.rotation = Quaternion.Euler(0, -90, 0);
                characterController.enabled = true;
                left = true;
                right = false;
                shop = false;


            }
            //modif rotation corps
            if (verifVillage == false)
            {
                characterController.enabled = false;
                playerBody.transform.rotation = Quaternion.Euler(0, 0, 0);
                verifVillage = true;
            }

        }
        else return;


    }

    private void HandleInventoriesInFarmAndVillageV2()
    {
        if (SceneManager.GetActiveScene().name.Contains("Farm") || SceneManager.GetActiveScene().name.Contains("Village"))
        {
            playerInventory.HandleInventoryUI();
            pRInventory.HandleInventoryUI();
        }
    }

    #endregion

    #region Handle Save & Load of Player Position

    public void SavePlayerPos()
    {
        //SaveSystem.SavePlayerPosition(PlayerInput);
    }

    private void LoadPlayerPos()
    {
        Vector3 playerPosition = Vector3.zero;

        if (SceneManager.GetActiveScene().name.Contains("Farm"))
        {
            /*Player_Data data = SaveSystem.LoadPlayerInput(LS, PlayerInput);

            if (data != null || data.playerPosition == null) return;

            playerPosition = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);*/

            //Debug.Log($"Go to inFarm : {playerPosition}");
        }
        else if (SceneManager.GetActiveScene().name.Contains("Village"))
        {
            playerPosition = FindObjectOfType<ChangeScene>().transform.position;
            //playerPosition = new Vector3(data.villagePosition[0], data.villagePosition[1], data.villagePosition[2]);

            //Debug.Log($"Go to village : {playerPosition}");
        }

        characterController.enabled = false;
        transform.position = playerPosition; 
        characterController.enabled = true;
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            LoadPlayerPos();
        }
    }

    #endregion

    private void ForceSaveAndLoad()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (playerInput.QuickSaveAction.triggered)
            {
                SavePlayerPos();
                Debug.Log("Save");
            }

            if (playerInput.QuickLoadAction.triggered)
            {
                if (SceneManager.GetActiveScene().buildIndex != sceneVerif.sceneIndexSave)
                {
                    sceneVerif.LoadPlayerSc();
                }

                Debug.Log("Load");
            }
        }
    }
}