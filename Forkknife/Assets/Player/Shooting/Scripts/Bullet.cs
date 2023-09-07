using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : NetworkBehaviour, IBullet
{
    public float speed = 5f;
    public int damage = 1;
    private Vector2 direction;

    void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsServer) // Ensure this logic only happens on the server for authority
        {
            DealDamage(collision);
            OnCollisionServerRpc();
        }
    }

    [ServerRpc]
    public void OnCollisionServerRpc(ServerRpcParams rpcParams = default)
    {
        OnCollisionClientRpc();
    }

    [ClientRpc]
    public void OnCollisionClientRpc(ClientRpcParams rpcParams = default)
    {
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
