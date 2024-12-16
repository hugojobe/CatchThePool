using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    public List<ES_PlayerDiv> playerDivs;

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i >= GameInstance.instance.playerCount)
            {
                playerDivs[i].gameObject.SetActive(false);
            }
            else
            {
                playerDivs[i].Init();
            }
        }
        
        StartCoroutine(ShowPlayers());
    }

    private IEnumerator ShowPlayers()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        
        int winningPlayerIndex = GameInstance.instance.winningPlayerIndex;
        
        for (int i = 0; i < playerDivs.Count; i++)
        {
            if (i == winningPlayerIndex)
                continue;

            playerDivs[i].ShowDiv();
            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSecondsRealtime(1f);
        
        playerDivs[winningPlayerIndex].ShowWinDiv();
    }
}
