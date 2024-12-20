using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
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

    public float moveSpeed;
    public Vector2 moveInput;
    public Vector3 previousFrameVelocity;

    public bool dashCooldownElapsed;
    public Coroutine dashCoroutine;
    
    private Coroutine damagePhysicsCoroutine;

    private List<RingRope> ringRopes = new();
    public Vector3 ropeEnterPosition;
    public float maxRopePullRadius = 0.5f;
    public float maxRopePullAngle = 80f;
    public Vector3 currentRopeNormalVector;
    public SpriteRenderer ropePullArrow;
    public float arrowSizeMin = 0.95f, arrowSizeMax = 3f;
    
    public bool abilityCooldownElapsed;

    public MaterialPropertyBlock circleMpb;
    public Renderer circleRend;
    public float circlePercent = 1;

    public Animator animator;
    
    public GameObject trailInstance;
    public GameObject flameInstance;
    
    public Coroutine abilityCoroutine;

    [Space] 
    public Transform spicyfartDamagePoint;
    
    [Space]
    public PlayerFeathers feathers;

    public void Initialize()
    {
        chickenColor = GameInstance.instance.playerColors[index];
        
        GameObject playerMesh = Instantiate(chickenConfig.chickenMeshPrefab, transform.position, Quaternion.identity, transform);
        playerMesh.transform.localPosition = new Vector3(0, -0.339f, 0);
        
        feedbackMachine = GetComponent<FeedbackMachine>();
        damageable = GetComponent<Damageable>();
        
        feedbackMachine.pc = this;
        damageable.currentHealth = chickenConfig.chickenHealthGameplay;
        
        GameInstance.instance.playerControllers.Add(this);

        moveSpeed = chickenConfig.chickenSpeed;

        playerState = PlayerState.Uncontrolled;
        dashCooldownElapsed = true;
        abilityCooldownElapsed = true;
        
        damageable.playerController = this;
        
        damageable.OnDamageTaken += OnDamageTaken;
        damageable.OnDamageTaken += feedbackMachine.OnDamageTaken;
        
        damageable.OnDeath += OnDeath;
        damageable.OnDeath += feedbackMachine.OnDeath;
        
        animator = GetComponentInChildren<Animator>();
        
        chickenConfig.ability.InitAbility(this);
        
        ropePullArrow.transform.parent.gameObject.SetActive(false);
        
        circleMpb = new MaterialPropertyBlock();
        circleMpb.SetColor("_PlayerColor", chickenColor);
        circleRend.SetPropertyBlock(circleMpb);
        
        feathers = GetComponentInChildren<PlayerFeathers>();
        
        isInitialized = true;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if(context.control.device.deviceId != gamepadID || !CanMove())
            return;

        moveInput = context.ReadValue<Vector2>();

        if (moveInput.magnitude > 0.1f && !isCloseToAnyPlayer && playerState != PlayerState.Dashing && playerState != PlayerState.RopePull)
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

        PlayerState oldState = playerState;
        
        watchRotation = Quaternion.LookRotation(rb.linearVelocity.normalized, transform.up).eulerAngles;
        
        dashCooldownElapsed = false;
        playerState = PlayerState.Dashing;
        rb.linearVelocity = Vector3.zero;
        
        if (oldState == PlayerState.RopePull)
        {
            dashCoroutine = StartCoroutine(DashCoroutine((ropeEnterPosition - transform.position).normalized, 3f, false));
            ropePullArrow.transform.parent.gameObject.SetActive(false);
            ReleaseRopesWithoutDelay();
        }
        else
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
        
        if (!isSpicyfart)
            feedbackMachine.OnDashStarted();
        else
            feedbackMachine.OnSpicyfartStarted();
        
        /*if (Physics.Raycast(transform.position, dashDirection, out RaycastHit hit, dashDistance))
        {
            dashDistance = hit.distance;
            dashTime = dashDistance / dashSpeed;
        }*/

        rb.linearVelocity = dashDirection * dashSpeed;

        float elapsedTime = 0f;
        while (elapsedTime < dashTime)
        {
            elapsedTime += Time.deltaTime;

            if(isSpicyfart)
                feedbackMachine.OnSpicyFartUpdate();
            else
                feedbackMachine.OnDashUpdate(elapsedTime / dashTime);
            
            yield return null;
        }

        rb.linearVelocity = Vector3.zero;
        moveSpeed = chickenConfig.chickenSpeed;

        yield return new WaitForEndOfFrame();
        
        playerState = PlayerState.Normal;

        if (isSpicyfart)
        {
            feedbackMachine.OnSpicyfartEnded();
            animator.SetTrigger("EndState");
            trailInstance.GetComponent<TrailRenderer>().emitting = false;
            flameInstance.GetComponent<ParticleSystem>().Stop();
            StartCoroutine(ReloadAbilityCoroutine());
        }
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
            playerState != PlayerState.RopePull &&
            playerState != chickenConfig.abilityState &&
            abilityCooldownElapsed;
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
            RingRope ringRope = other.gameObject.GetComponent<RingRope>();
            
            ringRopes.Add(ringRope);
            
            if (other.transform.childCount == 0)
                return;
            
            if (playerState == PlayerState.Dashing || playerState == PlayerState.Spicyfart)
            {
                Debug.Log("Dashing");
                
                Vector3 reflectVelocity = Vector3.Reflect(previousFrameVelocity, ringRope.perpendicularDirection.normalized);
                
                Debug.DrawLine(transform.position, transform.position + reflectVelocity, Color.red, 2f);
                
                moveInput = reflectVelocity.normalized;
                
                if (dashCoroutine != null)
                {
                    StopCoroutine(dashCoroutine);
                    dashCoroutine = null;

                    if (playerState == PlayerState.Dashing)
                        Invoke(nameof(ResetDashCooldown), chickenConfig.dashCooldown);
                    else
                    {
                        feedbackMachine.OnSpicyfartEnded();
                        animator.SetTrigger("EndState");
                        trailInstance.GetComponent<TrailRenderer>().emitting = false;
                        flameInstance.GetComponent<ParticleSystem>().Stop();
                        StartCoroutine(ReloadAbilityCoroutine());
                    }
                    
                    playerState = PlayerState.Normal;
                    
                    feedbackMachine.OnDashFinished();
                }
                dashCooldownElapsed = false;
                
                if(dashCoroutine != null)
                    StopCoroutine(dashCoroutine);
                
                playerState = PlayerState.Dashing;
                dashCoroutine = StartCoroutine(DashCoroutine(reflectVelocity.normalized, 3f));

                StartCoroutine(ReleaseRopes());
            }
            else
            {
                playerState = PlayerState.RopePull;
                animator.SetTrigger("PushRope");
                ropeEnterPosition = transform.position;
                currentRopeNormalVector = -ringRope.perpendicularDirection.normalized;
                
                ropePullArrow.transform.parent.gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator ReloadAbilityCoroutine()
    {
        StartCoroutine(SetCirclePercent());
        yield return new WaitForSecondsRealtime(chickenConfig.abilityCooldown);

        abilityCooldownElapsed = true;
    }

    private IEnumerator ReleaseRopes()
    {
        yield return new WaitForSeconds(0.05f);
        
        ringRopes.ForEach(r => r.ReleaseInteraction());
        ringRopes.Clear();
    }
    
    public void ReleaseRopesWithoutDelay()
    {
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

    public void ForceFinishDash()
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

    private void Update()
    {
        if(!isInitialized)
            return;
        
        Lebug.Log($"P{index} health:", damageable.currentHealth, $"Player {index}");
        Lebug.Log($"P{index} state:", playerState.ToString(), $"Player {index}");
        Lebug.Log($"P{index} ability cd:", Mathf.RoundToInt(circlePercent*100) + "%", $"Player {index}");
        
        if(playerState != PlayerState.Dead && playerState != PlayerState.Uncontrolled && playerState != chickenConfig.abilityState)
            RotateTowardsDirection();
        
        //DrawLocalAxes();
        
        previousFrameVelocity = rb.linearVelocity;
        
        if(playerState != PlayerState.Dashing && playerState != PlayerState.Dead && playerState != PlayerState.Uncontrolled && playerState != PlayerState.RopePull)
            rb.linearVelocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        else if (playerState == PlayerState.Dead)
            rb.linearVelocity = Vector3.zero;
        else if(playerState == PlayerState.RopePull)
        {
            Vector3 inputVelocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
            Vector3 newPosition = rb.position + inputVelocity * Time.fixedDeltaTime;

            Vector3 pullOffset = newPosition - ropeEnterPosition;
            float pullDistance = pullOffset.magnitude;

            if (pullDistance > maxRopePullRadius)
            {
                pullOffset = pullOffset.normalized * maxRopePullRadius;
                newPosition = ropeEnterPosition + pullOffset;
            }

            float angle = Vector3.Angle(currentRopeNormalVector, pullOffset);

            if (angle > maxRopePullAngle)
            {
                float signedAngle = Vector3.SignedAngle(currentRopeNormalVector, pullOffset, Vector3.up);
                float clampedAngle = Mathf.Sign(signedAngle) * maxRopePullAngle;

                Vector3 clampedDirection = Quaternion.AngleAxis(clampedAngle, Vector3.up) * currentRopeNormalVector;
                pullOffset = clampedDirection.normalized * pullOffset.magnitude;
                newPosition = ropeEnterPosition + pullOffset;
            }

            rb.position = newPosition;
            ropePullArrow.size = new Vector2(1f, Mathf.Lerp(arrowSizeMin, arrowSizeMax, pullDistance / maxRopePullRadius));
            ropePullArrow.transform.parent.rotation = Quaternion.LookRotation(ropeEnterPosition - transform.position, ropePullArrow.transform.parent.up);
        }

        if (playerState != PlayerState.Dead)
        {
            circleMpb.SetFloat("_Cooldown", circlePercent);
            circleRend.SetPropertyBlock(circleMpb);
        }
    }

    public IEnumerator SetCirclePercent()
    {
        float currentCooldown = 0;
        circlePercent = 0;
        
        while(currentCooldown < chickenConfig.abilityCooldown)
        {
            currentCooldown += Time.deltaTime;
            circlePercent = currentCooldown / chickenConfig.abilityCooldown;
            yield return null;
        }
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
        
        if (playerState == PlayerState.RopePull)
        {
            ReleaseRopesWithoutDelay();
            StartCoroutine(ReleaseRopes());
            ropePullArrow.transform.parent.gameObject.SetActive(false);
        }
        
        playerState = PlayerState.Dead;
        rb.linearVelocity = Vector3.zero;
        moveInput = Vector3.zero;
        
        circleRend.enabled = false;

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
    Nuggquake,
    RopePull
}