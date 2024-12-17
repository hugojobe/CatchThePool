using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Spicyfart")]
public class Spicyfart : Ability
{
    public override void Activate(PlayerController player)
    {
        Debug.Log("Spicyfart activated");
        
        player.feedbackMachine.OnSpicyfartActivated();
    }
}
