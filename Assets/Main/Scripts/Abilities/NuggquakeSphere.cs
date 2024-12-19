using System;
using System.Collections;
using UnityEngine;

public class NuggquakeSphere : MonoBehaviour
{
    public PlayerController owner;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null && pc != owner)
            {
                StartCoroutine(NuggquakeSphereCoroutine(pc));
            }
        }
    }

    private IEnumerator NuggquakeSphereCoroutine(PlayerController pc)
    {
        PlayerState currentState = pc.playerState;
        
        if(currentState == PlayerState.Dashing)
            pc.ForceFinishDash();
        
        if (pc.playerState != PlayerState.Spicyfart)
        {

            pc.moveSpeed = pc.chickenConfig.chickenSpeed / 4f;

            yield return new WaitForSeconds(1.5f);

            pc.moveSpeed = pc.chickenConfig.chickenSpeed;
        }
    }
}
