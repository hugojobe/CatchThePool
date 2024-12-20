using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Tourbiplume")]
public class Tourbiplume : Ability
{
    public float radius = 2f;
    public LayerMask playerLayer;

    public GameObject particlesPrefab;

    public override void InitAbility(PlayerController player)
    {
        
    }

    public override IEnumerator ActivationCoroutine(PlayerController player)
    {
        player.feedbackMachine.OnTourbiplumeActivated(particlesPrefab);
        
        player.animator.SetTrigger("Tourbiplume");
        AudioManager.PlaySfx("SFX_Combat/SFX_Attacks/SFX_Tourbiplume");

        player.playerState = player.chickenConfig.abilityState;
        player.abilityCooldownElapsed = false;
        player.moveInput = Vector2.zero;
    
        yield return new WaitForSecondsRealtime(0.15f);
    
        Vector3 position = player.transform.position;
        float angle = 2 * Mathf.PI / 12;
        Vector3 prevPoint = position + new Vector3(radius, 0, 0);

        for (int i = 1; i <= 12; i++)
        {
            float x = Mathf.Cos(i * angle) * radius;
            float z = Mathf.Sin(i * angle) * radius;
            Vector3 newPoint = position + new Vector3(x, 0, z);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    
        Collider[] hits = Physics.OverlapSphere(player.transform.position, radius, playerLayer);
    
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.CompareTag("Player"))
            {
                PlayerController hitPlayer = hits[i].GetComponent<PlayerController>();
                
                if(hitPlayer != null && hitPlayer != player)
                    hitPlayer.damageable.TakeDamage(player.gameObject);
            }
        }

        yield return new WaitForSecondsRealtime(0.8f);
        
        player.playerState = PlayerState.Normal;

        player.StartCoroutine(player.SetCirclePercent());
        yield return new WaitForSecondsRealtime(player.chickenConfig.abilityCooldown);

        player.abilityCooldownElapsed = true;
    }
}