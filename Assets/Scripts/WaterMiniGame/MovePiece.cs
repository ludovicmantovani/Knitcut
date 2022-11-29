using UnityEngine;

public class MovePiece : MonoBehaviour
{
    // verification if the switch happen
    public bool switchOn = false;
    private ProgressionLevelWaterGame PLW;

    [Header("Pieces")]
    [SerializeField] private Transform firstPieceToReplace;
    [SerializeField] private Transform secondPieceToReplace;

    Color defaultColorMat = Color.white;
    private void Start()
    {
        PLW = FindObjectOfType<ProgressionLevelWaterGame>();
    }

    void Update()
    {
        if(PLW.StopMoving == false)
        {
            DetectClick();
        }
       
    }

    private void DetectClick()
    {
        // If left click performed
        if (Input.GetMouseButtonDown(0))
        {
            // If ray (on mouse position) detected something
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                IdentifyObject(hit.transform);
            }
        }
    }

    private void IdentifyObject(Transform hitObject)
    {
        if (!hitObject.GetComponent<BlockPiece>()) return;

        // If no hitObject already selected, this hitObject is first
        if (firstPieceToReplace == null)
        {
            SelectPiece(hitObject, true);
        }
        // If there is already one hitObject selected, this hitObject is second
        else if(secondPieceToReplace == null)
        {
            SelectPiece(hitObject, false);

            // Two pieces are selected -> Switch positions
            SwitchPieces();
        }
    }

    private void SelectPiece(Transform piece, bool isFirstPiece)
    {
        // If no hitObject already selected, this hitObject is first
        if (isFirstPiece)
            firstPieceToReplace = piece;
        // If there is already one hitObject selected, this hitObject is second
        else
            secondPieceToReplace = piece;

        // If there is no default material color, save one
        if (defaultColorMat == Color.white)
            defaultColorMat = piece.Find("Outline").GetComponent<Renderer>().material.color;

        // Modify hitObject to show the selection
        ModifyPiece(piece, Color.green, new Vector3(0.9f, 0.9f, 0.9f));
    }

    private void ModifyPiece(Transform piece, Color color, Vector3 scale)
    {
        // Change color of the outline hitObject
        piece.Find("Outline").GetComponent<Renderer>().material.color = color;
        // Change scale to get a better view
        piece.localScale = scale;
    }

    private void SwitchPieces()
    {
        // Get position of the 2 pieces
        Vector3 firstPos = firstPieceToReplace.position;
        Vector3 secondPos = secondPieceToReplace.position;

        // Switch positions
        firstPieceToReplace.position = secondPos;
        secondPieceToReplace.position = firstPos;

        // switch on
        switchOn = true;

        // Reset pieces
        ModifyPiece(firstPieceToReplace, defaultColorMat, new Vector3(1, 1, 1));
        firstPieceToReplace = null;

        ModifyPiece(secondPieceToReplace, defaultColorMat, new Vector3(1, 1, 1));
        secondPieceToReplace = null;
    }
}