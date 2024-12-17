using System.Collections;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public abstract void InitAbility(PlayerController player);

    public void Activate(PlayerController player)
    {
        player.StartCoroutine(ActivationCoroutine(player));
    }

    public abstract IEnumerator ActivationCoroutine(PlayerController player);
}