using System.Collections;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Hadoukeouf")]
public class Hadoukeouf : Ability
{
    public GameObject hadoukoeufPrefab;
    
    public override void InitAbility(PlayerController player)
    {
        
    }

    public override IEnumerator ActivationCoroutine(PlayerController player)
    {
        player.playerState = player.chickenConfig.abilityState;
        player.abilityCooldownElapsed = false;
        AudioManager.PlaySfx("SFX_Combat/SFX_Attacks/SFX_Hadoukoeuf");

        var watchRotation = new Vector3(player.moveInput.x, 0, player.moveInput.y).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(0, watchRotation.y, 0), player.transform.up);
        player.rb.rotation = targetRotation;
        
        player.moveInput = Vector2.zero;
        
        player.animator.SetTrigger("Hadoukoeuf");
        
        yield return new WaitForSecondsRealtime(0.25f);
        
        player.feedbackMachine.OnHadoukoeufActivated(hadoukoeufPrefab);
        
        yield return new WaitForSecondsRealtime(0.2f);
        player.playerState = PlayerState.Normal;
        
        yield return new WaitForSecondsRealtime(player.chickenConfig.abilityCooldown);

        player.abilityCooldownElapsed = true;
        
        yield return null;
    }
}
