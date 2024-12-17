using DG.Tweening;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;


public class RingRope : MonoBehaviour
{
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Transform controlPoint1;
    private Transform controlPoint2;
    private float ropeRadius;
    private int segments;
    private Renderer ropeRenderer;

    private MeshFilter meshFilter;
    private bool isInteracting;
    private Transform interactingPlayer;
    private BoxCollider ropeCollider;
    private Vector3 previousController1;
    private Vector3 previousController2;
    private bool firstGen = false;

    public Vector3 perpendicularDirection;

    public void Initialize(Vector3 startPoint, Vector3 endPoint, float ropeRadius, Material ropeMaterial, int segments, float returnSpeed)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.ropeRadius = ropeRadius;
        this.segments = segments;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        ropeRenderer = meshRenderer;
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
        mpb = new MaterialPropertyBlock();

        UpdateMesh();
        GenerateCollider();

        lineDirection = (endPoint - startPoint).normalized;
        parentPosition = transform.parent.position;
        
        Vector3 directionToParent = (parentPosition - (startPoint + endPoint) / 2).normalized;
        
        bool isXAxis = Mathf.Abs(lineDirection.x) > Mathf.Abs(lineDirection.z);

        perpendicularDirection = isXAxis ? Vector3.forward : Vector3.right;

        perpendicularDirection *= Mathf.Sign(Vector3.Dot(directionToParent, perpendicularDirection));
    }
    public float curvatureOffset = 4.0f; 
    private Vector3 curvatureAdjustment;
    private bool isXAxis;
    private GameObject plane;
    private Renderer planeRenderer;
    private MaterialPropertyBlock mpb;
    public void BorderInit(Material planeMaterial,float borderslide)
    {
        // Créer le plane
        plane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        plane.name = "RopePlane";
        plane.transform.SetParent(transform);

        // Positionner le plane sous la corde
        plane.transform.localRotation = Quaternion.Euler(90, 90, 0); // Plan horizontal
        plane.transform.localPosition = new Vector3(0, -borderslide, 0); // Juste sous la corde

        // Ajuster la taille du plane pour correspondre au BoxCollider
        plane.transform.localScale = new Vector3(ropeCollider.size.z, ropeCollider.size.x/100, 1);

        planeRenderer = plane.GetComponent<Renderer>();
        planeRenderer.material = planeMaterial;
        isXAxis = Mathf.Abs(lineDirection.x) > Mathf.Abs(lineDirection.z);
        curvatureAdjustment = lineDirection * curvatureOffset;
        Destroy(plane.GetComponent<Collider>());
    }


    public float offsetter = 1.5f;
    private Vector3 parentPosition;
    private Vector3 lineDirection;
    private void Update()
    {
        if (isInteracting && interactingPlayer != null)
        {
            previousController1 = controlPoint1.position;
            previousController2 = controlPoint2.position;
            
            Vector3 playerPosition = interactingPlayer.position;

            Vector3 closestPointOnLine = startPoint + Vector3.Project(playerPosition - startPoint, lineDirection);
            float distanceToLine = Vector3.Distance(playerPosition, closestPointOnLine);
    
            float playerPositionFactor = Vector3.Dot(playerPosition - startPoint, lineDirection) / Vector3.Distance(startPoint, endPoint);
            
           float controlPoint1Weight = Mathf.Clamp01(playerPositionFactor);    
           float controlPoint2Weight = Mathf.Clamp01(1 - playerPositionFactor); 
           if (isXAxis)
           { 
               float baseOffset = (parentPosition.z < playerPosition.z) ? 1 : -1; 
               float totalOffset = baseOffset * offsetter + baseOffset * (distanceToLine / 2.0f);
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
                float baseOffset = (parentPosition.x < playerPosition.x) ? 1 : -1;
                float totalOffset = baseOffset * offsetter + baseOffset * (distanceToLine / 2.0f);

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

        if (previousController1 != controlPoint1.position || previousController2 != controlPoint2.position)
            UpdateMesh();

        if (isInteracting && Input.GetKeyDown(KeyCode.E))
            ReleaseInteraction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isInteracting) return;
        interactingPlayer = other.transform;
        isInteracting = true;
        mpb.SetFloat("_Emiss",3);
        ropeRenderer.SetPropertyBlock(mpb);
        if (planeRenderer == null) return;
        mpb.SetFloat("_BorderActiv",1);
        planeRenderer.SetPropertyBlock(mpb);
    }

    public void ReleaseInteraction()
    {
        interactingPlayer = null;
        isInteracting = false;
        Vector3 closestPointOnLine;
        closestPointOnLine = startPoint + Vector3.Project(controlPoint1.position - startPoint, lineDirection);

        DOTween.To(() => controlPoint1.position, x =>
        {
            controlPoint1.position = x;
        }, closestPointOnLine, 0.3f).SetEase(Ease.OutElastic);
            
        closestPointOnLine = startPoint + Vector3.Project(controlPoint2.position - startPoint, lineDirection);
        DOTween.To(() => controlPoint2.position, x =>
        {
            controlPoint2.position = x;
        }, closestPointOnLine, 0.3f).SetEase(Ease.OutElastic);
        previousController1 = controlPoint1.position;
        previousController2 = controlPoint2.position;
        mpb.SetFloat("_Emiss",0);
        ropeRenderer.SetPropertyBlock(mpb);
        if (planeRenderer == null) return;
        mpb.SetFloat("_BorderActiv",0);
        planeRenderer.SetPropertyBlock(mpb);
    }

    private void UpdateMesh()
    {
        Debug.Log("UpdateMesh");
        
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
        ropeCollider.size = new Vector3(ropeRadius * 100, ropeRadius * 4, length - 0.5f);

        Vector3 direction = (endPoint - startPoint).normalized;
        ropeCollider.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        
        
        ropeCollider.center = new(ropeCollider.center.x - 2.5f, ropeCollider.center.y, ropeCollider.center.z);
        
        ropeCollider.gameObject.tag = "Rope";
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
    
}
