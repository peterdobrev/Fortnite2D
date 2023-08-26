using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.Events;

public class FollowMouse : MonoBehaviour
{
    [Header("Edge Detection Settings")]
    private const float EDGE_90_PERCENT = 0.90f;
    private const float EDGE_75_PERCENT = 0.75f;
    private const float CENTER_TOLERANCE = 0.05f;  // 5% tolerance for determining "close to center"

    [Header("Mouse Position Events")]
    public UnityEvent OnCloseToRightEdge90;
    public UnityEvent OnCloseToLeftEdge90;
    public UnityEvent OnCloseToTopEdge90;
    public UnityEvent OnCloseToBottomEdge90;
    public UnityEvent OnCenterishHorizontal75;
    public UnityEvent OnCenterishVertical75;
    public UnityEvent OnCloseToCenter;

    void Update()
    {
        Vector2 mousePosition = UtilsClass.GetMouseWorldPositionCinemachine();
        transform.position = mousePosition;

        CheckMousePositionRelativeToScreen();
    }

    private void CheckMousePositionRelativeToScreen()
    {
        float mouseXPercentage = Input.mousePosition.x / Screen.width;
        float mouseYPercentage = Input.mousePosition.y / Screen.height;

        if (mouseXPercentage > EDGE_90_PERCENT) OnCloseToRightEdge90?.Invoke();
        else if (mouseXPercentage < 1 - EDGE_90_PERCENT) OnCloseToLeftEdge90?.Invoke();

        if (mouseYPercentage > EDGE_90_PERCENT) OnCloseToTopEdge90?.Invoke();
        else if (mouseYPercentage < 1 - EDGE_90_PERCENT) OnCloseToBottomEdge90?.Invoke();

        if (mouseXPercentage > EDGE_75_PERCENT && mouseXPercentage < 1 - EDGE_75_PERCENT) OnCenterishHorizontal75?.Invoke();
        if (mouseYPercentage > EDGE_75_PERCENT && mouseYPercentage < 1 - EDGE_75_PERCENT) OnCenterishVertical75?.Invoke();

        if (Mathf.Abs(mouseXPercentage - 0.5f) < CENTER_TOLERANCE && Mathf.Abs(mouseYPercentage - 0.5f) < CENTER_TOLERANCE)
            OnCloseToCenter?.Invoke();
    }
}
