using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControl : MonoBehaviour
{
    public Transform frontArmTarget;
    public Transform headTarget;

    void Update()
    {
        Vector3 mousePos = CodeMonkey.Utils.UtilsClass.GetMouseWorldPosition();

        frontArmTarget.position = mousePos;
        headTarget.position = mousePos;
    }
}
