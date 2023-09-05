using Unity.Netcode;
using UnityEngine;

public class IKControl : NetworkBehaviour
{
    public Transform rightArmTarget;
    public Transform leftArmTarget;

    private Vector3 recoilOffset = Vector3.zero;
    private float recoilSpeed = 10f;

    void Update()
    {
        if (!IsOwner) return;

        Vector3 mousePos = CodeMonkey.Utils.UtilsClass.GetMouseWorldPosition();

        PlayerState? playerState = GetPlayerState();

        if (playerState == null)
        {
            return;
        }
        else if (playerState == PlayerState.Shooting)
        {
            HandleIKInShootingState(mousePos);
        }
        else
        {
            HandleIKInBuildingState(mousePos);
        }

        // Gradually remove the recoil offset over time
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilSpeed);
    }


    public void ApplyRecoil(Vector3 recoilVector)
    {
        recoilOffset += recoilVector;
    }

    private PlayerState? GetPlayerState()
    {
        var stateController = transform.parent.GetComponent<PlayerActionController2D>();
        if (stateController == null)
        {
            Debug.LogWarning("Player doesn't have an ActionController2D or it couldn't be found!");
            return null;
        }

        return stateController.CurrentState;
    }

    private void HandleIKInShootingState(Vector3 mousePos)
    {
        rightArmTarget.position = mousePos + recoilOffset;
        leftArmTarget.position = mousePos + recoilOffset;
    }

    private void HandleIKInBuildingState(Vector3 mousePos)
    {
        Vector2 directionToMouse = (mousePos - gameObject.transform.position).normalized;

        rightArmTarget.position = gameObject.transform.position + (Vector3)directionToMouse * 5 + Vector3.down;
        leftArmTarget.position = gameObject.transform.position + (Vector3)directionToMouse * 5 + Vector3.down * 3;
    }

}
