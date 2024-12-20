using System.Linq;
using DG.Tweening;
using UnityEngine;

public class FeedbackMachine : MonoBehaviour
{
    public PlayerController pc;
    public GameObject playerVisualsParent;
    
    [Space]
    public AnimationCurve dashScaleCurve;

    [Space] 
    public GameObject damageVfx;
    
// DASH
    public void OnDashStarted()
    {
        
    }

    public void OnDashFinished()
    {
        
    }

    public void OnDashUpdate(float dashProgress)
    {
        
    }
    
// DAMAGE
    public void OnDamageTaken(GameObject damageCauser)
    {
        /*ImpactSetParameters obj = Instantiate(damageVfx).GetComponent<ImpactSetParameters>();
        obj.transform.position = pc.transform.position;
        obj.playerColor = damageCauser.GetComponent<PlayerController>().chickenColor;
        obj.targetPlayerColor = pc.chickenColor;
        obj.targetChicken = pc.chickenConfig.chickenType;*/
        
        float healthPercent = (float)pc.damageable.currentHealth / (float)pc.chickenConfig.chickenHealthGameplay;
        
        if (pc.damageable.currentHealth == 1)
        {
            for (int i = 0; i < pc.feathers.feathersList.Length; i++)
            {
                pc.feathers.feathersList[i].feathersInItem.ToList().ForEach(g => g.SetActive(false));
            }
        }
        else
        {
            for (int i = 0; i < pc.feathers.feathersList.Length; i++)
            {
                float threshold = (i + 1) / (float)pc.feathers.feathersList.Length;
                if (healthPercent <= threshold)
                    pc.feathers.feathersList[i].feathersInItem.ToList().ForEach(g => g.SetActive(true));
                else
                    pc.feathers.feathersList[i].feathersInItem.ToList().ForEach(g => g.SetActive(false));
            }
        }
    }
    
// DEATH
    public void OnDeath()
    {
        
    }
    
// ABIILITIES
    public void OnTourbiplumeActivated(GameObject particles)
    {
        GameObject particlesObj = Instantiate(particles, transform.position, Quaternion.identity);
    }
    
    public void OnHadoukoeufActivated(GameObject egg)
    {
        GameObject hadoukoeuf = Instantiate(egg, transform.position, Quaternion.identity);
        hadoukoeuf.transform.rotation = Quaternion.LookRotation(pc.transform.forward, pc.transform.up);
        hadoukoeuf.GetComponent<BS_EggCollide>().launcherPc = pc;
        Physics.IgnoreCollision(hadoukoeuf.GetComponent<Collider>(), pc.GetComponent<Collider>());
    }
    
    public void OnNuggquakeActivated()
    {
        
    }
    
    public void OnSpicyfartActivated()
    {
        
    }

    public void OnSpicyfartStarted()
    {
        
    }
    
    public void OnSpicyfartEnded()
    {
        
    }

    public void OnSpicyFartUpdate()
    {
        
    }
}