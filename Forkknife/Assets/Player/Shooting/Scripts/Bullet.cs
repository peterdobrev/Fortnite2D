using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public float speed = 40f;
    private Vector2 direction = new Vector2();

    private float timeAlive = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Bullet collided with: " + collision.gameObject.name);
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        Move();

        timeAlive -= Time.deltaTime;
        if (timeAlive < 0)
        {
            Destroy(gameObject);
        }
    }

    public void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void SetDirection(Vector2 dir)
    {
        Debug.Log($"8. Direction of bullet set");
        direction = dir;
    }

    public void OnDestroy()
    {
        Debug.Log("Bullet is being destroyed");
    }

    public void SetTimeAlive(float time)
    {
        timeAlive = time;
    }
}
