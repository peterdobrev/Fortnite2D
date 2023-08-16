using CodeMonkey.MonoBehaviours;
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition;
    [SerializeField] private float duration;
    [SerializeField] private float magnitude;



    void Awake()
    {
        originalPosition = transform.position;
    }

    public void Shake()
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        CameraFollow cameraFollow = GetComponent<CameraFollow>();
        if (cameraFollow == null)
        {
            Debug.LogError("CameraFollow script is missing!");
            yield break;
        }

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cameraFollow.ApplyShake(new Vector3(x, y, 0));

            elapsed += Time.deltaTime;

            yield return null;
        }

        cameraFollow.ApplyShake(Vector3.zero);
    }
}
