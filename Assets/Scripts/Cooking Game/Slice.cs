using UnityEngine;

public class Slice : MonoBehaviour
{
    public GameObject cutObject;
    public float cutLifetime;

    private bool dragging;
    private Vector2 swipeStart;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragging = true;
            swipeStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && dragging)
        {
            SpawnCut();
        }
    }

    private void SpawnCut()
    {
        // Identify where the swipe end
        Vector2 swipeEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Spawned the cut object
        GameObject cut = Instantiate(cutObject, swipeStart, Quaternion.identity);
        cut.GetComponent<LineRenderer>().SetPosition(0, swipeStart);
        cut.GetComponent<LineRenderer>().SetPosition(1, swipeEnd);

        // Adjusted the edge collider
        Vector2[] colliderPoints = new Vector2[2];
        colliderPoints[0] = Vector2.zero;
        colliderPoints[1] = swipeEnd - swipeStart;
        cut.GetComponent<EdgeCollider2D>().points = colliderPoints;

        // Scheduled the destruction of the cut object
        Destroy(cut, cutLifetime);
    }
}