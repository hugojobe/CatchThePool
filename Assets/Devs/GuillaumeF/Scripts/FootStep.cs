using UnityEngine;

public class FootStep : MonoBehaviour
{
    public Transform body = default;
    public FootStep otherFoot;
    public float speed;
    public float stepDistance;
    public float stepLength;
    public float stepHeight;

    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    float lerp;

    private void Start()
    {
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;

        lerp = 1;
    }

    // Update is called once per frame

    void Update()
    {
        transform.position = currentPosition;


        Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit info, 10))
        {
            if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = info.point + (body.forward * stepLength * direction);
            }
        }

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = tempPosition;
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.5f);
    }



    public bool IsMoving()
    {
        return lerp < 1;
    }
}
