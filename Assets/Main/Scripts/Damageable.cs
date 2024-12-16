using System;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public PlayerController playerController;
    
    public event Action<GameObject> OnDamageTaken;
    public event Action OnDeath; 

    public int currentHealth;

    public void TakeDamage(GameObject damageCauser)
    {
        OnDamageTaken?.Invoke(damageCauser);
        
        currentHealth--;
        
        if (currentHealth <= 0)
        {
            GameInstance.instance.playerKills[damageCauser.GetComponent<PlayerController>().index]++;
            OnDeath?.Invoke();
            GameInstance.instance.playerAlive[playerController.index] = false;
            GameInstance.instance.CheckForEndOfRound();
        }
    }
}