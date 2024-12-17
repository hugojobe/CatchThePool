using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Spicyfart")]
public class Spicyfart : Ability
{
    public override void InitAbility(PlayerController player)
    {
        
    }

    public override IEnumerator ActivationCoroutine(PlayerController player)
    {
        Debug.Log("Spicyfart activated");
        
        player.feedbackMachine.OnSpicyfartActivated();

        yield return null;
    }
}
