using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject defaultSelectedButton;
    
    public Image logoLeft, logoRight;
    public Image subtitlePanel, backgroundPanel;

    public GameObject menuButton1, menuButton2, menuButton3;
    
    [Space] public AnimationCurve buttonCurve;

    private void Awake()
    {
        logoLeft.transform.DOLocalMoveX(-1600, 0);
        logoRight.transform.DOLocalMoveX(1600, 0);
        subtitlePanel.DOFade(0, 0);
        backgroundPanel.transform.DOScale(0, 0);
        
        menuButton1.transform.DOScale(Vector3.zero, 0).SetEase(Ease.OutBack);
        menuButton2.transform.DOScale(Vector3.zero, 0).SetEase(Ease.OutBack);
        menuButton3.transform.DOScale(Vector3.zero, 0).SetEase(Ease.OutBack);
    }

    private void Start()
    {
        MainMenuStartCinematic();
    }

    public void Play()
    {
        menuButton1.GetComponent<InteractiblePanel>().Confirm();
        CSceneManager.LoadScene(SceneNames.PSM);
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
}
