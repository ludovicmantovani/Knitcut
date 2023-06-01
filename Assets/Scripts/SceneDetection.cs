using UnityEngine;

public class SceneDetection : MonoBehaviour
{
    private ChangeScene changeScene;

    private void Start()
    {
        changeScene = GetComponentInParent<ChangeScene>();
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleSceneDetection(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleSceneDetection(other, false);
    }

    private void HandleSceneDetection(Collider other, bool state)
    {
        if (other.CompareTag("Player"))
        {
            changeScene.CanChangeScene = state;

            changeScene.InteractionPanel.SetActive(state);
            changeScene.ShowInstruction = state;
        }
    }
}