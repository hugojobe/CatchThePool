using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Hadoukeouf")]
public class Hadoukeouf : Ability
{
    public override void Activate(PlayerController player)
    {
        Debug.Log("Hadoukoeuf activated");
        
        player.feedbackMachine.OnHadoukoeufActivated();
    }
}
