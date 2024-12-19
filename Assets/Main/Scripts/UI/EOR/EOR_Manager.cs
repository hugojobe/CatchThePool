using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class EOR_Manager : MonoBehaviour
{

    public CanvasGroup roundEndCanvas;
    
    public EOR_PlayerDiv[] eorPlayerDivs;

    public TextMeshProUGUI endRoundText;

    private void Awake()
    {
        roundEndCanvas.alpha = 0;
    }

    public void ShowRoundEndCanvas(int winningPlayerIndex, int currentRoundNumber)
    {
        endRoundText.text = $"End of round {currentRoundNumber}";
        StartCoroutine(ShowEORPlayerDivs(winningPlayerIndex, currentRoundNumber));
    }
    
    public IEnumerator ShowEORPlayerDivs(int winningPlayerIndex, int currentRoundNumber)
    {
        yield return roundEndCanvas.DOFade(1f, 0.25f).WaitForCompletion();
        
        for (int i = 0; i < GameInstance.instance.playerCount; i++)
        {
            eorPlayerDivs[i].canvasGroup.DOFade(1,0.15f);
            yield return new WaitForSeconds(0.1f);
        }

        if (winningPlayerIndex != -1)
        {
            yield return new WaitForSecondsRealtime(1.5f);
            eorPlayerDivs[winningPlayerIndex].AddChicken();
        }

        yield return new WaitForSecondsRealtime(2.5f);

        if (GameInstance.instance.playerScores[winningPlayerIndex] >= GameInstance.instance.requiredPointsToWin)
        {
            GameInstance.instance.winningPlayerIndex = winningPlayerIndex;
            CSceneManager.LoadScene(SceneNames.Victory);
            
            yield break;
        }
        
        HideRoundEndCanvas();
        GameInstance.instance.InitNewRound();
    } 

    public void HideRoundEndCanvas()
    {
        roundEndCanvas.DOFade(0, 0.1f); 
    }
}
