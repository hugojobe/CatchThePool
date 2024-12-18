using System;
using System.Collections;
using UnityEngine;

public class NuggquakeSphere : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
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
        
        pc.playerState = PlayerState.Uncontrolled;

        yield return null;
    }
}
