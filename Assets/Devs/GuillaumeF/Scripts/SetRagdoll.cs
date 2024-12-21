using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SetRagdoll : MonoBehaviour
{
    public GameObject[] Bones;
    public Rigidbody[] rbs;

    public float mass = .5f;
    public int _solverIterations;
    public int _velocitySolverIterations;
    public RigidbodyInterpolation interpolationType;

    public bool setGravity;
    private List<GameObject> rbGO = new();
    public List<GameObject> rigidBodiedGameObject => rbGO;

    public bool worldRagdoll;

    void OnEnable()
    {

        if ((Bones.Length > 0 || rbs.Length > 0) && _solverIterations != 0 && _velocitySolverIterations != 0)
        {

            if (Bones.Length > 0)
            {
                foreach (var b in Bones)
                {
                    if (b.GetComponent<Rigidbody>() == null)
                        b.AddComponent<Rigidbody>();
                    b.GetComponent<Rigidbody>().solverIterations = _solverIterations;
                    b.GetComponent<Rigidbody>().solverVelocityIterations = _velocitySolverIterations;
                    b.GetComponent<Rigidbody>().mass = mass;
                    if (setGravity)
                    {
                        b.GetComponent<Rigidbody>().useGravity = setGravity;
                    }
                    b.GetComponent<Rigidbody>().interpolation = interpolationType;
                    rbGO.Add(b);
                }
            }

            if (rbs.Length > 0)
            {
                foreach (Rigidbody rb in rbs)
                {
                    rb.solverIterations = _solverIterations;
                    rb.solverVelocityIterations = _velocitySolverIterations;
                    rbGO.Add(rb.gameObject);
                }
            }

            if (!worldRagdoll)
            {
                foreach (GameObject gameObject in rbGO)
                {
                    ConfigurableJoint joint;
                    if (gameObject.name.ToLower() != "root")
                    {
                        if (gameObject.GetComponent<ConfigurableJoint>() != null)
                        {
                            joint = gameObject.GetComponent<ConfigurableJoint>();
                        }
                        else
                        {
                            joint = gameObject.AddComponent<ConfigurableJoint>();
                        }

                        joint.connectedBody = gameObject.transform.parent.GetComponent<Rigidbody>();
                        joint.angularXMotion = ConfigurableJointMotion.Limited;
                        joint.angularYMotion = ConfigurableJointMotion.Limited;
                        joint.angularZMotion = ConfigurableJointMotion.Limited;
                        joint.connectedBody = gameObject.transform.parent.GetComponent<Rigidbody>();
                        joint.xMotion = ConfigurableJointMotion.Limited;
                        joint.yMotion = ConfigurableJointMotion.Limited;
                        joint.zMotion = ConfigurableJointMotion.Limited;
                    }
                }
            }
            else
            {

                foreach (GameObject gameObject in rbGO)
                {
                    gameObject.AddComponent<SphereCollider>().radius = .5f * transform.localScale.x;
                }
            }

            enabled = false;
        }



    }


}
