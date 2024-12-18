using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Nuggquake")]
public class Nuggquake : Ability
{
    public override void InitAbility(PlayerController player)
    {
        
    }

    public override IEnumerator ActivationCoroutine(PlayerController player)
    {
        player.feedbackMachine.OnNuggquakeActivated();
        
        player.animator.SetTrigger("Nuggquake");
        
        Debug.Log("Nuggquake activated");

        player.playerState = player.chickenConfig.abilityState;
        player.abilityCooldownElapsed = false;
        player.moveInput = Vector2.zero;
        
        yield return new WaitForSecondsRealtime(0.32f);
        
        // DOTween for vfxs
        
        yield return new WaitForSecondsRealtime(0.1f);
        
        player.playerState = PlayerState.Normal;

        yield return new WaitForSecondsRealtime(player.chickenConfig.abilityCooldown);

        player.abilityCooldownElapsed = true;
    }
}
