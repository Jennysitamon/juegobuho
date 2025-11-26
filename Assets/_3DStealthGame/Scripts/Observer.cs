using UnityEngine;

public class Observer : MonoBehaviour
{
    public GameEnding gameEnding;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[Observer] Player detectado → ATRAPADO");
            gameEnding.CaughtPlayer();
        }
    }
}
