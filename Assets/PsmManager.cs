using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PsmManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager playerInputManager;

    public CanvasGroup[] emptySlotsCG;
    public CanvasGroup[] occupiedSlotCG;

    public GameObject[] joinTexts;

    private void Start()
    {
        foreach (GameObject joinText in joinTexts)
        {
            joinText.transform.DOLocalMoveY(joinText.transform.localPosition.y + 15f, 2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

            Sequence rotationSequence = DOTween.Sequence();
            rotationSequence.Append(joinText.transform.DORotate(new Vector3(0, 0, 4), 2.5f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InOutSine));
            rotationSequence.Append(joinText.transform.DORotate(new Vector3(0, 0, -8), 5f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InOutSine));
            rotationSequence.SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        emptySlotsCG[GameInstance.instance.playerCount].DOFade(0, 0.1f);
        occupiedSlotCG[GameInstance.instance.playerCount].DOFade(1, 0.1f);
        occupiedSlotCG[GameInstance.instance.playerCount].transform.DOPunchScale(Vector2.one * 0.1f, 0.2f);
        occupiedSlotCG[GameInstance.instance.playerCount].transform.DOPunchRotation(new Vector3(0, 0, Random.value < 0.5? 5f : -5f), 0.2f);
        
        GameInstance.instance.playerCount++;
        
        if(GameInstance.instance.playerCount < playerInputManager.maxPlayerCount)
            emptySlotsCG[GameInstance.instance.playerCount].DOFade(1, 0.2f);
    }
}
