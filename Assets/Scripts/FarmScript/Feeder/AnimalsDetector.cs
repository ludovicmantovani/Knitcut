using UnityEngine;

public class AnimalsDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        HandleAnimalDetection(other.gameObject, true);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleAnimalDetection(other.gameObject, false);
    }

    private void HandleAnimalDetection(GameObject animal, bool add)
    {
        if (animal.CompareTag("Animal"))
        {
            if (!GetComponentInParent<Feeder>().AnimalsToFeed.Contains(animal))
            {
                if (add)
                {
                    GetComponentInParent<Feeder>().AnimalsToFeed.Add(animal);
                }
                else
                {
                    GetComponentInParent<Feeder>().AnimalsToFeed.Remove(animal);
                }
            }
        }
    }
}