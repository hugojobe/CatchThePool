using UnityEngine;

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


    #endregion



    // Update is called once per frame
    void LateUpdate()
    {
        if (!hasIK || UpperBone == null || LowerBone == null || EndBone == null || PoleVector == null || Target == null)
            return;


    }
}
