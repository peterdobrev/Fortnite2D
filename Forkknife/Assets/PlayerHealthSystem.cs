using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(HealthSystemComponent))]
public class PlayerHealthSystem : MonoBehaviour, IDamageable
{
    private HealthSystem healthSystem;

    private void Start()
    {
        healthSystem = GetComponent<HealthSystemComponent>().GetHealthSystem();
        healthSystem.OnDead += Die;
    }

    public void TakeDamage(float amount)
    {
        healthSystem.Damage(amount);
    }

    private void Die(object sender, System.EventArgs e)
    {
        SoundManager.instance.PlaySound("Die");
        KillPlayerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void KillPlayerServerRpc()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }


}
