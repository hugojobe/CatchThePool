using DG.Tweening;
using UnityEngine;

public class FeedbackMachine : MonoBehaviour
{
    public PlayerController pc;
    public GameObject playerVisualsParent;
    
    [Space]
    public AnimationCurve dashScaleCurve;
    
    public void OnDashStarted()
    {
        
    }

    public void OnDashFinished()
    {
        
    }

    public void OnDashUpdate(float dashProgress)
    {
        Vector3 localScale = playerVisualsParent.transform.localScale;
        playerVisualsParent.transform.localScale = new Vector3(localScale.x, localScale.y, dashScaleCurve.Evaluate(dashProgress));
        
    }
}