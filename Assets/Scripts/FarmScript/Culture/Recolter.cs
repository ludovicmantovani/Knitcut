using UnityEngine;

public class Recolter : MonoBehaviour
{
    public PlayerInput playerInput;
    Planter planter;
    Pousse pousse;
    // Start is called before the first frame update
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();

        planter = GetComponentInParent<Planter>();
        pousse = GetComponentInParent<Pousse>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player_Farm" && playerInput.InteractionAction.triggered)
        {
            planter.Vide = false;
            Destroy(pousse.GraineX);
        }
    }
}