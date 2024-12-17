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

        player.playerState = player.chickenConfig.abilityState;
        player.abilityCooldownElapsed = false;
        player.moveInput = Vector2.zero;
    
        yield return new WaitForSecondsRealtime(0.15f);
    
        Vector3 position = player.transform.position;
        float step = 10f;
        for (float theta = 0; theta < 360; theta += step)
        {
            for (float phi = 0; phi < 360; phi += step)
            {
                float x = radius * Mathf.Sin(Mathf.Deg2Rad * theta) * Mathf.Cos(Mathf.Deg2Rad * phi);
                float y = radius * Mathf.Sin(Mathf.Deg2Rad * theta) * Mathf.Sin(Mathf.Deg2Rad * phi);
                float z = radius * Mathf.Cos(Mathf.Deg2Rad * theta);
                Vector3 point = position + new Vector3(x, y, z);
                Debug.DrawLine(position, point, Color.red, 1f);
            }
        }
    
        Collider[] hits = Physics.OverlapSphere(player.transform.position, radius, playerLayer);
    
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject.CompareTag("Player"))
            {
                PlayerController hitPlayer = hits[i].GetComponent<PlayerController>();
                
                if(hitPlayer != null && hitPlayer != player)
                    hitPlayer.damageable.TakeDamage(hitPlayer.gameObject);
            }
        }

        yield return new WaitForSecondsRealtime(0.8f);
        
        player.playerState = PlayerState.Normal;

        yield return new WaitForSecondsRealtime(player.chickenConfig.abilityCooldown);

        player.abilityCooldownElapsed = true;
    }
}