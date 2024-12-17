using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Nuggquake")]
public class Nuggquake : Ability
{
    public override void Activate(PlayerController player)
    {
        Debug.Log("Nuggquake activated");
        
        player.feedbackMachine.OnNuggquakeActivated();
    }
}
