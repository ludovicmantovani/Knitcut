using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RelationCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text[] tMP_Texts;

    private int _currentIndex = 0;

    public void Next()
    {
        if (_currentIndex + 1 < tMP_Texts.Length)
        {
            tMP_Texts[_currentIndex].alpha = 0.5f;
            _currentIndex++;
            tMP_Texts[_currentIndex].alpha = 1f;
        }
    }

    public int GetTextCount()
    {
        return tMP_Texts.Length;
    }
}
