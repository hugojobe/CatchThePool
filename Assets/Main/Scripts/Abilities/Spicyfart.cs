using System.Collections;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Spicyfart")]
public class Spicyfart : Ability
{
    public GameObject frontVfx;
    public GameObject backVfx;
    public GameObject trail;
    public GameObject flames;
    
    public override void InitAbility(PlayerController player)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        
        AudioManager.PlaySfx("SFX_Combat/SFX_Attacks/SFX_SpicyFart");
        pc.trailInstance = Instantiate(trail);
        pc.trailInstance.transform.position = player.transform.position + new Vector3(0, 0, 0);
        pc.trailInstance.transform.SetParent(player.transform);
        pc.trailInstance.GetComponent<TrailRenderer>().emitting = true;
        
        pc.flameInstance = Instantiate(flames);
        pc.flameInstance.transform.position = player.transform.position + new Vector3(0, 0, 0);
        pc.flameInstance.transform.SetParent(player.transform);
        pc.flameInstance.GetComponent<ParticleSystem>().Stop();
    }

    public override IEnumerator ActivationCoroutine(PlayerController player)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        
        player.feedbackMachine.OnSpicyfartActivated();
        
        GameObject frontVfxInstance = Instantiate(frontVfx);
        frontVfxInstance.transform.position = player.transform.position;
        frontVfxInstance.transform.SetParent(player.transform);
        frontVfxInstance.transform.rotation = Quaternion.LookRotation(player.transform.forward, player.transform.up);
        GameObject child = frontVfxInstance.transform.GetChild(1).gameObject;
        float alpha = 1;
        DOTween.To(() => alpha, y =>
        {
            alpha = y;
            child.GetComponent<Renderer>().material.SetFloat("_AlphaFactor", alpha);
        }, 0f, 1f).SetEase(Ease.OutCubic);
        
        GameObject backVfxInstance = Instantiate(backVfx);
        backVfxInstance.transform.position = player.transform.position;
        frontVfxInstance.transform.rotation = Quaternion.LookRotation(player.transform.forward, player.transform.up);
        
        pc.trailInstance.GetComponent<TrailRenderer>().emitting = true;
        pc.flameInstance.GetComponent<ParticleSystem>().Play();
        
        player.animator.SetTrigger("Spicyfart");

        player.playerState = PlayerState.Spicyfart;
        player.abilityCooldownElapsed = false;
        
        player.StartCoroutine(player.DashCoroutine(new Vector3(player.moveInput.x, 0, player.moveInput.y).normalized, 2.1f));
        
        player.playerState = PlayerState.Normal;
        player.animator.SetTrigger("EndState");
        
        pc.trailInstance.GetComponent<TrailRenderer>().emitting = false;
        pc.flameInstance.GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSecondsRealtime(player.chickenConfig.abilityCooldown);

        player.abilityCooldownElapsed = true;
    }
}
