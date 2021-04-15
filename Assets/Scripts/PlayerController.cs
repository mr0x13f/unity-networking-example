using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] GameObject cameraHolder;

    [SerializeField] Item[] items;

    int itemIndex;
    int previousItemIndex = -1;

    [SerializeField] float
        mouseSensitivity = 3f * 0.022f,
        sprintSpeed = 6f,
        walkSpeed = 3f,
        jumpForce = 300f,
        smoothTime = 0.15f;

    private float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;

    PhotonView photonView;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int) photonView.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start() {
        if (photonView.IsMine) {
            EquipItem(0);
        } else {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
    }

    void Update() {

        if (!photonView.IsMine)
            return;

        Look();
        Move();
        Jump();

        for (int i = 0; i < items.Length; i++)
            if (Input.GetKeyDown((i + 1).ToString())) {
                EquipItem(i);
                break;
            }

        if (Input.GetMouseButtonDown(0))
            items[itemIndex].Use();

        if (transform.position.y < -10f)
            Die();

    }

    void FixedUpdate() {

        if (!photonView.IsMine)
            return;
            
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    void Jump() {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
            rb.AddForce(transform.up * jumpForce);
    }

    void Move() {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? walkSpeed : sprintSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Look() {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    public void SetGroundedState(bool grounded) {
        this.grounded = grounded;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!photonView.IsMine && targetPlayer == photonView.Owner) {
            EquipItem((int) changedProps["itemIndex"]);
        }
    }

    void EquipItem(int index) {
        if (index == previousItemIndex)
            return;

        itemIndex = index;

        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
            items[previousItemIndex].itemGameObject.SetActive(false);
    
        previousItemIndex = itemIndex;

        if (photonView.IsMine) {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public void TakeDamage(float damage) {
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage) {
        if (!photonView.IsMine)
            return;

        currentHealth -= damage;

        if (currentHealth < 0)
            Die();
    }

    void Die() {
        playerManager.Die();
    }
}
