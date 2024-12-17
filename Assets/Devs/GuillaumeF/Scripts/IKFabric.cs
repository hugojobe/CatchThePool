using UnityEngine;


[ExecuteInEditMode]

public class IKFabric : MonoBehaviour
{
    #region IK
    public int chainLength = 2;


    public Transform target;
    public Transform pole;

    #endregion

    //IterationSolver :

    [Header("Solver")]
    public int iterations = 18;
    public float targetDelta = 0.001f;

    [Range(0.0f, 1.0f)]
    public float startSnappingStrength = 1f;
}

