using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "";
    [SerializeField] private GameObject interactionPanel;

    private PlayerInput pI;
    private Shop_Enclos se;
    private playerController PC;
    private List_Slots LS;

    bool canChangeScene = false;

    void Awake()
    {
        pI = GetComponent<PlayerInput>();
        se = FindObjectOfType<Shop_Enclos>();
        PC = FindObjectOfType<playerController>();
        LS = FindObjectOfType<List_Slots>();
    }

    void Update()
    {
        ShowInteraction();
        HandleChangeScene();
    }

    private void HandleChangeScene()
    {
        if (canChangeScene && sceneToLoad != "" && pI.actions["Intercation_Environnements"].triggered)
        {
            canChangeScene = false;

            if (SceneManager.GetActiveScene().name.Contains("Farm"))
            {
                FindObjectOfType<playerController>().SavePlayerPos();
            }

            if (SceneManager.GetActiveScene().name.Contains("Village"))
            {
                se.SaveEncloslevel();
                PC.farm = false;
            }

            LS.SaveData();

            MinigameManager.KeepPlayerInventory(LS);

            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void ShowInteraction()
    {
        if (canChangeScene && sceneToLoad != "")
        {
            if (SceneManager.GetActiveScene().name.Contains("Farm"))
            {
                interactionPanel.GetComponentInChildren<Text>().text = "Use " + pI.actions["Intercation_Environnements"].GetBindingDisplayString() + " to go to Village";
            }

            if (SceneManager.GetActiveScene().name.Contains("Village"))
            {
                interactionPanel.GetComponentInChildren<Text>().text = "Use " + pI.actions["Intercation_Environnements"].GetBindingDisplayString() + " to return to Farm";
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) canChangeScene = true;

        interactionPanel.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) canChangeScene = false;

        interactionPanel.SetActive(false);
    }
}