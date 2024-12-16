using System;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ES_PlayerDiv : MonoBehaviour
{
    public ChickenConfig chickenConfig;
    [FormerlySerializedAs("moveablTransform")] public Transform moveableTransform;
    [Space]
    public int index;
    public float hiddenYPos;

    [Space] 
    public TextMeshProUGUI chickenNameText;
    public TextMeshProUGUI scoreText;
    public Image chickenImage;
    
    public void Init()
    {
        chickenConfig = GameInstance.instance.playerConfigs[index];
        
        chickenImage.sprite = chickenConfig.chickenImage;
        chickenNameText.text = chickenConfig.chickenName;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Kills : " + GameInstance.instance.playerKills[index]);
        sb.AppendLine("Deaths : " + GameInstance.instance.playerDeaths[index]);

        scoreText.text = sb.ToString();
        
        moveableTransform.DOLocalMoveY(hiddenYPos, 0);
    }

    public void ShowDiv()
    {
        moveableTransform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutBack);
    }

    public void ShowWinDiv()
    {
        moveableTransform.DOScale(0, 0);
        moveableTransform.DOLocalMoveY(0, 0);
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(moveableTransform.DOScale(1.15f, 0.5f).SetEase(Ease.OutBack));
        sequence.Join(moveableTransform.DOLocalRotate(new Vector3(0, 360*2, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutCirc));
        sequence.AppendInterval(0.2f);
        sequence.Append(moveableTransform.DOScale(1f, 0.1f).SetEase(Ease.OutBack));
    }
}
