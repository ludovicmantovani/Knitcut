using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject inventoryKeyUI;
    [SerializeField] private GameObject recipesInventoryKeyUI;
    [SerializeField] private GameObject cursorKeyUI;
    [SerializeField] private GameObject returnToMenuButton;

    [Header("References")]
    [SerializeField] private string farmSceneName = "FarmScene";
    [SerializeField] private string villageSceneName = "TradingVillage";
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform spawn;
    [SerializeField] private int money = 0;
    [SerializeField] private float timeTeleport = 1f;

    private CharacterController characterController;
    private SceneVerification sceneVerif;
    private PlayerInput playerInput;
    private PlayerInventory playerInventory;
    private PlayerRecipesInventory playerRecipesInventory;
    private ListSlots listSlots;

    [Header("References Scene Handlers")]
    [SerializeField] private GameObject farmHandler;
    [SerializeField] private GameObject villageHandler;
    [SerializeField] private GameObject cameraFerme;

    [Header("Datas")]
    [SerializeField] private float turnSmoothTime;
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool talkingShop = false;
    [SerializeField] private bool inFarm = false;

    [Header("Camera Datas")]
    [SerializeField] private CinemachineFreeLook freeLook;
    [SerializeField] private float cameraSensibilityX;
    [SerializeField] private float cameraSensibilityY;

    private float turnSmoothVelocity;

    private float _targetAngle;
    private float _angle;

    private Vector3 move;
    private float verticalVelocity;
    private float gravity = 9.81f;
    private float currentSpeed;

    private bool canMove = true;
    private bool teleportToSpawn = false;

    #region Getters / Setters

    public GameObject ReturnToMenuButton
    {
        get { return returnToMenuButton; }
        set { returnToMenuButton = value; }
    }

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

    public CinemachineBrain CameraCineBrain
    {
        get { return cameraFerme.GetComponent<CinemachineBrain>(); }
    }

    public CinemachineFreeLook FreeLook
    {
        get { return freeLook; }
        set { freeLook = value; }
    }

    public float CameraSensibilityX
    {
        get { return cameraSensibilityX; }
        set { cameraSensibilityX = value; }
    }

    public float CameraSensibilityY
    {
        get { return cameraSensibilityY; }
        set { cameraSensibilityY = value; }
    }

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

    public PlayerInput PlayerInput
    {
        get { return playerInput; }
    }

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

    public ListSlots ListSlots
    {
        get { return listSlots; }
        set { listSlots = value; }
    }

    #endregion

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        sceneVerif = GetComponent<SceneVerification>();

        playerInventory = FindObjectOfType<PlayerInventory>();
        playerRecipesInventory = FindObjectOfType<PlayerRecipesInventory>();
        listSlots = FindObjectOfType<ListSlots>();
    }

    private void Start()
    {
        LoadCameraSensibility();

        LoadKeysUI();
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

    private void LoadKeysUI()
    {
        inventoryKeyUI.GetComponentInChildren<Text>().text = playerInput.InventoryAction.GetBindingDisplayString();
        recipesInventoryKeyUI.GetComponentInChildren<Text>().text = playerInput.RecipesInventoryAction.GetBindingDisplayString();
        cursorKeyUI.GetComponentInChildren<Text>().text = $"L Ctrl";
        //cursorKeyUI.GetComponentInChildren<Text>().text = KeyCode.LeftControl.ToString();
    }

    #region Camera Sensibility

    public void UpdateCameraSensibility()
    {
        freeLook.m_XAxis.m_MaxSpeed = cameraSensibilityX;
        freeLook.m_YAxis.m_MaxSpeed = cameraSensibilityY;
    }

    private void LoadCameraSensibility()
    {
        Audio_Data data = (Audio_Data)SaveSystem.Load(SaveSystem.SaveType.Save_Volume);

        if (data == null) return;

        cameraSensibilityX = data.cameraSensibilityX;
        cameraSensibilityY = data.cameraSensibilityY;
    }

    #endregion

    #region Handle Player in Farm Scene & Village Scene

    private void HandlePlayerInFarmScene()
    {
        if (SceneManager.GetActiveScene().name.Contains(farmSceneName))
        {
            farmHandler.SetActive(true);
            villageHandler.SetActive(false);
            
            if (teleportToSpawn)
                StartCoroutine(WaitTeleport());
            else
                PlayerMovementFarm();

            UpdateCameraSensibility();
        }
    }

    private IEnumerator WaitTeleport()
    {
        listSlots.SaveData();

        transform.position = spawn.position;
        
        yield return new WaitForSeconds(timeTeleport);

        teleportToSpawn = false;
        canMove = true;
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

    public void HandlePlayerMovement(bool state)
    {
        //if (!state) currentSpeed = 0;

        canMove = state;
        CameraCineBrain.enabled = state;
    }

    #endregion

    #region Handle Save & Load of Player Position

    public void TeleportPlayer()
    {
        teleportToSpawn = true;
        canMove = false;
    }

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
        SavePlayerPositionInScene();
    }

    #endregion
}
