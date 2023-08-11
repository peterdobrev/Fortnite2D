using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour, IBullet
{
    public float speed = 5f;
    public int damage = 1;
    private Vector2 direction;

    void Update()
    {
        Move();
    }

    public void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DealDamage(collision);
        OnCollision();
    }

    public void OnCollision() //implementing the interface
    {
        Destroy(gameObject);
    }

    private void DealDamage(Collision2D collision)
    {
        IDamageable damageableObject = collision.gameObject.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }
}
