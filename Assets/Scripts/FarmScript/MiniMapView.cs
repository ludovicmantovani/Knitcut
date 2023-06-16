using System.Collections.Generic;
using UnityEngine;

public class MiniMapView : MonoBehaviour
{
    [System.Serializable]
    public class SpriteTransformPair
    {
        public string objectName;
        public Transform objectTransform;
        public Sprite objectSprite;
    }

    [SerializeField] private RenderTexture renderTexture = null;
    [SerializeField] private string layer;
    [SerializeField] private float spriteScale = 6f;
    [SerializeField] private SpriteTransformPair playerPosition;
    [SerializeField] private List<SpriteTransformPair> objectsToShow = null;

    private Camera _camera = null;
    private AnimalPenManager animalPenManager;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        animalPenManager = FindObjectOfType<AnimalPenManager>();

        MakeMiniMap();
    }

    public void MakeMiniMap()
    {
        if (_camera && renderTexture != null)
        {
            _camera.orthographic = true;
            _camera.targetTexture = renderTexture;
        }

        HandlePicto(playerPosition, false);

        foreach (SpriteTransformPair spriteTransformPair in objectsToShow)
        {
            spriteTransformPair.objectTransform = animalPenManager.GetAnimalPenWithPicto(spriteTransformPair.objectName);

            HandlePicto(spriteTransformPair, true);
        }
    }

    private void HandlePicto(SpriteTransformPair spriteTransformPair, bool customScale)
    {
        if (spriteTransformPair.objectTransform == null) return;

        GameObject go = new GameObject(spriteTransformPair.objectName, typeof(SpriteRenderer));

        if (layer.Length > 0 && LayerMask.NameToLayer(layer) != -1) go.layer = LayerMask.NameToLayer(layer);

        Renderer objectRenderer = null;
        spriteTransformPair.objectTransform.gameObject.TryGetComponent<Renderer>(out objectRenderer);

        if (objectRenderer != null)
        {
            Vector3 centerPosition = objectRenderer.bounds.center;
            go.transform.position = new Vector3(
                centerPosition.x,
                gameObject.transform.position.y - 10f,
                centerPosition.z);

        }
        else
        {
            go.transform.position = new Vector3(
                spriteTransformPair.objectTransform.position.x,
                gameObject.transform.position.y - 10f,
                spriteTransformPair.objectTransform.position.z);
        }

        go.transform.rotation = Quaternion.AngleAxis(-90f, Vector3.right);

        if (customScale) go.transform.localScale = spriteTransformPair.objectTransform.localScale * spriteScale;

        go.GetComponent<SpriteRenderer>().sprite = spriteTransformPair.objectSprite;
    }
}