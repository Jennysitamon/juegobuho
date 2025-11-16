using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    [Header("Player target (se asigna solo si usas tags)")]
    public Transform player;

    private bool m_IsPlayerInRange;

    void OnTriggerEnter(Collider other)
    {
        // Detectar por TAG (lo más seguro)
        if (other.CompareTag("Player"))
        {
            m_IsPlayerInRange = true;

            // Si player no está asignado, lo asignamos automáticamente
            player = other.transform.root;

            Debug.Log("[Observer] Player detectado dentro del rango: " + player.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_IsPlayerInRange = false;
            Debug.Log("[Observer] Player salió del rango");
        }
    }

    void Update()
    {
        if (!m_IsPlayerInRange || player == null)
            return;

        Vector3 direction = player.position - transform.position + Vector3.up;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit))
        {
            // Asegura que detecte aunque golpee un hijo del player
            if (raycastHit.collider.transform.root == player)
            {
                Debug.Log("Player was caught!");
            }
        }
    }
}
