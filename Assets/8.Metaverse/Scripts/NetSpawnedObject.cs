using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSpawnedObject : NetworkBehaviour
{
    public string Str_ChangeTargetAnimState;

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        var remotePlayer = other.GetComponent<NetworkBehaviour>();
        if(remotePlayer == null)
        {
            remotePlayer = other.gameObject.GetComponentInParent<NetworkBehaviour>();
            if(remotePlayer == null)
            {
                return;
            }
        }

        var remoteNetId = remotePlayer.netId;

        var gObj = GameObject.Find("MetaNetworkManager");
        if (gObj != null)
        {
            var metaManager = gObj.GetComponent<MetaNetworkManager>();
            if (metaManager != null)
            {
                metaManager.RequestChangeAnimStateByRemoteId(remoteNetId, Str_ChangeTargetAnimState, true);
            }
        }
    }
}
