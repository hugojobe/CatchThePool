using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PsmManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager playerInputManager;

    public Transform[] psmSlots;
    public CanvasGroup[] emptySlotsCG;
    public CanvasGroup[] emptySlotJoinTexts;
    public CanvasGroup[] occupiedSlotCG;
    public Transform[] psmScrollRects;
    public TextMeshProUGUI[] chickenNameTexts;
    public TextMeshProUGUI[] chickenSpellNameTexts;
    public Slider[] chickenHealthSliders;
    public Slider[] chickenSpeedSliders;
    public Transform[] confirmPanels;
    public GameObject[] joinTexts;
    public GameObject[] leftArrows;
    public GameObject[] rightArrows;
    
    private string gameLaunchTextString = "Starting game... {0}";

    [Space] public TextMeshProUGUI gameLaunchText;

    private int _confirmedPlayerCount;
    public int confirmedPlayerCount
    {
        get => _confirmedPlayerCount;
        set {
            _confirmedPlayerCount = value;
            OnConfirmedPlayerCountChanged(value);
        }
    }

    private Coroutine gameLaunchCoroutine;

    [Space]
    public ChickenConfig[] chickenConfigs;

    private void Start()
    {
        AudioManager.PlayMusic("Mu_SelectChicken",0);

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
        gameLaunchText.DOFade(0, 0f);
        AudioManager.PlaySfx("Menu/SFX_Presentator/SFX_PresentatorCYC");

    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (gameLaunchCoroutine != null)
        {
            StopCoroutine(gameLaunchCoroutine);
            gameLaunchCoroutine = null;
            
            gameLaunchText.DOFade(0, 0.3f).SetEase(Ease.InOutSine);
        }

        emptySlotsCG[GameInstance.instance.playerCount].DOFade(0, 0.1f);
        occupiedSlotCG[GameInstance.instance.playerCount].DOFade(1, 0.1f);
        occupiedSlotCG[GameInstance.instance.playerCount].transform.DOPunchScale(Vector2.one * 0.1f, 0.2f);
        occupiedSlotCG[GameInstance.instance.playerCount].transform.DOPunchRotation(new Vector3(0, 0, Random.value < 0.5? 5f : -5f), 0.2f);
        
        GameInstance.instance.playerCount++;
        GameInstance.instance.playerConfigs.Add(null);
        GameInstance.instance.gamepadIDs.Add(playerInput.GetDevice<Gamepad>().deviceId);
        GameInstance.instance.playerAlive.Add(true);
        GameInstance.instance.playerScores.Add(0);
        GameInstance.instance.playerKills.Add(0);
        GameInstance.instance.playerDeaths.Add(0);
        
        if(GameInstance.instance.playerCount < playerInputManager.maxPlayerCount)
            emptySlotJoinTexts[GameInstance.instance.playerCount].DOFade(1, 0.2f);

        PsmSelectionController psmController = playerInput.GetComponent<PsmSelectionController>();
        psmController.psmManager = this;
        psmController.input = playerInput;
        psmController.index = playerInput.playerIndex;
        psmController.psmSlot = psmSlots[playerInput.playerIndex];
        psmController.psmScrollRect = psmScrollRects[playerInput.playerIndex];
        
        psmController.chickenNameText = chickenNameTexts[playerInput.playerIndex];
        psmController.chickenSpellNameText = chickenSpellNameTexts[playerInput.playerIndex];
        psmController.chickenHealthSlider = chickenHealthSliders[playerInput.playerIndex];
        psmController.chickenSpeedSlider = chickenSpeedSliders[playerInput.playerIndex];
        psmController.leftArrow = leftArrows[playerInput.playerIndex];
        psmController.rightArrow = rightArrows[playerInput.playerIndex];

        confirmPanels[playerInput.playerIndex].DOLocalMoveY(-634f, 0.15f).SetEase(Ease.OutBack).SetDelay(0.5f);
    }

    private void OnConfirmedPlayerCountChanged(int currentConfirmedPlayerCount)
    {
        if(currentConfirmedPlayerCount == GameInstance.instance.playerCount && currentConfirmedPlayerCount > 1)
            gameLaunchCoroutine = StartCoroutine(LaunchGame());
    }
    
    private IEnumerator LaunchGame()
    {
        gameLaunchText.text = string.Format(gameLaunchTextString, "3");
        gameLaunchText.DOFade(1f, 0.3f).SetEase(Ease.InOutSine);
        
        yield return new WaitForSeconds(1f);
        gameLaunchText.text = string.Format(gameLaunchTextString, "2");
        yield return new WaitForSeconds(1f);
        gameLaunchText.text = string.Format(gameLaunchTextString, "1");
        yield return new WaitForSeconds(1f);
        gameLaunchText.DOFade(0, 0.3f).SetEase(Ease.InOutSine);
        
        gameLaunchCoroutine = null;
        
        CSceneManager.LoadScene(SceneNames.Gameplay);
    }
}
