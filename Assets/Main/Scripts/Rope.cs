using DG.Tweening;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public Vector3 ropeReflectNormal;
    public Vector3 contactPointNormalPosition;

    public LineRenderer lineRenderer;
    private Vector3 targetPosition;
    private bool isInitialized = false;
    private bool tweenCompleted = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, ropeReflectNormal);
    }

    private void OnDrawGizmos()
    {
        if (isInitialized)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 0.2f);
        }
    }

    public void Init(Vector3 contactPosition, PlayerController player)
    {
        Vector3 startPoint = lineRenderer.GetPosition(0);
        Vector3 endPoint = lineRenderer.GetPosition(2);

        Vector3 direction = (endPoint - startPoint).normalized;
        Vector3 projectedContactPosition = Vector3.Project(contactPosition - startPoint, direction) + startPoint;

        contactPointNormalPosition = new Vector3(projectedContactPosition.x, lineRenderer.GetPosition(2).y, projectedContactPosition.z);

        targetPosition = contactPointNormalPosition - ropeReflectNormal;

        isInitialized = true;
        tweenCompleted = false;

        Vector3 initialPosition = contactPointNormalPosition;
        DOTween.To(() => initialPosition, x =>
        {
            targetPosition = x;
            lineRenderer.SetPosition(1, targetPosition);
        }, targetPosition, 0.05f).OnComplete(() =>
        {
            DOTween.To(() => targetPosition, x =>
            {
                targetPosition = x;
                lineRenderer.SetPosition(1, targetPosition);
            }, initialPosition, 0.2f).SetEase(Ease.OutElastic).OnComplete(() => tweenCompleted = true);
        });
    }

    private void Update()
    {
        if (isInitialized && !tweenCompleted)
        {
            lineRenderer.SetPosition(1, targetPosition);
        }
    }
}