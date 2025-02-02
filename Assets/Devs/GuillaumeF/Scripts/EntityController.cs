using System;
using UnityEngine;

// Tutorial Followed :
// https://weaverdev.io/projects/bonehead-procedural-animation/
// https://discussions.unity.com/t/ik-foot-placement-setup/882657/2

[ExecuteInEditMode]

public class EntityController : MonoBehaviour
{

    #region head
    [Header("Head Parts"), Space(10)]
    [SerializeField]
    Transform headBone;
    [SerializeField]
    Transform target;

    public bool moveHead;

    private Quaternion currentHeadLocalRotation;
    private Quaternion targetHeadLocalRotation;

    Vector3 LookDir, LocalLookDir;


    Quaternion targetHeadRotation;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float maxHeadTurnAngle = 60f;
    #endregion

    #region eyes
    [Header("Eyes element"), Space (10)]
    public bool animateEyes = true;

    [SerializeField]
    private Transform leftEyeBone;
    [SerializeField]
    private Transform rightEyeBone;

    [SerializeField]
    private float eyeTrackingSpeed = 5f;
    [SerializeField]
    private float eyeMaxYRotation = 90f;
    [SerializeField]
    private float eyeMinYRotation = 0f;


    #endregion


    void LateUpdate()
    {
        if (moveHead && headBone!= null && target != null)
            HeadTrackingUpdate();
        if (animateEyes && leftEyeBone != null && rightEyeBone != null)
            EyeTrackingUpdate();



    }

    private void EyeTrackingUpdate()
    {
        Quaternion targetEyeRotation = Quaternion.LookRotation( target.position - headBone.position, transform.up);

        leftEyeBone.rotation = Quaternion.Slerp(leftEyeBone.rotation, targetEyeRotation, 1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime));
        rightEyeBone.rotation = Quaternion.Slerp(rightEyeBone.rotation, targetEyeRotation, 1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime));

        float leftEyeCurrentRotation = leftEyeBone.localEulerAngles.y;
        float rightEyeCurrentRotation = rightEyeBone.localEulerAngles.y;

        if (leftEyeCurrentRotation > 180)
            leftEyeCurrentRotation -= 360;

        if (rightEyeCurrentRotation > 180)
            rightEyeCurrentRotation -= 360;

        float leftEyeClampedRotation = ClampingRotation(leftEyeCurrentRotation, eyeMinYRotation, eyeMaxYRotation);
        float rightEyeClampedRotation = ClampingRotation(rightEyeCurrentRotation, eyeMinYRotation, eyeMaxYRotation);

        leftEyeBone.localEulerAngles = new Vector3(leftEyeBone.localEulerAngles.x, leftEyeClampedRotation, leftEyeBone.localEulerAngles.z);
        rightEyeBone.localEulerAngles = new Vector3(rightEyeBone.localEulerAngles.x, rightEyeClampedRotation, rightEyeBone.localEulerAngles.z);

    }

    private float ClampingRotation(float currentEyeRotation, float minYRotation, float maxYRotation)
    {
        float clampedEyeRotation = Mathf.Clamp(currentEyeRotation, minYRotation, maxYRotation);

        return clampedEyeRotation;
    }

    private void HeadTrackingUpdate()
    {
        currentHeadLocalRotation = headBone.localRotation;

        headBone.localRotation = Quaternion.identity;

        LookDir = (target.position - headBone.position);

        LocalLookDir = headBone.InverseTransformDirection(LookDir);

        LocalLookDir = Vector3.RotateTowards(Vector3.forward, LocalLookDir, Mathf.Deg2Rad * maxHeadTurnAngle, 0);

        targetHeadRotation = Quaternion.LookRotation(LocalLookDir, transform.up);

        targetHeadLocalRotation = Quaternion.LookRotation(LocalLookDir, Vector3.up);

        headBone.localRotation = Quaternion.Slerp(currentHeadLocalRotation, targetHeadLocalRotation, 1 - Mathf.Exp(-speed * Time.deltaTime));
    }

}
