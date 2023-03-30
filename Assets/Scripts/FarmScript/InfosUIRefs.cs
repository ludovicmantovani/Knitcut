using UnityEngine;
using UnityEngine.UI;

public class InfosUIRefs : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image imageUI;
    [SerializeField] private Text nameUI;
    [SerializeField] private Text priceUI;
    [SerializeField] private InputField amountUI;
    [SerializeField] private Button amountButtonUp;
    [SerializeField] private Button amountButtonDown;
    [SerializeField] private Button operationUI;

    public Image ImageUI
    {
        get { return imageUI; }
        set { imageUI = value; }
    }

    public Text NameUI
    {
        get { return nameUI; }
        set { nameUI = value; }
    }

    public Text PriceUI
    {
        get { return priceUI; }
        set { priceUI = value; }
    }

    public InputField AmountUI
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