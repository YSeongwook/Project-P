using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NetPlayer : NetworkBehaviour
{
    [Header("Components")]
    public NavMeshAgent NavAgent_Player;
    public Animator Animator_Player;

    [Header("Movement")]
    public float _rotationSpeed = 100.0f;

    [Header("Interaction")]
    public KeyCode _atkKey = KeyCode.Space;

    [SerializeField] Rigidbody RigidBody_Player;
    [SerializeField] Camera Camera_Player;

    private float _moveSpeed = 5.0f;
    private float _mouseSensitivity = 100.0f;
    private float _cameraRotationX = 0.0f;

    private void Start()
    {
        if (isLocalPlayer == false)
        {
            Camera_Player.gameObject.SetActive(false);
            return;
        }

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
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

    private void MoveOnUpdate()
    {
        if (isLocalPlayer == false)
            return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        bool isRunning = movement.magnitude > 0;
        RequestCommandAnimChange("Run", isRunning);

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
    void RequestCommandAnimChange(string animKey, bool isStart)
    {
        RpcOnAnimChange(animKey, isStart);
    }

    [ClientRpc]
    void RpcOnAnimChange(string animStateKey, bool isStart)
    {
        Animator_Player.SetBool(animStateKey, isStart);
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
