using UnityEngine;
using System.Collections.Generic;

public class RingRopeManager : MonoBehaviour
{
    [Header("Ring Configuration")]
    public Transform[] pillars;
    public int ropesPerSide = 3; 
    public float ropeSpacing = 0.5f; 
    public Material ropeMaterial; 
    public float ropeRadius = 0.1f; 
    public int ropeSegments = 50;

    public float borderSlide = 3;
    public Material borderMaterial;
    [Header("Interaction Configuration")]
    public float interactionRange = 2.0f; 
    public float returnSpeed = 5.0f; 

    private List<RingRope> ropes = new List<RingRope>();
    

    private void Start()
    {
        if (pillars.Length != 4) return;
        GenerateRopes();
    }

    private void GenerateRopes()
    {
        ropes.Clear();

        for (int i = 0; i < pillars.Length; i++)
        {
            Transform startPillar = pillars[i];
            Transform endPillar = pillars[(i + 1) % pillars.Length];

            // Générer les cordes entre les deux piliers
            for (int j = 0; j < ropesPerSide; j++)
            {
                float heightOffset = j * ropeSpacing;

                Vector3 startPoint = startPillar.position + Vector3.up * heightOffset;
                Vector3 endPoint = endPillar.position + Vector3.up * heightOffset;

                GameObject ropeObject = new GameObject($"Rope_{i}_{j}");
                ropeObject.transform.SetParent(transform);

                RingRope rope = ropeObject.AddComponent<RingRope>();
                rope.Initialize(startPoint, endPoint, ropeRadius, ropeMaterial, ropeSegments, returnSpeed);

                ropes.Add(rope);
            }
            //TODO PLANE FOR VISUAL INTECACT
            ropes[^1].BorderInit(borderMaterial,borderSlide);
        }
    }


    private void OnDrawGizmos()
    {
        // Visualisation des cordes dans l'éditeur
        if (pillars.Length == 4)
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < pillars.Length; i++)
            {
                Transform startPillar = pillars[i];
                Transform endPillar = pillars[(i + 1) % pillars.Length];

                Gizmos.DrawLine(startPillar.position, endPillar.position);
            }
        }
    }
}

