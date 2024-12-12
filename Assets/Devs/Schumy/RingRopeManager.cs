using UnityEngine;
using System.Collections.Generic;

public class RingRopeManager : MonoBehaviour
{
    [Header("Ring Configuration")]
    public Transform[] pillars; // Les 4 piliers (coin du ring)
    public int ropesPerSide = 3; // Nombre de cordes par côté
    public float ropeSpacing = 0.5f; // Espacement vertical entre les cordes
    public Material ropeMaterial; // Matériau des cordes
    public float ropeRadius = 0.1f; // Rayon des cordes
    public int ropeSegments = 50; // Segments pour la courbe de Bézier

    [Header("Interaction Configuration")]
    public float interactionRange = 2.0f; // Distance maximale pour interagir avec une corde
    public float returnSpeed = 5.0f; // Vitesse de retour du point de contrôle

    private List<RingRope> ropes = new List<RingRope>();

    private void Start()
    {
        if (pillars.Length != 4)
        {
            Debug.LogError("Le ring doit avoir exactement 4 piliers !");
            return;
        }

        GenerateRopes();
    }

    private void GenerateRopes()
    {
        ropes.Clear();

        // Boucler sur chaque paire de piliers
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

