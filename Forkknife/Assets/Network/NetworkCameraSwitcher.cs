using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class NetworkCameraSwitcher : NetworkBehaviour
{
    private CinemachineVirtualCamera vcam;

    void Start()
    {
        if (!IsOwner) return;

        vcam = GetComponent<CinemachineVirtualCamera>();

        if (vcam != null)
        {
            if (vcam.Priority <= 10)
            {
                vcam.Priority = 11;
            }
        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera component not found on this object!");
        }
    }
}
