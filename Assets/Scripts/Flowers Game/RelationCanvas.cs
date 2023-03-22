using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RelationCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text[] tMP_Texts;
    [SerializeField] private float visibleTextAlpha = 1f;
    [SerializeField] private float hideTextAlpha = 0.5f;

    private int _currentIndex = 0;

    public void Next()
    {
        if (_currentIndex + 1 < tMP_Texts.Length)
        {
            tMP_Texts[_currentIndex].alpha = hideTextAlpha;
            _currentIndex++;
            tMP_Texts[_currentIndex].alpha = visibleTextAlpha;
        }
    }

    public void Previous()
    {
        if (_currentIndex - 1 >=0)
        {
            tMP_Texts[_currentIndex].alpha = hideTextAlpha;
            _currentIndex--;
            tMP_Texts[_currentIndex].alpha = visibleTextAlpha;
        }
    }

    public int GetTextCount()
    {
        return tMP_Texts.Length;
    }

    public void Reset()
    {
        foreach (TMP_Text item in tMP_Texts) { item.alpha = hideTextAlpha;}
        if (tMP_Texts.Length > 0)
            tMP_Texts[0].alpha = visibleTextAlpha;
    }
}
