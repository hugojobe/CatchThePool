using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EOR_PlayerDiv : MonoBehaviour
{
    public List<Image> chickens;
    private int chickenCount = 0;
    
    public Color chickenColor;

    public void AddChicken()
    {
        Image chickenImage = chickens[chickenCount];
        
        chickenImage.DOColor(chickenColor, 0.15f);
        chickenImage.transform.DOPunchScale(Vector3.one * 0.1f, 0.15f);
        
        chickenCount++;
    }
}
