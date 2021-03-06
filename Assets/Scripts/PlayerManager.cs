using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView photonView;
    GameObject controller;

    void Awake() {
        photonView = GetComponent<PhotonView>();
    }

    void Start() {
        if (photonView.IsMine)
            CreateController();
    }

    void CreateController() {
        Transform spawnpoint = SpawnManager.instance.GetSpawnpoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnpoint.position, spawnpoint.rotation, 0,
            new object[] { photonView.ViewID });
    }

    public void Die() {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
