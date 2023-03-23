using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneVerification : MonoBehaviour
{
    PlayerInput playerInput;

    [SerializeField] private int sceneIndex;
    [SerializeField] private int sceneIndexSaved;
    [SerializeField] private float timeBeforeLoadingPlayer = 0.5f;

    public int SceneIndex
    {
        get { return sceneIndex; }
        set { sceneIndex = value; }
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        //ForceSaveAndLoadLastScene();
    }

    private void ForceSaveAndLoadLastScene()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (playerInput.QuickSaveAction.triggered)
            {
                SaveLastScene();
            }
            if (playerInput.QuickLoadAction.triggered)
            {
                LoadLastScene();
            }
        }
    }

    public void SaveLastScene()
    {
        //SaveSystem.SaveScene(this);
        SaveSystem.Save(SaveSystem.SaveType.Save_SceneVerification, this);
    }

    public void LoadLastScene()
    {
        StartCoroutine(LoadingLastScene());
    }

    private IEnumerator LoadingLastScene()
    {
        yield return new WaitForSeconds(timeBeforeLoadingPlayer);

        Player_Data data = (Player_Data)SaveSystem.Load(SaveSystem.SaveType.Save_SceneVerification, this);

        sceneIndexSaved = data.scene;

        if (sceneIndexSaved != sceneIndex) SceneManager.LoadScene(sceneIndexSaved);
    }
}