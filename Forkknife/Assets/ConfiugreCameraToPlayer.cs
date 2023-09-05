using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class ConfigureCameraToPlayer : NetworkBehaviour
{
    private CinemachineVirtualCamera vcam;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner) // Only if this instance belongs to the local player.
        {
            // Find the Cinemachine camera in the scene (assuming you only have one).
            vcam = FindObjectOfType<CinemachineVirtualCamera>();

            if (vcam != null)
            {
                vcam.Follow = transform; // Set the camera to follow this player.

                // Optional: enable the camera if it's disabled.
                vcam.enabled = true;
            }
            else
            {
                Debug.LogError("No CinemachineVirtualCamera found in the scene.");
            }
        }
    }
}
