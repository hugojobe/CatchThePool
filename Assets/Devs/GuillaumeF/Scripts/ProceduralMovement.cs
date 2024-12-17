using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class ProceduralMovement : MonoBehaviour
{
    [Header("Rig Parameters")]
    public Transform Root;

    public Transform LeftLeg;
    public Transform RightLeg;

    private Transform LeftKnee;
    private Transform RightKnee;

    private Transform LeftAnkle;
    private Transform RightAnkle;

    [Header("Hint/Pole Vector")]
    public Transform PoleVectorL;
    public Transform PoleVectorR;

    [Header("Target")]
    public Transform TargetL;
    public Transform TargetR;

    [Header("MovementParameters")]
    public float detectionRange;
    public float stepDistance = .75f;
    public float stepHeight = 1.5f;
    public float stepSpeed = 2;

    [Header("Intermediary Parameter")]
    private Vector3 currentGroundingLeftPos;
    private Vector3 currentGroundingRightPos;

    private Vector3 researchGroundingLeftPos;
    private Vector3 researchGroundingRightPos;

    private Vector3 nextLeftGroundingPos;
    private Vector3 nextRightGroundingPos;

    private void OnValidate()
    {
        if ((Root == null))
            Root = transform;

        if (LeftLeg != null)
        {
            LeftKnee = LeftLeg.GetChild(0);
            LeftAnkle = LeftKnee.GetChild(0);
        }

        if (RightLeg != null)
        {
            RightKnee = RightLeg.GetChild(0);
            RightAnkle = RightKnee.GetChild(0);
        }
    }

    private RaycastHit infos;
    private Ray leftRay;
    private Ray rightRay;
    private Ray leftNextRay;
    private Ray rightNextRay;
    private Ray rootRay;

    private float lerpR;
    private float lerpL;
    private bool isStepping;

    private bool isLStep;
    private bool isRStep;
    public bool canWalkWall;

    private void Start()
    {
        if (detectionRange <= 0)
            detectionRange = transform.position.y;

        leftRay = new Ray(LeftAnkle.position, -transform.up);
        if (Physics.Raycast(leftRay, out infos, detectionRange))
            currentGroundingLeftPos = infos.point;

        rightRay = new Ray(RightAnkle.position, -transform.up);
        if (Physics.Raycast(rightRay, out infos, detectionRange))
            currentGroundingRightPos = infos.point;


    }

    private void Update()
    {
        TargetL.position = currentGroundingLeftPos;
        TargetR.position = currentGroundingRightPos;


        leftNextRay = new Ray(LeftLeg.position, -transform.up);
        if (Physics.Raycast(leftNextRay, out infos, detectionRange))
            researchGroundingLeftPos = infos.point;

        rightNextRay = new Ray(RightLeg.position, -transform.up);
        if (Physics.Raycast(rightNextRay, out infos, detectionRange))
            researchGroundingRightPos = infos.point;


        rootRay = canWalkWall ? new Ray(transform.position, -transform.up) : new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(rootRay, out infos, detectionRange))
            transform.rotation = Quaternion.FromToRotation(transform.up, infos.normal) * transform.rotation;


        if (Vector3.Distance(currentGroundingLeftPos, researchGroundingLeftPos) > stepDistance && !isRStep)
        {
            nextLeftGroundingPos = researchGroundingLeftPos;
            if (nextLeftGroundingPos != Vector3.zero)
            {
                if (!isStepping)
                {
                    lerpL = 0;
                    isLStep = true;
                    isStepping = true;
                }

                if (lerpL < 1)
                {
                    Vector3 tempPosition = Vector3.Lerp(currentGroundingLeftPos, nextLeftGroundingPos, lerpL);
                    tempPosition.y += Mathf.Sin(lerpL * Mathf.PI) * stepHeight;

                    TargetL.position = tempPosition;
                    lerpL += Time.deltaTime * stepSpeed;

                }
                else
                {
                    currentGroundingLeftPos = nextLeftGroundingPos;
                    TargetL.position = currentGroundingLeftPos;
                    nextLeftGroundingPos = Vector3.zero;
                    isStepping = false;
                    isLStep = false;
                }
            }
        }

        if (Vector3.Distance(currentGroundingRightPos, researchGroundingRightPos) > stepDistance && !isLStep)
        {
            nextRightGroundingPos = researchGroundingRightPos;
            if (nextRightGroundingPos != Vector3.zero)
            {
                if (!isStepping)
                {
                    lerpR = 0;
                    isRStep = true;
                    isStepping = true;
                }

                if (lerpR < 1)
                {
                    Vector3 tempPosition = Vector3.Lerp(currentGroundingRightPos, nextRightGroundingPos, lerpR);
                    tempPosition.y += Mathf.Sin(lerpR * Mathf.PI) * stepHeight;

                    TargetR.position = tempPosition;
                    lerpR += Time.deltaTime * stepSpeed;

                }
                else
                {
                    currentGroundingRightPos = nextRightGroundingPos;
                    TargetR.position = currentGroundingRightPos;
                    nextRightGroundingPos = Vector3.zero;
                    isStepping = false;
                    isRStep = false;
                }
            }
        }

    }




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(researchGroundingLeftPos, .25f);
        Gizmos.DrawSphere(researchGroundingRightPos, .25f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(nextLeftGroundingPos, .25f);
        Gizmos.DrawSphere(nextRightGroundingPos, .25f);
    }



}
