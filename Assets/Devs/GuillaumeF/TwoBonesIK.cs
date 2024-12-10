using UnityEngine;


//[ExecuteInEditMode]

//public class TwoBonesIK : MonoBehaviour
//{
//    #region IK

//    [Header("IK Elements"), Space(10)]
//        public bool hasIK;
//        public float rotationUOffset;
//        public float rotationLOffset;

//    [Header("Bones")]
//        [SerializeField]
//        private Transform UpperBone;
//        [SerializeField]
//        private Transform LowerBone;
//        [SerializeField]
//        private Transform EndBone;

//    [Header("External Points")]
//        [SerializeField]
//        private Transform PoleVector;
//        [SerializeField]
//        private Transform Target;

//    #region cosValue
//        private float a;
//        private float b;
//        private float c;
//        private Vector3 plNm;
//    #endregion
//    #endregion

//    //WIP

//    // Update is called once per frame
//    void LateUpdate()
//    {
//        if (!hasIK || UpperBone == null || LowerBone == null || EndBone == null || PoleVector == null || Target == null)
//            return;

//        a = LowerBone.localPosition.magnitude;
//        b = EndBone.localPosition.magnitude;
//        c = Vector3.Distance(UpperBone.position, Target.position);


//        plNm = Vector3.Cross(Target.position - UpperBone.position, PoleVector.position - UpperBone.position);


//        Debug.DrawLine(UpperBone.position, Target.position);
//        Debug.DrawLine((UpperBone.position + Target.position) / 2, LowerBone.position);

//        Debug.Log("The angle is: " + CosAngle(a, b, c));
//        //UpperBone rotation :
//        UpperBone.rotation = Quaternion.LookRotation(Target.position - UpperBone.position, Quaternion.AngleAxis(rotationUOffset, LowerBone.position - UpperBone.position) * (plNm));
//        UpperBone.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, LowerBone.localPosition));
//        UpperBone.rotation = Quaternion.AngleAxis(-CosAngle(a, c, b), -plNm) * UpperBone.rotation;

//        //LowerBone rotation
//        LowerBone.rotation = Quaternion.LookRotation(Target.position - LowerBone.position, Quaternion.AngleAxis(rotationLOffset, EndBone.position - LowerBone.position) * (plNm));
//        LowerBone.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, EndBone.localPosition));

//        //LowerBone oriented toward PoleVector
//        LowerBone.LookAt(LowerBone, PoleVector.position - UpperBone.position);
//        LowerBone.rotation = Quaternion.AngleAxis(CosAngle(a, b, c), plNm);
//    }

//    //Function finding an angle based on cosine rule 
//    float CosAngle(float a, float b, float c)
//    {
//        if (!float.IsNaN(Mathf.Acos((-(c * c) + (a * a) + (b * b)) / (-2 * a * b)) * Mathf.Rad2Deg))
//        {
//            return Mathf.Acos((-(c * c) + (a * a) + (b * b)) / (2 * a * b)) * Mathf.Rad2Deg;
//        }
//        else
//        {
//            return 1;
//        }
//    }


//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.green;
//        Gizmos.DrawLine(UpperBone.position, LowerBone.position);
//        Gizmos.DrawLine(LowerBone.position, EndBone.position);
//    }

//}
