using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public int index;
    public AudioChannel audioChannel;
    public Gradient titleGradient;
    public Image titleImage;
    
    public MainMenuManager mmm;

    private void Start()
    {
        AudioManager.SetMixerVolume(GetComponent<Slider>().value * 20, audioChannel);
        titleImage.DOColor(titleGradient.Evaluate(GetComponent<Slider>().value/6), 0f);
    }

    public void OnVolumeChanged(float value)
    {
        AudioManager.SetMixerVolume(value * 20, audioChannel);
        titleImage.DOColor(titleGradient.Evaluate(value/6), 0.15f);
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
    
    private void Highlight()
    {
        mmm.selectionArrow.transform.localPosition = mmm.arrowPositions[index].transform.localPosition;
        mmm.selectionArrow.transform.transform.DOScale(Vector3.zero, 0);
        mmm.selectionArrow.transform.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }
    
    private void Unhighlight() {
        
    }
}
