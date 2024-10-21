using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotateUI : MonoBehaviour
{
    void Update()
    {
        this.transform.Rotate(Vector3.forward * Time.deltaTime * 500);
    }
}
