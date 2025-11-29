using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    public string KeyName;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        if (player == null) return;

        if (player.OwnKey(KeyName))
        {
            Debug.Log("[Door] Puerta abierta con llave: " + KeyName);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("[Door] Necesitas la llave: " + KeyName);
        }
    }
}

