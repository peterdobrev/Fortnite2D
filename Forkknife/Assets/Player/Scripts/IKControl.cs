using UnityEngine;

public class IKControl : MonoBehaviour
{
    public Transform frontArmTarget;
    public Transform headTarget;

    private Vector3 recoilOffset = Vector3.zero;
    private float recoilSpeed = 10f;

    void Update()
    {
        Vector3 mousePos = CodeMonkey.Utils.UtilsClass.GetMouseWorldPosition();

        frontArmTarget.position = mousePos + recoilOffset;
        headTarget.position = mousePos; // assuming you don't want recoil on the head

        // Gradually remove the recoil offset over time
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilSpeed);
    }

    public void ApplyRecoil(Vector3 recoilVector)
    {
        recoilOffset += recoilVector;
    }
}
