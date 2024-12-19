using System.Collections;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Nuggquake")]
public class Nuggquake : Ability
{
    public GameObject anticipationSmoke;
    public GameObject impactSmoke;
    public GameObject impactVfx;
    public GameObject impactSprite;
    public GameObject sphere;
    
    public override void InitAbility(PlayerController player)
    {
        
    }

    public override IEnumerator ActivationCoroutine(PlayerController player)
    {
        player.feedbackMachine.OnNuggquakeActivated();
        
        player.animator.SetTrigger("Nuggquake");
        AudioManager.PlaySfx("SFX_Combat/SFX_Attacks/SFX_NuggQuake");

        GameObject anticipationSmokeInstance = Instantiate(anticipationSmoke);
        anticipationSmokeInstance.transform.position = player.transform.position;

        player.playerState = player.chickenConfig.abilityState;
        player.abilityCooldownElapsed = false;
        player.moveInput = Vector2.zero;
        
        yield return new WaitForSecondsRealtime(1.4f);
        
        GameObject impactSmokeInstance = Instantiate(impactSmoke);
        impactSmokeInstance.transform.position = player.transform.position;
        GameObject impactVfxInstance = Instantiate(impactVfx);
        impactVfxInstance.transform.position = player.transform.position;
        
        GameObject sphereInstance = Instantiate(sphere);
        sphereInstance.transform.position = player.transform.position;
        sphereInstance.transform.localScale = Vector3.zero;
        sphereInstance.transform.DOScale(50f, 10f).SetEase(Ease.OutCirc);
        float fadeValue = 1;
        DOTween.To(() => fadeValue, y =>
        {
            fadeValue = y;
            sphereInstance.GetComponent<Renderer>().material.SetFloat("_AlphaFactor", fadeValue);
        }, 0f, 2f).SetEase(Ease.OutCirc);

        GameManager.instance.ringMPB.SetVector($"_Player{player.index + 1}Pos", player.transform.position);
        
        float radius = 0;
        DOTween.To(() => radius, x =>
        {
            radius = x;
            GameManager.instance.ringMPB.SetFloat($"_RadiusP{player.index + 1}", radius);
            GameManager.instance.ringRend.SetPropertyBlock(GameManager.instance.ringMPB);
        }, 15, 2f).SetEase(Ease.OutCirc);
        
        yield return new WaitForSecondsRealtime(0.1f);
        
        player.playerState = PlayerState.Normal;

        yield return new WaitForSecondsRealtime(player.chickenConfig.abilityCooldown);

        player.abilityCooldownElapsed = true;
    }
}
