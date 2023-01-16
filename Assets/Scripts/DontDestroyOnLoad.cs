using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    [SerializeField] private string sceneToSave;
    [SerializeField] private List<Item> itemsToSave;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().name.Equals(sceneToSave))
        {
            for (int i = 0; i < itemsToSave.Count; i++)
            {
                Container container = FindObjectOfType<Container>();
                container.AddItemToInventory(itemsToSave[i], container.PlayerInventory);
            }

            Debug.Log($"All items added to player inventory");
            Destroy(gameObject);
        }
    }

    public void SaveItem(Item item)
    {
        itemsToSave.Add(item);
    }
}