using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bullet : NetworkBehaviour//, IBullet
{
    public NetworkVariable<float> speed = new NetworkVariable<float>(5f);
    public NetworkVariable<int> damage = new NetworkVariable<int>(1);
    private NetworkVariable<Vector2> direction = new NetworkVariable<Vector2>();

    private bool hasSpawned = false;

    //private float timeAlive = 5f;

    public override void OnNetworkSpawn()
    {
        hasSpawned = true;
        base.OnNetworkSpawn();
    }

    void FixedUpdate()
    {
        if(!hasSpawned) return;
        Move();
        /*
        timeAlive -= Time.deltaTime;
        if (timeAlive < 0)
        {
            DestroyObjectServerRpc();
        }
        */
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
            NetworkLog.LogInfoServer($"Bullet collided with {collision.gameObject.name}");
            var damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                NetworkLog.LogInfoServer($"Bullet is being destroyed because it collided with idamageable: {collision.gameObject.name}");
                DealDamage(damageable);
                DestroyObjectServerRpc();
            }
        }
    }
    [ServerRpc]
    public void DestroyObjectServerRpc(ServerRpcParams rpcParams = default)
    {
        GetComponent<NetworkObject>().Despawn(true);
    }*/


    private void DealDamage(IDamageable damageable)
    {
        if (damageable != null)
        {
            damageable.TakeDamage(damage.Value);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        NetworkLog.LogInfoServer($"8. Direction of bullet set - {NetworkObjectId}");
        direction.Value = dir;
    }
}
