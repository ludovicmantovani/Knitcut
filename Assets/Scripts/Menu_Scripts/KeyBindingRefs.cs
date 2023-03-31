using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindingRefs : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string inputActionName;
    [SerializeField] private int inputActionBindingIndex;
    [SerializeField] private TMP_Text actionName;
    [SerializeField] private Button triggerRebindButton;
    [SerializeField] private Button resetDefaultButton;

    public string InputActionName
    {
        get { return inputActionName; }
        set { inputActionName = value; }
    }

    public int InputActionBindingIndex
    {
        get { return inputActionBindingIndex; }
        set { inputActionBindingIndex = value; }
    }

    public TMP_Text ActionName
    {
        get { return actionName; }
        set { actionName = value; }
    }

    public Button TriggerRebindButton
    {
        get { return triggerRebindButton; }
        set { triggerRebindButton = value; }
    }

    public Button ResetDefaultButton
    {
        get { return resetDefaultButton; }
        set { resetDefaultButton = value; }
    }
}