using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
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
    public Color chickenColor;
    private bool isInitialized;
    public PlayerState playerState;

    public FeedbackMachine feedbackMachine;
    public Damageable damageable;

    public Vector3 watchRotation;
    public bool isCloseToAnyPlayer;

    [SerializeField] private float moveSpeed;
    public Vector2 moveInput;
    public Vector3 previousFrameVelocity;

    [SerializeField] private bool dashCooldownElapsed;
    private Coroutine dashCoroutine;
    
    private Coroutine damagePhysicsCoroutine;

    private List<RingRope> ringRopes = new();
    
    public bool abilityCooldownElapsed;

    public Animator animator;

    public void Initialize()
    {
        chickenColor = chickenConfig.chickenColors[Random.Range(0, chickenConfig.chickenColors.Length)];
        
        GameObject playerMesh = Instantiate(chickenConfig.chickenMeshPrefab, transform.position, Quaternion.identity, transform);
        playerMesh.transform.localPosition = new Vector3(0, -0.339f, 0);
        
        feedbackMachine = GetComponent<FeedbackMachine>();
        damageable = GetComponent<Damageable>();
        
        feedbackMachine.pc = this;
        damageable.currentHealth = chickenConfig.chickenHealthGameplay;
        
        GameInstance.instance.playerControllers.Add(this);

        moveSpeed = chickenConfig.chickenSpeed;

        playerState = PlayerState.Normal;
        dashCooldownElapsed = true;
        
        damageable.playerController = this;
        
        damageable.OnDamageTaken += OnDamageTaken;
        damageable.OnDamageTaken += feedbackMachine.OnDamageTaken;
        
        damageable.OnDeath += OnDeath;
        damageable.OnDeath += feedbackMachine.OnDeath;
        
        animator = GetComponentInChildren<Animator>();
        
        chickenConfig.ability.InitAbility(this);
        
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
        if(context.control.device.deviceId != gamepadID || !CanDash() || context.phase != InputActionPhase.Started)
            return;

        if (moveInput == Vector2.zero)
            return;

        if(dashCoroutine != null)
            StopCoroutine(dashCoroutine);
        
        dashCooldownElapsed = false;
        playerState = PlayerState.Dashing;
        rb.linearVelocity = Vector3.zero;
        dashCoroutine = StartCoroutine(DashCoroutine(new Vector3(moveInput.x, 0, moveInput.y).normalized));
    }

    public void UseAbility(InputAction.CallbackContext context)
    {
        if(context.control.device.deviceId != gamepadID || !CanUseAbility() || context.phase != InputActionPhase.Started)
            return;
        
        chickenConfig.ability.Activate(this);
    }

    public IEnumerator DashCoroutine(Vector3 direction, float forceMultiplier = 1f, bool isSpicyfart = false)
    {
        Vector3 dashDirection = direction;
        moveInput = new Vector2(dashDirection.x, dashDirection.z);
        float dashDistance = chickenConfig.dashDistance * forceMultiplier;
        float dashSpeed = chickenConfig.dashSpeed;
        float dashTime = dashDistance / dashSpeed;

        moveSpeed = dashSpeed;
        
        if (isSpicyfart)
            feedbackMachine.OnDashStarted();
        else
            feedbackMachine.OnSpicyfartStarted();
        
        
        if (Physics.Raycast(transform.position, dashDirection, out RaycastHit hit, dashDistance))
        {
            dashDistance = hit.distance;
            dashTime = dashDistance / dashSpeed;
        }

        rb.linearVelocity = dashDirection * dashSpeed;;

        float elapsedTime = 0f;
        while (elapsedTime < dashTime)
        {
            elapsedTime += Time.fixedDeltaTime;

            if(isSpicyfart)
                feedbackMachine.OnSpicyFartUpdate();
            else
                feedbackMachine.OnDashUpdate(elapsedTime / dashTime);
            
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector3.zero;
        moveSpeed = chickenConfig.chickenSpeed;
        playerState = PlayerState.Normal;
        
        if(isSpicyfart)
            feedbackMachine.OnSpicyfartEnded();
        else
            feedbackMachine.OnDashFinished();
        
        dashCoroutine = null;

        yield return new WaitForSeconds(chickenConfig.dashCooldown);
        ResetDashCooldown();
    }

    private bool CanMove()
    {
        return
            playerState != PlayerState.Dead &&
            playerState != PlayerState.Uncontrolled &&
            playerState != PlayerState.Dashing &&
            playerState != chickenConfig.abilityState;
    }

    private bool CanDash()
    {
        return
            playerState != PlayerState.Dead &&
            playerState != PlayerState.Uncontrolled &&
            playerState != PlayerState.Dashing &&
            playerState != chickenConfig.abilityState &&
            dashCooldownElapsed;
    }

    private bool CanUseAbility()
    {
        return
            playerState != PlayerState.Dead &&
            playerState != PlayerState.Uncontrolled &&
            playerState != PlayerState.Dashing &&
            playerState != chickenConfig.abilityState;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject otherPlayer = other.gameObject;
            
            otherPlayer.TryGetComponent<PlayerController>(out PlayerController otherPC);

            if (playerState == PlayerState.Dashing && otherPC.playerState == PlayerState.Dashing)
            {
                OnDamageTaken(otherPlayer);
                return;
            }

            if (otherPC.playerState == PlayerState.Dashing)
            {
                return;
            }
            
            if(playerState == PlayerState.Dashing)
            {
                
                otherPlayer.TryGetComponent<Damageable>(out Damageable otherDamageable);
                otherDamageable.TakeDamage(gameObject);
                
                ForceFinishDash();
                
                StartCoroutine(DashDamageStopCoroutine());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Rope"))
        {
            Debug.Log("Rope");
            
            RingRope ringRope = other.gameObject.GetComponent<RingRope>();
            
            ringRopes.Add(ringRope);
            
            if (other.transform.childCount == 0)
                return;
            
            if (playerState == PlayerState.Dashing)
            {
                Debug.Log("Dashing");
                
                Vector3 reflectVelocity = Vector3.Reflect(previousFrameVelocity, ringRope.perpendicularDirection.normalized);
                
                Debug.DrawLine(transform.position, transform.position + reflectVelocity, Color.red, 2f);
                
                moveInput = reflectVelocity.normalized;
                
                if (dashCoroutine != null)
                {
                    StopCoroutine(dashCoroutine);
                    dashCoroutine = null;
                    Invoke(nameof(ResetDashCooldown), chickenConfig.dashCooldown);
                    feedbackMachine.OnDashFinished();
                }
                dashCooldownElapsed = false;
                dashCoroutine = StartCoroutine(DashCoroutine(reflectVelocity.normalized));

                StartCoroutine(ReleaseRopes());
            }
        }
    }

    private IEnumerator ReleaseRopes()
    {
        yield return new WaitForSeconds(0.05f);
        
        ringRopes.ForEach(r => r.ReleaseInteraction());
        ringRopes.Clear();
    }

    private IEnumerator DashDamageStopCoroutine()
    {
        playerState = PlayerState.Uncontrolled;
        yield return new WaitForSeconds(0.2f);
        
        if(playerState != PlayerState.Dead)
            playerState = PlayerState.Normal;
    }

    private void ForceFinishDash()
    {
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            dashCoroutine = null;
            Invoke(nameof(ResetDashCooldown), chickenConfig.dashCooldown);
            feedbackMachine.OnDashFinished();
        }

        rb.linearVelocity = Vector3.zero;
        moveSpeed = chickenConfig.chickenSpeed;
        
        if(playerState != PlayerState.Dead)
            playerState = PlayerState.Normal;
    }

    private void FixedUpdate()
    {
        if(!isInitialized)
            return;
        
        if(playerState != PlayerState.Dashing && playerState != PlayerState.Dead && playerState != PlayerState.Uncontrolled)
            rb.linearVelocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        else if (playerState == PlayerState.Dead)
            rb.linearVelocity = Vector3.zero;
    }

    private void Update()
    {
        Lebug.Log($"P{index} health:", damageable.currentHealth, $"Player {index}");
        Lebug.Log($"P{index} state:", playerState.ToString(), $"Player {index}");
        
        if(playerState != PlayerState.Dead && playerState != PlayerState.Uncontrolled && playerState != chickenConfig.abilityState)
            RotateTowardsDirection();
        
        //DrawLocalAxes();
        
        previousFrameVelocity = rb.linearVelocity;
    }

    private void RotateTowardsDirection()
    {
        Vector3 targetRotation = new Vector3(0, watchRotation.y, 0);
        transform.DORotate(targetRotation, 0.2f);
    }

    public void OnDamageTaken(GameObject damageCauser)
    {
        if (damagePhysicsCoroutine != null)
        {
            StopCoroutine(damagePhysicsCoroutine);
            rb.linearVelocity = Vector3.zero;
        }
        
        rb.linearVelocity = Vector3.zero;
        damagePhysicsCoroutine = StartCoroutine(DamagePhysicsCoroutine(damageCauser));
    }

    private IEnumerator DamagePhysicsCoroutine(GameObject damageCauser)
    {
        playerState = PlayerState.Uncontrolled;
        Vector3 knockbackDirection = damageCauser.GetComponent<Rigidbody>().linearVelocity.normalized;
        float knockbackForce = 50f;
        float knockbackDuration = 0.2f;
        float elapsedTime = 0f;

        while (elapsedTime < knockbackDuration)
        {
            rb.AddForce(knockbackDirection * (knockbackForce * Time.fixedDeltaTime), ForceMode.VelocityChange);
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if(playerState != PlayerState.Dead)
            playerState = PlayerState.Normal;
    }

    public void OnDeath()
    {
        ForceFinishDash();
        playerState = PlayerState.Dead;
        rb.linearVelocity = Vector3.zero;
        moveInput = Vector3.zero;

        GameInstance.instance.playerAlive[index] = false;
        GameInstance.instance.playerDeaths[index]++;
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
    Dead,
    Hadoukoeuf,
    Spicyfart,
    Tourbiplume,
    Nuggquake
}