using UnityEngine;

public class BlockPiece : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            gameObject.layer =  LayerMask.NameToLayer("Ignore Raycast");
        }
    }
}