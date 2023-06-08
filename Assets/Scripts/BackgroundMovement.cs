using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackgroundMovement : MonoBehaviour
{
    [Header("Screen")]
    [SerializeField] private float scrollSpeed;

    private void Update()
    {
        MoveBackgrounds();
    }

    #region Background Movement

    private void MoveBackgrounds()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            MoveSprite(transform.GetChild(i).GetComponent<Renderer>());
        }
    }

    private void MoveSprite(Renderer renderer)
    {
        float x = Mathf.Repeat(Time.time * scrollSpeed, 1);

        Vector2 offset = new Vector2(x, 0);

        renderer.sharedMaterial.mainTextureOffset = offset;
    }

    #endregion
}