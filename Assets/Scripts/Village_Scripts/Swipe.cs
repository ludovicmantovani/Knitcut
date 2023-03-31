using UnityEngine;
using UnityEngine.UI;

public class Swipe : MonoBehaviour
{
    [SerializeField] private GameObject scrollbar;

    private float scrollPos = 0;
    private float[] pos;

    public float ScrollPos
    {
        set { scrollPos = value; }
        get { return scrollPos; }
    }

    private void Update()
    {
        SwipePos();
    }

    private void SwipePos()
    {
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);

        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            scrollPos = scrollbar.GetComponent<Scrollbar>().value;
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2))
                {
                    scrollbar.GetComponent<Scrollbar>().value = pos[i];
                }
            }
        }
    }
}