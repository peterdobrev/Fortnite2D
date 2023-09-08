using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : NetworkBehaviour//, IBullet
{
    public NetworkVariable<float> speed = new NetworkVariable<float>(5f);
    public NetworkVariable<int> damage = new NetworkVariable<int>(1);
    private NetworkVariable<Vector2> direction = new NetworkVariable<Vector2>();

    void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        transform.Translate(direction.Value * speed.Value * Time.deltaTime);
    }

     /*
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
            damageableObject.TakeDamage(damage.Value);
        }
    }*/

    public void SetDirection(Vector2 dir)
    {
        NetworkLog.LogInfoServer($"8. Direction of bullet set - {NetworkObjectId}");
        direction.Value = dir;
    }
}
