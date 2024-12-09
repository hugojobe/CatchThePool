using UnityEngine;


[ExecuteInEditMode]
public class TwoBonesIKScript : MonoBehaviour
{
    #region IK

    [Header("IK Elements"), Space(10)]

    public bool hasIK;

    [Header("Bones")]
    [SerializeField]
    private Transform UpperBone;
    [SerializeField]
    private Transform LowerBone;
    [SerializeField]
    private Transform EndBone;

    [Header("External Points")]
    [SerializeField]
    private Transform PoleVector;

    [SerializeField]
    private Transform Target;
    [SerializeField]
    private float animSpeed = 10f;
    [SerializeField]
    private float powStrength = 2;


    #endregion



    // Update is called once per frame
    void LateUpdate()
    {
        if (!hasIK || UpperBone == null || LowerBone == null || EndBone == null || PoleVector == null || Target == null)
            return;

        float upperLength = Vector3.Distance(UpperBone.position, LowerBone.position);
        float lowerLength = Vector3.Distance(LowerBone.position, EndBone.position);
        float targetLength = Vector3.Distance(EndBone.position, Target.position);
        targetLength = Mathf.Min(targetLength, upperLength + lowerLength);


        float a = Mathf.Acos(Mathf.Pow(upperLength,powStrength) + Mathf.Pow(targetLength, powStrength) -Mathf.Pow(lowerLength,powStrength)/(2* upperLength * targetLength));
        float b = Mathf.Acos(Mathf.Pow(upperLength, powStrength) + Mathf.Pow(lowerLength, powStrength) - Mathf.Pow(targetLength, powStrength) / (2 * upperLength * lowerLength));

        //Rotation of upper bones toward target
        Quaternion upperRotation = Quaternion.LookRotation(Target.position - UpperBone.position, PoleVector.position - UpperBone.position);
        UpperBone.rotation = Quaternion.Slerp(UpperBone.rotation, upperRotation, Time.deltaTime * animSpeed);

        //Rotation of lower bones toward target
        //LowerBone.rotation = Quaternion.Slerp(LowerBone.rotation, Quaternion.Euler(0, 0, b * Mathf.Rad2Deg), Time.deltaTime * animSpeed);

        EndBone.position = Target.position;

    }
}
