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
    [SerializeField] GameObject Prefab_SpeechBallonSlotUI;

    [Header("SpawnFieldObj")]
    [SerializeField] Transform Transform_SpawnObj;

    private float _moveSpeed = 5.0f;
    private float _mouseSensitivity = 100.0f;
    private float _cameraRotationX = 0.0f;
    private Transform _chatRoot;

    public Transform GetSpawnObjTransform() { return Transform_SpawnObj; }

    private void Start()
    {
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
                    MetaNetworkManager.BindLocalPlayer(this);
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
            if(IsAnimStateChanged(animStateKey, isActive))
            {
                Animator_Player.SetBool(animStateKey, isActive);
            }
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

        var gObj = GameObject.Find("ChatSlotUIRoot");
        if(gObj != null)
        {
            _chatRoot = gObj.transform;
            var slotUI = Instantiate(Prefab_SpeechBallonSlotUI, _chatRoot);
            if(slotUI != null)
            {
                var speechBallonUI = slotUI.GetComponent<SpeechBallonSlotUI>();
                if(speechBallonUI != null)
                {
                    speechBallonUI.SetSpeechText(this.transform, msg);
                }
            }
        }
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

    private void CheckAndCancelAnimState(string animState)
    {
        if (Animator_Player.GetBool(animState))
        {
            MetaNetworkManager?.RequestChangeAnimState(animState, false);
        }
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
            CheckAndCancelAnimState("Sit");
            CheckAndCancelAnimState("InteractLoop");
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

}
