using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Tourbiplume")]
public class Tourbiplume : Ability
{
    public override void Activate(PlayerController player)
    {
        Debug.Log("Tourbiplume activated");
        
        player.feedbackMachine.OnTourbiplumeActivated();
    }
}
