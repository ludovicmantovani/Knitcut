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
        if (other.CompareTag("Player")) changeScene.CanChangeScene = true;

        changeScene.InteractionPanel.SetActive(true);
        changeScene.ShowInstruction = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) changeScene.CanChangeScene = false;

        changeScene.InteractionPanel.SetActive(false);
        changeScene.ShowInstruction = false;
    }
}