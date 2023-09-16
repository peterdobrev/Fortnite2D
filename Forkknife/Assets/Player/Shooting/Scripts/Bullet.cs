using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// this whole class should be server controlled, but it isnt
/// </summary>


[RequireComponent(typeof(Collider2D))]
public class Bullet : NetworkBehaviour//, IBullet
{
    public NetworkVariable<float> speed = new NetworkVariable<float>(5f);
    public NetworkVariable<int> damage = new NetworkVariable<int>(1);
    private NetworkVariable<Vector2> direction = new NetworkVariable<Vector2>();

    private float timeAlive = 5f;

    void FixedUpdate()
    {
        if (gameObject.IsDestroyed()) return;
        Move();
        timeAlive -= Time.deltaTime;
        if (timeAlive < 0)
        {
            DestroyObjectServerRpc();
        }
    }

    public void Move()
    {
        transform.Translate(direction.Value * speed.Value * Time.deltaTime);
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (gameObject.IsDestroyed()) return;

        NetworkLog.LogInfoServer($"Bullet collided with {collider.gameObject.name}");
        var damageable = collider.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            NetworkLog.LogInfoServer($"Bullet is being destroyed because it collided with idamageable: {collider.gameObject.name}");
            DealDamage(damageable);
        }
        gameObject.SetActive(false);
        DestroyObjectServerRpc();
    }


    [ServerRpc (RequireOwnership = false)]
    public void DestroyObjectServerRpc(ServerRpcParams rpcParams = default)
    {
        if (gameObject.IsDestroyed()) return;
        GetComponent<NetworkObject>().Despawn(true);
    }


    private void DealDamage(IDamageable damageable)
    {
        if (gameObject.IsDestroyed()) return;
        if (damageable != null)
        {
            damageable.TakeDamage(damage.Value);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        if (gameObject.IsDestroyed()) return;
        NetworkLog.LogInfoServer($"8. Direction of bullet set - {NetworkObjectId}");
        direction.Value = dir;
    }
}
