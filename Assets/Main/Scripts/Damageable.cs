using System;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public event Action<GameObject> OnDamageTaken;
    public event Action OnDeath; 

    public int currentHealth;

    public void TakeDamage(GameObject damageCauser)
    {
        OnDamageTaken?.Invoke(damageCauser);
        
        currentHealth--;
        
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}