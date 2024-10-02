using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NetPlayer : NetworkBehaviour
{
    [Header("MetaNetworkManager")]
    public MetaNetworkManager MetaNetworkManager;

    [Header("Components")]
    public NavMeshAgent NavAgent_Player;
    public Animator Animator_Player;

    [Header("Movement")]
    public float _rotationSpeed = 100.0f;

    [Header("Interaction")]
    public KeyCode _atkKey = KeyCode.Space;

    [SerializeField] Rigidbody RigidBody_Player;
    [SerializeField] Camera Camera_Player;

    [Header("ChatMesh")]
    [SerializeField] BillBoardChatMesh GObj_ChatMesh;
    [SerializeField] TextMesh TextMesh_Chat;

    private float _moveSpeed = 5.0f;
    private float _mouseSensitivity = 100.0f;
    private float _cameraRotationX = 0.0f;

    private void Start()
    {
        GObj_ChatMesh.gameObject.SetActive(false);
        Camera_Player.gameObject.SetActive(false);

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        StartCoroutine(CoDelayBindRpc());
    }

    private IEnumerator CoDelayBindRpc()
    {
        yield return new WaitForSeconds(1.0f);

        var gObj = GameObject.Find("MetaNetworkManager");
        if(gObj != null)
        {
            var metaManager = gObj.GetComponent<MetaNetworkManager>();
            if(metaManager != null)
            {
                MetaNetworkManager = metaManager;
                if (this.isLocalPlayer)
                {
                    Camera_Player.name = "LocalPlayerCamera";
                    MetaNetworkManager.BindLocalPlayerNetId(this.netId);
                    Camera_Player.gameObject.SetActive(true);
                }
                
                MetaNetworkManager.BindRpcAnimStateChangedCallback(OnRpcAnimStateChanged);
                MetaNetworkManager.BindRecvMsgCallback(OnRecvChatMsg);
            }
        }
    }

    private void OnRpcAnimStateChanged(uint netId, string animStateKey, bool isActive)
    {
        if(netId == this.netId)
        {
            Animator_Player.SetBool(animStateKey, isActive);
        }
    }

    private void OnRecvChatMsg(uint netId, string msg)
    {
        if (this.isLocalPlayer)
        {
            return;
        }

        if(netId != this.netId)
        {
            return;
        }

        GObj_ChatMesh.ShowChatMsg(msg);
    }

    private void Update()
    {
        if(CheckIsFocusedOnUpdate() == false)
        {
            return;
        }

        MoveOnUpdate();
    }

    private bool CheckIsFocusedOnUpdate()
    {
        return Application.isFocused;
    }

    private bool IsAnimStateChanged(string animState, bool targetState)
    {
        var curState = Animator_Player.GetBool(animState);
        return (curState != targetState);
    }

    private void MoveOnUpdate()
    {
        if (isLocalPlayer == false)
            return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        string curStateName = "Run";
        bool isRunning = movement.magnitude > 0;
        if(IsAnimStateChanged(curStateName, isRunning))
        {
            MetaNetworkManager?.RequestChangeAnimState("InteractLoop", false);
            MetaNetworkManager?.RequestChangeAnimState(curStateName, isRunning);
        }

        // Rotate Camera
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        _cameraRotationX -= mouseY;
        _cameraRotationX = Mathf.Clamp(_cameraRotationX, -90f, 90f);

        Camera_Player.transform.localRotation = Quaternion.Euler(_cameraRotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Root Anim - True
        // this.transform.Translate(Vector3.right * (moveHorizontal * _moveSpeed * Time.deltaTime));
        // this.transform.Translate(Vector3.forward * (moveVertical * _moveSpeed * Time.deltaTime));
    }

    [Command]
    void RequestCommandInteractionOnServer()
    {
        //GameObject atkObjectForSpawn = Instantiate(Prefab_AtkObject, Tranfrom_AtkSpawnPos.transform.position, Tranfrom_AtkSpawnPos.transform.rotation);
        //NetworkServer.Spawn(atkObjectForSpawn);

        //RpcOnInteract("Atk");
    }

    [ClientRpc]
    void RpcOnInteract(string animStateKey)
    {
        Animator_Player.SetTrigger(animStateKey);
    }

    void RotateLocalPlayer()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out RaycastHit hit, 100))
        //{
        //    Debug.DrawLine(ray.origin, hit.point);
        //    Vector3 lookRotate = new Vector3(hit.point.x, Transform_Player.position.y, hit.point.z);
        //    Transform_Player.LookAt(lookRotate);
        //}
    }
}
