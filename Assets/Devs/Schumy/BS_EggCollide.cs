using System.Collections;
using UnityEngine;

public class BS_EggCollide : MonoBehaviour
{
    [SerializeField] private GameObject eggPartLeft;
    [SerializeField] private GameObject eggPartRight;
    //[SerializeField] private GameObject eggCracks;
    //[SerializeField] private GameObject eggYellow;

    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float splitForce = 5f;
    [SerializeField] private float forwardRetentionFactor = 0.5f;
    [SerializeField] private float dragOverTime = 0.2f; 
    [SerializeField] private float rotationForce = 5f; 
    private bool collideable;
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.linearVelocity = transform.forward * forwardSpeed;
    }
    
    void OnCollisionEnter(Collision collision){
        if (collideable) return;
        collideable = true;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;

        GameObject leftPart = Instantiate(eggPartLeft, transform.position, transform.rotation);
        GameObject rightPart = Instantiate(eggPartRight, transform.position, transform.rotation);
        //Instantiate(eggCracks, transform.position, transform.rotation);
       // Instantiate(eggYellow, transform.position, transform.rotation);

        Rigidbody leftRb = leftPart.GetComponent<Rigidbody>();
        Rigidbody rightRb = rightPart.GetComponent<Rigidbody>();
        if (leftRb == null) leftRb = leftPart.AddComponent<Rigidbody>();
        if (rightRb == null) rightRb = rightPart.AddComponent<Rigidbody>();

        Vector3 forwardForce = transform.forward * forwardSpeed * forwardRetentionFactor;
        Vector3 leftForce = -transform.right * splitForce + forwardForce;
        Vector3 rightForce = transform.right * splitForce + forwardForce;

        leftRb.AddForce(leftForce, ForceMode.Impulse);
        rightRb.AddForce(rightForce, ForceMode.Impulse);
        Vector3 randomRotationLeft = new Vector3(Random.Range(-rotationForce, rotationForce), Random.Range(-rotationForce, rotationForce), Random.Range(-rotationForce, rotationForce));
        Vector3 randomRotationRight = new Vector3(Random.Range(-rotationForce, rotationForce), Random.Range(-rotationForce, rotationForce), Random.Range(-rotationForce, rotationForce));

        leftRb.AddTorque(randomRotationLeft, ForceMode.Impulse);
        rightRb.AddTorque(randomRotationRight, ForceMode.Impulse);
        leftPart.AddComponent<BS_EggPartHandler>().Initialize(leftRb, dragOverTime);
        rightPart.AddComponent<BS_EggPartHandler>().Initialize(rightRb, dragOverTime);

        Destroy(gameObject.GetComponent<MeshFilter>());
        StartCoroutine(Destroying());
    }

    IEnumerator Destroying()
    {
        Destroy(transform.GetChild(0).gameObject);
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }
}
