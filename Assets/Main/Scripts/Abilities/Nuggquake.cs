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
        Debug.Log("Nuggquake activated");
        
        player.feedbackMachine.OnNuggquakeActivated();

        yield return null;
    }
}
