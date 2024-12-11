using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public int index;
    public int gamepadID;
    public PlayerInput input;
    public Rigidbody rb;
    public ChickenConfig chickenConfig;
    private bool isInitialized;
    public PlayerState playerState;

    private FeedbackMachine feedbackMachine;

    public Vector3 watchRotation;
    public bool isCloseToAnyPlayer;

    [SerializeField] private float moveSpeed;
    public Vector2 moveInput;

    [SerializeField] private bool dashCooldownElapsed;
    private Coroutine dashCooldownCoroutine;

    public void Initialize()
    {
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = chickenConfig.chickenColors[Random.Range(0, chickenConfig.chickenColors.Length)];
        
        feedbackMachine = GetComponent<FeedbackMachine>();
        feedbackMachine.pc = this;
        
        GameInstance.instance.playerControllers.Add(this);

        moveSpeed = chickenConfig.chickenSpeed;

        playerState = PlayerState.Normal;
        dashCooldownElapsed = true;

        isInitialized = true;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if(context.control.device.deviceId != gamepadID || !CanMove())
            return;

        moveInput = context.ReadValue<Vector2>();

        if (moveInput.magnitude > 0.1f && !isCloseToAnyPlayer)
            watchRotation = Quaternion.LookRotation(rb.linearVelocity.normalized, transform.up).eulerAngles;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(context.control.device.deviceId != gamepadID || !CanDash())
            return;

        dashCooldownElapsed = false;
        playerState = PlayerState.Dashing;
        rb.linearVelocity = Vector3.zero;
        dashCooldownCoroutine = StartCoroutine(DashCoroutine());
    }

    public IEnumerator DashCoroutine()
    {
        Vector3 dashDirection = moveInput.normalized;
        float dashTime = chickenConfig.dashDistance / chickenConfig.dashSpeed;
        float elapsedTime = 0f;

        moveSpeed = chickenConfig.dashSpeed;
        
        feedbackMachine.OnDashStarted();

        if (Physics.Raycast(transform.position, dashDirection, out RaycastHit hit, chickenConfig.dashDistance))
        {
            dashTime = hit.distance / chickenConfig.dashSpeed;
        }

        while (elapsedTime < dashTime)
        {
            rb.linearVelocity = dashDirection * moveSpeed;
            elapsedTime += Time.fixedDeltaTime;
            
            feedbackMachine.OnDashUpdate(elapsedTime / dashTime);

            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector3.zero;
        moveSpeed = chickenConfig.chickenSpeed;
        playerState = PlayerState.Normal;
        
        feedbackMachine.OnDashFinished();

        yield return new WaitForSecondsRealtime(chickenConfig.dashCooldown);
        ResetDashCooldown();

        dashCooldownCoroutine = null;
    }

    private bool CanMove()
    {
        return
            playerState != PlayerState.Dead &&
            playerState != PlayerState.Uncontrolled &&
            playerState != PlayerState.Dashing;
    }

    private bool CanDash()
    {
        return
            playerState != PlayerState.Dead &&
            playerState != PlayerState.Uncontrolled &&
            playerState != PlayerState.Dashing &&
            dashCooldownElapsed;
    }

    private void FixedUpdate()
    {
        if(!isInitialized)
            return;

        DrawLocalAxes();

        rb.linearVelocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;

        rb.DORotate(watchRotation, 0.3f);
    }

    private void ResetDashCooldown()
    {
        dashCooldownElapsed = true;
    }

    private void DrawLocalAxes()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 2, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.right * 2, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + transform.up * 2, Color.green);
    }
}

public enum PlayerState
{
    Uncontrolled = 0,
    Normal,
    Dashing,
    Dead
}