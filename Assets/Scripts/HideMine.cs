using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HideMine : MonoBehaviour
{
    [SerializeField] PhotonView photonView;

    void Start()
    {
        if (photonView.IsMine) {
            gameObject.layer = 8;
        }
    }
}
