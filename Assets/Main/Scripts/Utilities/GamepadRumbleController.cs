using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadRumbleController : MonoBehaviour
{
    public static void Rumble(Gamepad gamepad, AnimationCurve lowFrequecyRumbleCurve, AnimationCurve highFrequecyRumbleCurve, float duration, float intensity)
    {
        if (GameInstance.instance.gamepadRumbleCoroutines.ContainsKey(gamepad))
        {
            GameInstance.instance.StopCoroutine(GameInstance.instance.gamepadRumbleCoroutines[gamepad]);
            GameInstance.instance.gamepadRumbleCoroutines.Remove(gamepad);
        }


        GameInstance.instance.gamepadRumbleCoroutines.Add(gamepad, GameInstance.instance.StartCoroutine(RumbleCoroutine(gamepad, lowFrequecyRumbleCurve, highFrequecyRumbleCurve, duration, intensity)));
    }
    
    private static IEnumerator RumbleCoroutine(Gamepad gamepad, AnimationCurve lowFrequecyRumbleCurve, AnimationCurve highFrequecyRumbleCurve, float duration, float intensity)
    {
        gamepad.SetMotorSpeeds(0, 0);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float lItensity = lowFrequecyRumbleCurve.Evaluate(elapsedTime / duration);
            float hItensity = highFrequecyRumbleCurve.Evaluate(elapsedTime / duration);
            gamepad.SetMotorSpeeds(lItensity * intensity, hItensity * intensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        gamepad.SetMotorSpeeds(0, 0);
    }
}