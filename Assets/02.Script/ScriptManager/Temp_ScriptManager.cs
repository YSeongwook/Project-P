using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_ScriptManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> ScriptList;

    private void Start()
    {
        foreach (var script in ScriptList)
        {
            script.SetActive(true);
        }
    }
}
