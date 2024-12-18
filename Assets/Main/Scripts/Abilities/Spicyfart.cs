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
        player.feedbackMachine.OnSpicyfartActivated();
        
        player.animator.SetTrigger("Spicyfart");

        player.playerState = PlayerState.Spicyfart;
        player.abilityCooldownElapsed = false;
        
        player.StartCoroutine(player.DashCoroutine(new Vector3(player.moveInput.x, 0, player.moveInput.y).normalized, 2.1f));
        
        player.playerState = PlayerState.Normal;
        player.animator.SetTrigger("EndState");

        yield return new WaitForSecondsRealtime(player.chickenConfig.abilityCooldown);

        player.abilityCooldownElapsed = true;
    }
}
