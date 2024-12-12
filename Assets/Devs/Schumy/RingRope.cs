using UnityEngine;

public class RingRope : MonoBehaviour
{
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Transform controlPoint1;
    private Transform controlPoint2;
    private float ropeRadius;
    private int segments;
    private float returnSpeed;

    private MeshFilter meshFilter;
    private bool isInteracting;
    private Transform interactingPlayer;
    private BoxCollider ropeCollider;

    public void Initialize(Vector3 startPoint, Vector3 endPoint, float ropeRadius, Material ropeMaterial, int segments, float returnSpeed)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.ropeRadius = ropeRadius;
        this.segments = segments;
        this.returnSpeed = returnSpeed;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = ropeMaterial;
        transform.position = (startPoint + endPoint) / 2;

        
        GameObject controlPoint1Object = new ("ControlPoint1");
        controlPoint1 = controlPoint1Object.transform;
        controlPoint1.position = startPoint;

        GameObject controlPoint2Object = new ("ControlPoint2");
        controlPoint2 = controlPoint2Object.transform;
        controlPoint2.position = endPoint;

        ropeCollider = gameObject.AddComponent<BoxCollider>();
        ropeCollider.isTrigger = true;

        UpdateMesh();
        GenerateCollider();

        lineDirection = (endPoint - startPoint).normalized;
        parentPosition = transform.parent.position;
    }

    private Vector3 parentPosition;
    private Vector3 lineDirection;
    private void Update()
    {
if (isInteracting && interactingPlayer != null)
{

    Vector3 playerPosition = interactingPlayer.position;

    Vector3 closestPointOnLine = startPoint + Vector3.Project(playerPosition - startPoint, lineDirection);
    float distanceToLine = Vector3.Distance(playerPosition, closestPointOnLine);
    
    float playerPositionFactor = Vector3.Dot(playerPosition - startPoint, lineDirection) / Vector3.Distance(startPoint, endPoint);

    float curvatureOffset = 4.0f; 
    Vector3 curvatureAdjustment = lineDirection * curvatureOffset;
    
    float controlPoint1Weight = Mathf.Clamp01(playerPositionFactor);    
    float controlPoint2Weight = Mathf.Clamp01(1 - playerPositionFactor); 
    bool isXAxis = Mathf.Abs(lineDirection.x) > Mathf.Abs(lineDirection.z);
    if (isXAxis)
    {
        float baseOffset = (parentPosition.z < playerPosition.z) ? 1.0f : -1.0f;
        float totalOffset = baseOffset + (distanceToLine / 2.0f);
        controlPoint1.position = new Vector3(
            playerPosition.x,
            controlPoint1.position.y,
            playerPosition.z + (totalOffset * controlPoint1Weight)
        ) + curvatureAdjustment * controlPoint1Weight;

        controlPoint2.position = new Vector3(
            playerPosition.x,
            controlPoint2.position.y,
            playerPosition.z + (totalOffset * controlPoint2Weight)
        ) - curvatureAdjustment * controlPoint2Weight;
    }
    else
    {
        float baseOffset = (parentPosition.x < playerPosition.x) ? 1.0f : -1.0f;
        float totalOffset = baseOffset + (distanceToLine / 2.0f);

        controlPoint1.position = new Vector3(
            playerPosition.x + (totalOffset * controlPoint1Weight),
            controlPoint1.position.y,
            playerPosition.z
        ) + curvatureAdjustment * controlPoint1Weight;

        controlPoint2.position = new Vector3(
            playerPosition.x + (totalOffset * controlPoint2Weight),
            controlPoint2.position.y,
            playerPosition.z
        ) - curvatureAdjustment * controlPoint2Weight;
    }
}





        else
        {
            controlPoint2.position = Vector3.Lerp(controlPoint1.position, startPoint, returnSpeed * Time.deltaTime);
            controlPoint1.position = Vector3.Lerp(controlPoint2.position, endPoint, returnSpeed * Time.deltaTime);
        }

        UpdateMesh();

        if (isInteracting && Input.GetKeyDown(KeyCode.E))
            ReleaseInteraction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInteracting)
        {
            interactingPlayer = other.transform;
            isInteracting = true;
        }
    }

    public void ReleaseInteraction()
    {
        interactingPlayer = null;
        isInteracting = false;
    }

    private void UpdateMesh()
    {
        Vector3[] bezierPoints = CalculateBezierCurve(startPoint, controlPoint1.position, controlPoint2.position, endPoint, segments);

        Mesh mesh = new();
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

    private void GenerateCollider()
    {
        float length = Vector3.Distance(startPoint, endPoint);
        ropeCollider.size = new Vector3(ropeRadius * 2, ropeRadius * 2, length);

        Vector3 direction = (endPoint - startPoint).normalized;
        ropeCollider.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private Vector3[] CalculateBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int segments)
    {
        Vector3[] points = new Vector3[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            points[i] = Mathf.Pow(1 - t, 3) * p0 +
                        3 * Mathf.Pow(1 - t, 2) * t * p1 +
                        3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
                        Mathf.Pow(t, 3) * p3;
        }
        return points;
    }

    private void OnDrawGizmos()
    {
        if (controlPoint1 != null && controlPoint2 != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPoint, controlPoint1.position);
            Gizmos.DrawLine(controlPoint1.position, controlPoint2.position);
            Gizmos.DrawLine(controlPoint2.position, endPoint);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(controlPoint1.position, 0.1f);
            Gizmos.DrawSphere(controlPoint2.position, 0.1f);
            
            // Calculer la position centrale de la corde
            Vector3 centerPoint = (startPoint + endPoint) / 2;

            // Calculer la direction vers le parent
            Vector3 parentPosition = transform.parent.position;
            Vector3 directionToParent = (parentPosition - centerPoint).normalized;

            // Identifier l'axe principal de la corde
            Vector3 lineDirection = (endPoint - startPoint).normalized;
            bool isXAxis = Mathf.Abs(lineDirection.x) > Mathf.Abs(lineDirection.z);

            // Calculer la direction perpendiculaire
            Vector3 perpendicularDirection = isXAxis ? Vector3.forward : Vector3.right;

            // Ajuster la direction perpendiculaire pour pointer vers le parent
            perpendicularDirection *= Mathf.Sign(Vector3.Dot(directionToParent, perpendicularDirection));

            // Dessiner la corde
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPoint, endPoint);

            // Dessiner la ligne perpendiculaire
            Gizmos.color = Color.green;
            Gizmos.DrawLine(centerPoint, centerPoint + (perpendicularDirection * 3f)); // Ligne de longueur 3
        }
    }
}
