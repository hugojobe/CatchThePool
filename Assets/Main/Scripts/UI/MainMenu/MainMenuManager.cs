using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject defaultSelectedButton;
    
    public GameObject gameLogo;
    public Image logoLeft, logoRight;
    public Image subtitlePanel, backgroundPanel;

    public GameObject menuButton1, menuButton2, menuButton3;
    
    [Space] 
    public AnimationCurve buttonCurve;

    [Space] 
    public GameObject optionsBackground;

    public GameObject optionsTitle;
    public GameObject[] optionsElements;
    public GameObject selectionArrow;
    public GameObject[] arrowPositions;
    
    public GameObject defaultSelectedOption;
    
    public bool isInOptionsMenu = false;

    private void Awake()
    {
        logoLeft.transform.DOLocalMoveX(-1600, 0);
        logoRight.transform.DOLocalMoveX(1600, 0);
        subtitlePanel.DOFade(0, 0);
        backgroundPanel.transform.DOScale(0, 0);

        menuButton1.transform.DOScale(Vector3.zero, 0);
        menuButton2.transform.DOScale(Vector3.zero, 0);
        menuButton3.transform.DOScale(Vector3.zero, 0);
        
        optionsBackground.transform.DOLocalMoveX(-1120, 0);
        optionsTitle.transform.DOLocalMoveX(-1530, 0);
        for(int i = 0; i < optionsElements.Length; i++)
        {
            optionsElements[i].GetComponent<CanvasGroup>().DOFade(0, 0);
            optionsElements[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        selectionArrow.transform.DOScale(Vector3.zero, 0);
    }

    private void Start()
    {

        MainMenuStartCinematic();
        AudioManager.PlayMusic("Mu_MainMenu",0);
    }

    public void Play()
    {
        menuButton1.GetComponent<InteractiblePanel>().Confirm();
        CSceneManager.LoadScene(SceneNames.PSM);
    }
    
    public void Options()
    {
        menuButton2.GetComponent<InteractiblePanel>().Confirm();
        OptionsMenuCinematic();
        isInOptionsMenu = true;
    }

    public void Quit()
    {
        menuButton3.GetComponent<InteractiblePanel>().Confirm();
        Application.Quit();
    }

    private void MainMenuStartCinematic()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f);
        
        sequence.Append(logoLeft.transform.DOLocalMoveX(0, 0.25f).SetEase(Ease.OutBack));
        sequence.Join(logoRight.transform.DOLocalMoveX(0, 0.25f).SetEase(Ease.OutBack));
        
        sequence.Join(backgroundPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetDelay(0.3f));
        
        sequence.Join(logoLeft.transform.DOPunchScale(Vector3.one * 0.05f, 0.15f).SetDelay(0.1f));
        sequence.Join(logoRight.transform.DOPunchScale(Vector3.one * 0.05f, 0.15f).SetDelay(0.1f));

        sequence.Append(subtitlePanel.DOFade(1, 0.5f).SetDelay(0.5f));

        sequence.Append(menuButton1.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        sequence.Append(menuButton2.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        sequence.Append(menuButton3.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        
        sequence.AppendInterval(0.5f);
        sequence.AppendCallback(() => EventSystem.current.SetSelectedGameObject(defaultSelectedButton));
    }

    private void OptionsMenuCinematic()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => EventSystem.current.SetSelectedGameObject(null));
        
        sequence.Append(gameLogo.transform.DOLocalMoveX(1890, 0.15f));
        sequence.Join(menuButton1.transform.DOLocalMoveX(1890, 0.15f).SetDelay(0.05f));
        sequence.Join(menuButton2.transform.DOLocalMoveX(1890, 0.15f).SetDelay(0.1f));
        sequence.Join(menuButton3.transform.DOLocalMoveX(1890, 0.15f).SetDelay(0.15f));
        
        sequence.Join(optionsBackground.transform.DOLocalMoveX(0, 0.2f).SetEase(Ease.OutBack).SetDelay(0.25f));
        sequence.Join(optionsTitle.transform.DOLocalMoveX(-962, 0.2f).SetEase(Ease.OutBack).SetDelay(0.3f));
        
        for (int i = 0; i < optionsElements.Length; i++)
        {
            sequence.Append(optionsElements[i].GetComponent<CanvasGroup>().DOFade(1, 0.2f).SetDelay(0.05f * i));
            sequence.JoinCallback(() => optionsElements[i].GetComponent<CanvasGroup>().blocksRaycasts = true);
        }
        sequence.AppendCallback(() => EventSystem.current.SetSelectedGameObject(defaultSelectedOption));
    }

    private void CloseOptionsMenu()
    {
        AudioManager.PlaySfx("Menu/SFX_Menu_Cancel/SFX_Menu_Cancel");

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => EventSystem.current.SetSelectedGameObject(null));
        
        sequence.Join(optionsBackground.transform.DOLocalMoveX(-1120, 0.15f));
        sequence.Join(optionsTitle.transform.DOLocalMoveX(-1530, 0.15f));
        sequence.Join(selectionArrow.transform.DOScale(Vector3.zero, 0.1f));
        
        for (int i = 0; i < optionsElements.Length; i++)
        {
            sequence.Join(optionsElements[i].GetComponent<CanvasGroup>().DOFade(0, 0.1f).SetDelay(i * 0.05f));
            sequence.JoinCallback(() => optionsElements[i].GetComponent<CanvasGroup>().blocksRaycasts = false);
        }
        
        sequence.Append(gameLogo.transform.DOLocalMoveX(0, 0.15f).SetEase(Ease.OutBack));
        sequence.Join(menuButton1.transform.DOLocalMoveX(0, 0.15f).SetDelay(0.05f).SetEase(Ease.OutBack));
        sequence.Join(menuButton2.transform.DOLocalMoveX(0, 0.15f).SetDelay(0.1f).SetEase(Ease.OutBack));
        sequence.Join(menuButton3.transform.DOLocalMoveX(0, 0.15f).SetDelay(0.15f).SetEase(Ease.OutBack));
        
        sequence.AppendCallback(() => EventSystem.current.SetSelectedGameObject(defaultSelectedButton));
    }

    public void OnBackButtonPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isInOptionsMenu)
            {
                CloseOptionsMenu();
                isInOptionsMenu = false;
            }
        }
    }
}