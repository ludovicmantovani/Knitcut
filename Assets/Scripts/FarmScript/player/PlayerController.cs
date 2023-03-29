using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Parameters

    List_Slots LS;
    PlayerInventory playerInventory;
    PlayerRecipesInventory pRInventory;

    [Header("References")]
    [SerializeField] private string farmSceneName = "FarmScene";
    [SerializeField] private string villageSceneName = "TradingVillage";
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform spawn;
    [SerializeField] private int money = 0;

    public PlayerInventory PlayerInventory
    {
        get { return playerInventory; }
    }

    public int Money
    {
        get { return money; }
        set { money = value; }
    }

    [Header("References Scene Handlers")]
    [SerializeField] private GameObject farmHandler;
    [SerializeField] private GameObject villageHandler;
    [SerializeField] private GameObject cameraFerme;

    public CinemachineBrain CameraCineBrain
    {
        get { return cameraFerme.GetComponent<CinemachineBrain>(); }
    }

    [Header("Datas")]
    [SerializeField] private float turnSmoothTime;
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool talkingShop = false;
    [SerializeField] private bool inFarm = false;

    private float turnSmoothVelocity;

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
    private SceneVerification sceneVerif;
    private PlayerInput playerInput;

    public PlayerInput PlayerInput
    {
        get { return playerInput; }
    }

    private float _targetAngle;
    private float _angle;

    private bool canMove = true;

    public bool CanMove
    { 
        get { return canMove; }
        set { canMove = value; }
    }

    #endregion

    private void Awake()
    {
        LS = FindObjectOfType<List_Slots>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        pRInventory = FindObjectOfType<PlayerRecipesInventory>();

        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        sceneVerif = GetComponent<SceneVerification>();
    }

    private void Start()
    {
        if (!MinigameManager.StartOK)
        {
            MinigameManager.StartOK = true;

            Debug.Log($"Initialization OK");
            LoadPlayerPositionInScene();
        }
    }

    private void Update()
    {
        HandleInventoriesInFarmAndVillage();

        HandlePlayerInFarmScene();
        HandlePlayerInVillageScene();

        //ForceSaveAndLoad();
    }

    #region Handle Player in Farm Scene & Village Scene V1

    /*private void HandlePlayerInFarmSceneV1()
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

            PlayerMovementFarmV1();
        }
    }

    private void HandlePlayerInVillageSceneV1()
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

            PlayerMovementVillageV1();
        }
    }

    private void PlayerMovementFarmV1()
    {
        if (!canMove) return;

        Vector2 input = playerInput.MoveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);

        move = move.x * cam.transform.right + move.z * cam.transform.forward;
        move.y = 0f;

        characterController.Move(move * Time.deltaTime * moveSpeed);

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

    private void PlayerMovementVillageV1()
    {
        if (!canMove) return;

        if (talkingShop == true && shopsConfiguration == false)
        {

            characterController.enabled = false;
            characterController.transform.rotation = Quaternion.Euler(0, 0, 0);
            characterController.enabled = true;
            left = false;
            right = false;
            shopsConfiguration = true;
        }

        //modif
        if (talkingShop == false)
        {
            //nouveau system
            Vector2 input = playerInput.MoveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0, 0);
            move = move.x * cam.transform.right + move.z * cam.transform.forward;
            move.y = 0f;
            characterController.Move(move * Time.deltaTime * moveSpeed);
            playerSpeed = move;

            //modif rotation corps
            if (playerSpeed.x > 0 && right == false)
            {
                characterController.enabled = false;
                playerBody.transform.rotation = Quaternion.Euler(0, 90, 0);
                characterController.enabled = true;
                right = true;
                left = false;
                shopsConfiguration = false;
            }
            //modif rotation corps
            if (playerSpeed.x < 0 && left == false)
            {

                characterController.enabled = false;
                playerBody.transform.rotation = Quaternion.Euler(0, -90, 0);
                characterController.enabled = true;
                left = true;
                right = false;
                shopsConfiguration = false;


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

    private void HandleInventoriesInFarmAndVillageV1()
    {
        if (SceneManager.GetActiveScene().name.Contains("Farm") || SceneManager.GetActiveScene().name.Contains("Village"))
        {
            playerInventory.HandleInventoryUI();
            pRInventory.HandleInventoryUI();
        }
    }*/

    #endregion

    #region Handle Player in Farm Scene & Village Scene V2

    private void HandlePlayerInFarmScene()
    {
        if (SceneManager.GetActiveScene().name.Contains(farmSceneName))
        {
            farmHandler.SetActive(true);
            villageHandler.SetActive(false);

            PlayerMovementFarm();
        }
    }

    private void PlayerMovementFarm()
    {
        if (!canMove) return;

        Vector2 input = playerInput.MoveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);

        move = move.x * cameraFerme.transform.right + move.z * cameraFerme.transform.forward;
        move.y = 0f;

        if (input != Vector2.zero)
        {
            _targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cameraFerme.transform.eulerAngles.y;

            _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0, _angle, 0);

            move = Quaternion.Euler(0, _targetAngle, 0) * Vector3.forward;
        }

        characterController.Move(move * Time.deltaTime * moveSpeed);
    }

    private void HandlePlayerInVillageScene()
    {
        if (SceneManager.GetActiveScene().name.Contains(villageSceneName))
        {
            farmHandler.SetActive(false);
            villageHandler.SetActive(true);

            PlayerMovementVillage();
        }
    }

    private void PlayerMovementVillage()
    {
        if (!canMove) return;

        Vector2 input = playerInput.MoveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, 0);

        move.y = 0f;
        move.z = 0f;

        characterController.Move(move * Time.deltaTime * moveSpeed);

        if (input.x == -1) playerBody.transform.rotation = Quaternion.LookRotation(-transform.right);
        if (input.x == 1) playerBody.transform.rotation = Quaternion.LookRotation(transform.right);        
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

    #region Handle Save & Load of Player Position

    private void SavePlayerPosition()
    {
        SaveSystem.Save(SaveSystem.SaveType.Save_PlayerInput, playerInput);
    }

    private void LoadPlayerPosition()
    {
        Vector3 playerPosition = Vector3.zero;

        if (SceneManager.GetActiveScene().name.Contains("Farm"))
        {
            Player_Data data = (Player_Data)SaveSystem.Load(SaveSystem.SaveType.Save_PlayerInput, playerInput);

            if (data == null) return;

            if (data.playerPosition == null) return;

            playerPosition = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
        }
        else if (SceneManager.GetActiveScene().name.Contains("Village"))
        {
            playerPosition = spawn.position;
        }

        transform.position = playerPosition;
    }

    public void SavePlayerPositionInScene()
    {
        sceneVerif.SaveLastScene();
        SavePlayerPosition();
    }

    public void LoadPlayerPositionInScene()
    {
        sceneVerif.LoadLastScene();
        LoadPlayerPosition();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Quit");
        SavePlayerPositionInScene();
    }

    #endregion

    /*private void ForceSaveAndLoad()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (playerInput.QuickSaveAction.triggered)
            {
                Debug.Log("Save");
                SavePlayerPosition();
            }

            if (playerInput.QuickLoadAction.triggered)
            {
                if (SceneManager.GetActiveScene().buildIndex != sceneVerif.sceneIndexSaved)
                {
                    sceneVerif.LoadLastScene();
                }

                Debug.Log("Load");
                LoadPlayerPosition();
            }
        }
    }*/
}
