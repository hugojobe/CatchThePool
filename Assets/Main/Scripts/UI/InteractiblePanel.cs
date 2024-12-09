using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractiblePanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private Image backgroundImage;
    private Outline outline;

    public Color bgNormalColor;
    public Color bgHoverColor;

    public Color olNormalColor;
    public Color OlHoverColor;

    private void Awake() {
        backgroundImage = GetComponent<Image>();
        outline = GetComponent<Outline>();
        
        backgroundImage.color = bgNormalColor;
        outline.DOColor(olNormalColor, 0f);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Highlight();
    }

    public void OnPointerExit(PointerEventData eventData) {
        Unhighlight();
    }

    public void OnSelect(BaseEventData eventData) {
        Highlight();
    }

    public void OnDeselect(BaseEventData eventData) {
        Unhighlight();
    }
    
    private void Highlight() {
        backgroundImage.DOColor(bgHoverColor, 0.25f).SetEase(Ease.OutBack);
        outline.DOColor(OlHoverColor, 0.25f).SetEase(Ease.OutBack);
    }
    
    private void Unhighlight() {
        backgroundImage.DOColor(bgNormalColor, 0.25f).SetEase(Ease.OutBack);
        outline.DOColor(olNormalColor, 0.25f).SetEase(Ease.OutBack);
    }
    
    public void OnClick() {
        transform.DOPunchScale(Vector3.one * -0.1f, 0.15f, 10, 1f).SetEase(Ease.OutBack);
    }
}
