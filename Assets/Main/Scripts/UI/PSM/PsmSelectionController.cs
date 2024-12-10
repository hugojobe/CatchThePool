using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PsmSelectionController : MonoBehaviour
{
    public int index = 0;

    public PsmManager psmManager;
    
    public PlayerInput input;
    private Gamepad pad;
    
    public Transform psmScrollRect;
    public int selectedIndex;
    private float psmImageWidth = 484f;
    
    private bool hasMoved = false;
    private bool hasConfirmed = false;
    
    public AnimationCurve lowFrequencyRumbleCurve;

    public AnimationCurve confirmRumbleCurve;


    public Transform psmSlot;
    public TextMeshProUGUI chickenNameText;
    public TextMeshProUGUI chickenSpellNameText;
    public Slider chickenHealthSlider;
    public Slider chickenSpeedSlider;

    private void Start()
    {
        pad = input.GetDevice<Gamepad>();
        UpdateDisplay();
    }

    public void OnPSM_MoveSelection(InputAction.CallbackContext context)
    {
        if (hasConfirmed)
            return;
        
        Vector2 input = context.ReadValue<Vector2>();

        if (context.phase == InputActionPhase.Performed)
        {
            if (Mathf.Abs(input.x) > 0.3f && !hasMoved)
            {
                hasMoved = true;

                if (input.x < 0)
                {
                    if (selectedIndex > 0)
                    {
                        selectedIndex--;
                        GamepadRumbleController.Rumble(pad, lowFrequencyRumbleCurve, lowFrequencyRumbleCurve, 0.1f, 0.1f);
                    }
                }
                else
                {
                    if (selectedIndex < 3)
                    {
                        selectedIndex++;
                        GamepadRumbleController.Rumble(pad, lowFrequencyRumbleCurve, lowFrequencyRumbleCurve, 0.1f, 0.1f);
                    }
                }
                
                psmScrollRect.DOLocalMoveX(-psmImageWidth * selectedIndex, 0.1f).SetEase(Ease.OutBack);
                UpdateDisplay();
            }
        }

        if (Mathf.Abs(input.x) <= 0.3f)
            hasMoved = false;
    }
    
    public void OnConfirmSelection(InputAction.CallbackContext context)
    {
        if (hasConfirmed)
            return;
        
        hasConfirmed = true;
        psmManager.confirmedPlayerCount++;
        
        GameInstance.instance.playerConfigs[index] = psmManager.chickenConfigs[selectedIndex];
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(psmManager.confirmPanels[index].DOLocalMoveY(-542f, 0.1f).SetEase(Ease.OutBack));
        sequence.Append(psmSlot.DOScale(Vector3.one * 1.08f, 0.15f).SetEase(Ease.OutQuint));
        sequence.Append(psmSlot.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutCirc));
        sequence.JoinCallback(() => GamepadRumbleController.Rumble(pad, confirmRumbleCurve, confirmRumbleCurve, 0.2f, 0.1f));
    }

    private void UpdateDisplay()
    {
        chickenNameText.text = psmManager.chickenConfigs[selectedIndex].chickenName;
        chickenSpellNameText.text = psmManager.chickenConfigs[selectedIndex].chickenSpellName;

        chickenHealthSlider.DOValue(psmManager.chickenConfigs[selectedIndex].chickenHealthPSM / 3f, 0.1f);
        chickenSpeedSlider.DOValue(psmManager.chickenConfigs[selectedIndex].chickenSpeedPSM / 3f, 0.1f);
    }
}
