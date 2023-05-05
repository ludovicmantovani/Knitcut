using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Parameters

    PlayerInventory playerInventory;
    PlayerRecipesInventory playerRecipesInventory;
    List_Slots listSlots;

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

    public PlayerRecipesInventory PlayerRecipesInventory
    {
        get { return playerRecipesInventory; }
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

    private Vector3 move;
    private float verticalVelocity;
    private float gravity = 9.81f;
    private float velocity = 0f;
    private float currentSpeed;

    private bool canMove = true;

    public float CurrentSpeed
    {
        get { return currentSpeed; }
        set { currentSpeed = value; }
    }

    public bool CanMove
    { 
        get { return canMove; }
        set { canMove = value; }
    }

    public List_Slots ListSlots
    {
        get { return listSlots; }
        set { listSlots = value; }
    }

    #endregion

    private void Awake()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerRecipesInventory = FindObjectOfType<PlayerRecipesInventory>();
        listSlots = FindObjectOfType<List_Slots>();

        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        sceneVerif = GetComponent<SceneVerification>();
    }

    private void Update()
    {
        HandleInventoriesInFarmAndVillage();

        HandlePlayerInFarmScene();
        HandlePlayerInVillageScene();

        DeleteAllSaves();
    }

    private void DeleteAllSaves()
    {
        if (playerInput.DeleteSavesAction.triggered)
        {
            SaveSystem.DeleteAllSaves();
        }
    }

    #region Handle Player in Farm Scene & Village Scene

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

        // Gravity

        bool groundedPlayer = characterController.isGrounded;

        if (groundedPlayer && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }

        verticalVelocity -= gravity * Time.deltaTime;

        // Movement

        Vector2 input = playerInput.MoveAction.ReadValue<Vector2>();

        currentSpeed = Mathf.Abs(input.x) + Mathf.Abs(input.y);

        move.Set(input.x, 0f, input.y);

        move = move.x * cameraFerme.transform.right + move.z * cameraFerme.transform.forward;

        if (input != Vector2.zero)
        {
            _targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cameraFerme.transform.eulerAngles.y;

            _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0, _angle, 0);

            move = Quaternion.Euler(0, _targetAngle, 0) * Vector3.forward;
        }

        move.y = verticalVelocity;

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

        // Movement

        Vector2 input = playerInput.MoveAction.ReadValue<Vector2>();

        currentSpeed = Mathf.Abs(input.x);

        move.Set(input.x, 0f, 0f);

        characterController.Move(move * Time.deltaTime * moveSpeed);

        if (input.x == -1) playerBody.transform.rotation = Quaternion.LookRotation(-transform.right);
        if (input.x == 1) playerBody.transform.rotation = Quaternion.LookRotation(transform.right);        
    }

    private void HandleInventoriesInFarmAndVillage()
    {
        if (SceneManager.GetActiveScene().name.Contains("Farm") || SceneManager.GetActiveScene().name.Contains("Village"))
        {
            playerInventory.HandleInventoryUI();
            playerRecipesInventory.HandleInventoryUI();
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
}
