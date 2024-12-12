using Unity.Cinemachine;
using UnityEngine;

public class RingRope : MonoBehaviour
{
    private Vector3 startPoint;
    private Vector3 interactionPoint;
    private Vector3 endPoint; 
    private Transform controlPoint; 
    private float ropeRadius;
    private Material ropeMaterial;
    private int segments;
    private float returnSpeed;

    private MeshFilter meshFilter;
    private bool isInteracting = false; 
    private Transform interactingPlayer; 
    private BoxCollider ropeCollider; 

    public void Initialize(Vector3 startPoint, Vector3 endPoint, float ropeRadius, Material ropeMaterial, int segments, float returnSpeed)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.ropeRadius = ropeRadius;
        this.ropeMaterial = ropeMaterial;
        this.segments = segments;
        this.returnSpeed = returnSpeed;
        
        meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = ropeMaterial;

        GameObject controlPointObject = new GameObject("ControlPoint");
        controlPoint = controlPointObject.transform;
        controlPoint.position = (startPoint + endPoint) / 2;
        transform.position = controlPoint.position;
        ropeCollider = gameObject.AddComponent<BoxCollider>();
        ropeCollider.isTrigger = true; 

        GenerateBezierMesh();
        UpdateCollider();
    }
    public float t = 0.5f;

    private void Update()
    {
        if (isInteracting && interactingPlayer != null)
        {
            float t = 0.5f;
            float oneMinusT = 1 - t;
            float weightStart = oneMinusT * oneMinusT; 
            float weightEnd = t * t; 
            float weightControl = 2 * oneMinusT * t; 
            

            Vector3 controlPosition = (interactingPlayer.position - (weightStart * startPoint) - (weightEnd * endPoint)) / weightControl;
            controlPosition.y = interactingPlayer.position.y;
            controlPoint.position = controlPosition;

        }
        else
        {
            Vector3 targetPosition = (startPoint + endPoint) / 2;
            controlPoint.position = Vector3.Lerp(controlPoint.position, targetPosition, returnSpeed * Time.deltaTime);
        }

        GenerateBezierMesh();

        if (isInteracting && Input.GetKeyDown(KeyCode.E))
            ReleaseInteraction();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInteracting)
        {
            
            interactingPlayer = other.transform;
            isInteracting = true;
            
            interactionPoint = other.transform.position;
        }
    }
    

    private void ReleaseInteraction()
    {
        interactingPlayer = null;
        isInteracting = false;
    }

    private void GenerateBezierMesh()
    {
        Vector3[] bezierPoints = CalculateBezierCurve(startPoint, controlPoint.position, endPoint, segments);

        Mesh mesh = new ();
        Vector3[] vertices = new Vector3[bezierPoints.Length * 8];
        Vector3[] normals = new Vector3[vertices.Length];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(bezierPoints.Length - 1) * 8 * 6];

        for (int i = 0; i < bezierPoints.Length; i++)
        {
            Vector3 localPoint = transform.InverseTransformPoint(bezierPoints[i]);

            Vector3 forward = (i < bezierPoints.Length - 1)
                ? (transform.InverseTransformPoint(bezierPoints[i + 1]) - localPoint).normalized
                : (localPoint - transform.InverseTransformPoint(bezierPoints[i - 1])).normalized;

            Vector3 up = Vector3.up;
            Vector3 right = Vector3.Cross(forward, up).normalized;

            for (int j = 0; j < 8; j++)
            {
                float angle = j * Mathf.PI * 2 / 8;
                Vector3 offset = right * (Mathf.Cos(angle) * ropeRadius) + up * (Mathf.Sin(angle) * ropeRadius);
                vertices[i * 8 + j] = localPoint + offset;
                normals[i * 8 + j] = offset.normalized;
                uvs[i * 8 + j] = new Vector2(j / 8f, i / (float)(bezierPoints.Length - 1));
            }
        }

        int triangleIndex = 0;
        for (int i = 0; i < bezierPoints.Length - 1; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                int current = i * 8 + j;
                int next = i * 8 + (j + 1) % 8;
                int nextRow = current + 8;
                int nextRowNext = next + 8;

                triangles[triangleIndex++] = current;
                triangles[triangleIndex++] = nextRow;
                triangles[triangleIndex++] = next;

                triangles[triangleIndex++] = next;
                triangles[triangleIndex++] = nextRow;
                triangles[triangleIndex++] = nextRowNext;
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;
    }

    private Vector3 direction;
    private void UpdateCollider()
    {
        float length = Vector3.Distance(startPoint, endPoint);
        ropeCollider.size = new Vector3(ropeRadius * 2, ropeRadius * 2, length);

        direction = (endPoint - startPoint).normalized;
        ropeCollider.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private Vector3[] CalculateBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, int segments)
    {
        Vector3[] points = new Vector3[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            points[i] = Mathf.Pow(1 - t, 2) * p0 +
                        2 * (1 - t) * t * p1 +
                        Mathf.Pow(t, 2) * p2;
        }
        return points;
    }

    private void OnDrawGizmos()
    {
        if (startPoint != null && endPoint != null && controlPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPoint, controlPoint.position);
            Gizmos.DrawLine(controlPoint.position, endPoint);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(controlPoint.position, 0.1f);
        }
    }
}
