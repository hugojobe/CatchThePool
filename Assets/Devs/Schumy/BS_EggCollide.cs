using UnityEngine;

public class BS_EggCollide : MonoBehaviour
{
    public GameObject eggPartLeft;
    public GameObject eggPartRight;
    public GameObject eggCracks;
    public float forwardSpeed = 10f;
    public float splitForce = 5f;
    public float forwardRetentionFactor = 0.5f;
    public float dragOverTime = 0.2f; // Facteur de ralentissement
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.linearVelocity = transform.forward * forwardSpeed;
    }

    void OnTriggerEnter(Collider collision){
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;

        GameObject leftPart = Instantiate(eggPartLeft, transform.position, transform.rotation);
        GameObject rightPart = Instantiate(eggPartRight, transform.position, transform.rotation);
        Instantiate(eggCracks, transform.position, transform.rotation);
        
        Rigidbody leftRb = leftPart.GetComponent<Rigidbody>();
        Rigidbody rightRb = rightPart.GetComponent<Rigidbody>();
        if (leftRb == null) leftRb = leftPart.AddComponent<Rigidbody>();
        if (rightRb == null) rightRb = rightPart.AddComponent<Rigidbody>();

        Vector3 forwardForce = transform.forward * forwardSpeed * forwardRetentionFactor;
        Vector3 leftForce = -transform.right * splitForce + forwardForce;
        Vector3 rightForce = transform.right * splitForce + forwardForce;

        leftRb.AddForce(leftForce, ForceMode.Impulse);
        rightRb.AddForce(rightForce, ForceMode.Impulse);

        leftPart.AddComponent<BS_EggPartHandler>().Initialize(leftRb, dragOverTime);
        rightPart.AddComponent<BS_EggPartHandler>().Initialize(rightRb, dragOverTime);

        Destroy(gameObject);
    }
}
