using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum PickupType
{
    Roach,
    Health
}

public class Pickup : MonoBehaviourPun
{
    public PickupType type;
    public int value;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>
            ();
            if (type == PickupType.Roach)
                player.photonView.RPC("GiveRoach", player.photonPlayer, value);
            else if (type == PickupType.Health)
                player.photonView.RPC("Heal", player.photonPlayer, value);
            PhotonNetwork.Destroy(gameObject);
        }
    }

}
