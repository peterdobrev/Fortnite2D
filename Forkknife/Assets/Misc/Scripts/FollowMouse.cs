using CodeMonkey.Utils;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = UtilsClass.GetMouseWorldPosition();
        transform.position = mousePosition;
        Debug.Log(mousePosition);
    }
}
