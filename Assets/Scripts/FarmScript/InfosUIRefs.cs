using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfosUIRefs : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image imageUI;
    [SerializeField] private TMP_Text nameUI;
    [SerializeField] private TMP_Text priceUI;
    [SerializeField] private TMP_InputField amountUI;
    [SerializeField] private Button amountButtonUp;
    [SerializeField] private Button amountButtonDown;
    [SerializeField] private Button operationUI;

    public Image ImageUI
    {
        get { return imageUI; }
        set { imageUI = value; }
    }

    public TMP_Text NameUI
    {
        get { return nameUI; }
        set { nameUI = value; }
    }

    public TMP_Text PriceUI
    {
        get { return priceUI; }
        set { priceUI = value; }
    }

    public TMP_InputField AmountUI
    {
        get { return amountUI; }
        set { amountUI = value; }
    }

    public Button AmountButtonUp
    {
        get { return amountButtonUp; }
        set { amountButtonUp = value; }
    }

    public Button AmountButtonDown
    {
        get { return amountButtonDown; }
        set { amountButtonDown = value; }
    }

    public Button OperationUI
    {
        get { return operationUI; }
        set { operationUI = value; }
    }
}