using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EOR_PlayerDiv : MonoBehaviour
{
    public List<Image> chickens;
    private int chickenCount = 0;
    
    public Color chickenColor;
    
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 0;
    }

    public void AddChicken()
    {
        Image chickenImage = chickens[chickenCount];


        Sequence sequence = DOTween.Sequence();
        sequence.Append(chickenImage.DOColor(chickenColor, 0.1f));
        sequence.Append(chickenImage.DOFade(0, 0.1f));
        sequence.Append(chickenImage.DOFade(1, 0.1f));
        sequence.Append(chickenImage.DOColor(chickenColor, 0.1f));
        
        chickenCount++;
    }
}
