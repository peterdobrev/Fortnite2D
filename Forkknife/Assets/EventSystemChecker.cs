using UnityEngine.EventSystems;
using UnityEngine;

public class EventSystemChecker : MonoBehaviour
{
    float timer = 0f;
    float time = 5f;

    private void Awake()
    {
        EnsureSingleEventSystem();
    }

    private void Update()
    {
        if(time > timer)
        {
            timer += Time.deltaTime;
            return;
        }
        timer = 0f;
        EnsureSingleEventSystem();
    }

    private void EnsureSingleEventSystem()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        if (eventSystems.Length > 1)
        {
            for (int i = 1; i < eventSystems.Length; i++)  // start at index 1 to keep the first one
            {
                Destroy(eventSystems[i].gameObject);
            }
        }
    }
}
