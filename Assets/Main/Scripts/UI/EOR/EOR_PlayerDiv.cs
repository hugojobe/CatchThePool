using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EOR_PlayerDiv : MonoBehaviour
{
    public List<Image> chickens;
    public Sprite[] chickenSprites;
    private int chickenCount = 0;
    
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 0;
    }

    public void AddChicken()
    {
        Image chickenImage = chickens[chickenCount];

        Sequence sequence = DOTween.Sequence();
        chickens[chickenCount].sprite = chickenSprites[Random.Range(0, chickenSprites.Length)];
        sequence.Append(chickenImage.DOFade(1f, 0.1f));
        sequence.Append(chickenImage.transform.DOScale(1.1f, 0f));
        sequence.Append(chickenImage.transform.DOScale(1f, 0.1f));
        
        chickenCount++;
    }
}
