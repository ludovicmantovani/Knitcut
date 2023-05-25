using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CookingResultCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text textPrice = null;
    [SerializeField] private TMP_Text textScore = null;
    [SerializeField] private TMP_Text textInfo = null;
    [SerializeField] private RawImage imageDish = null;

    public void SetData(string finalPrice = "", string score = "", string info = "", Sprite dishSprite = null)
    {
        if (textPrice) textPrice.text = finalPrice;
        if (textScore) textScore.text = score;
        if (textInfo) textInfo.text = info;
        if (imageDish != null && dishSprite != null) imageDish.texture = dishSprite.texture;
    }
}
