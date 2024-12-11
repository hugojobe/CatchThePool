using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractiblePanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public MainMenuManager mmm;
    public Image[] particlesOff;
    public Image[] particlesOn;
    public Image normalTextImage, selectedTextImage;
    public Transform background;

    private void Awake()
    {
        int i = 0;
        foreach (Image pImage in particlesOff) {
            float randomRotationOffset = Random.Range(-4f, 4f);

            pImage.transform.localRotation = Quaternion.Euler(0, 0, randomRotationOffset);

            Image pImageOn = particlesOn[i];
            pImageOn.transform.localRotation = Quaternion.Euler(0, 0, randomRotationOffset);

            bool rotationDirection = Random.value < 0.5f;
            
            Sequence rotationSequence = DOTween.Sequence();

            rotationSequence.Append(pImage.transform.DORotate(new Vector3(0, 0, rotationDirection ? 4 : -4), 2.5f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine));
            rotationSequence.Join(pImageOn.transform.DORotate(new Vector3(0, 0, rotationDirection ? 4 : -4), 2.5f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine));
            rotationSequence.Append(pImage.transform.DORotate(new Vector3(0, 0, rotationDirection ? -8 : 8), 5f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine));
            rotationSequence.Join(pImageOn.transform.DORotate(new Vector3(0, 0, rotationDirection ? -8 : 8), 5f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine));
            rotationSequence.SetLoops(-1, LoopType.Yoyo);

            pImage.transform.localScale = Vector3.zero;
            pImageOn.DOFade(0, 0);
            i++;
        }
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
        int i = 0;
        foreach (Image pImage in particlesOff) {
            Image pImageOn = particlesOn[i];

            pImageOn.DOFade(0, 0);
            pImageOn.transform.DOScale(Vector3.zero, 0);
            pImage.DOFade(1, 0);
            pImageOn.DOFade(1, 0);
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(pImage.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack));
            sequence.Join(pImageOn.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack));
            sequence.Join(normalTextImage.DOFade(0, 0.15f));
            sequence.Join(selectedTextImage.DOFade(1, 0.15f));
            sequence.Join(background.DOPunchScale(Vector3.one * 0.05f, 0.15f));
            sequence.AppendInterval(0.25f);
            sequence.Append(pImageOn.DOFade(1f, 0.2f));
            i++;
        }

        foreach (Gamepad pad in Gamepad.all)
        {
            AnimationCurve buttonCurve = mmm.buttonCurve;
            GamepadRumbleController.Rumble(pad, buttonCurve, buttonCurve, 0.1f, 0.1f);
        }
    }
    
    private void Unhighlight() {
        int i = 0;
        foreach (Image pImage in particlesOff) {
            Image pImageOn = particlesOn[i];

            Sequence sequence = DOTween.Sequence();
            sequence.Append(pImage.transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuad));
            sequence.Join(pImageOn.transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuad));
            sequence.Join(pImage.DOFade(0, 0.15f));
            sequence.Join(pImageOn.DOFade(0, 0.15f));
            sequence.Join(normalTextImage.DOFade(1, 0.15f));
            sequence.Join(selectedTextImage.DOFade(0, 0.15f));
            sequence.Join(background.DOScale(Vector3.one, 0.15f));
            i++;
        }
    }

    public void Confirm()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOPunchScale(Vector3.one * 0.15f, 0.15f).SetEase(Ease.OutBack));
        sequence.AppendCallback(Unhighlight);
    }
}
