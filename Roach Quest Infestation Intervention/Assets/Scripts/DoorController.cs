using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DoorController : MonoBehaviourPun
{


    [PunRPC]
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<PlayerController>().roach >= 25)
            {

            }
        }
    }
}
