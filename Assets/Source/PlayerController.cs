using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Player maximum movement speed")]
    [SerializeField]
    private float movementSpeed = 0.05f;
    [Tooltip("The time it takes for the player to accelerate and decelerate. Larger values mean slower and smoother transition - smaller value for snappier response")]
    [SerializeField]
    private float movementAcceleration = 0.1f;
    [Tooltip("The amount of force for player's jump ability, triggered with 'Space' by default.")]
    [SerializeField]
    private float jumpForce = 250;
    [Tooltip("Toggle to give the player the ability to double jump.")]
    [SerializeField]
    private bool doubleJump;
    [Tooltip("Mask of layers the player can walk on.")]
    [SerializeField]
    private LayerMask collisionMask;
    [Tooltip("Reference to the Rigidbody component that runs the player's physics.")]
    [SerializeField]
    private Rigidbody2D physicsRB;
    [Tooltip("Toggle to give the player the ability to control its movement while in the air.")]
    [SerializeField]
    private bool airControl = true;
    [Tooltip("The amount of 'Coyote Time' or 'Hang Time'. When the player leaves and edge, they will hang around in the air and still be able to jump for this amount of time.")]
    [SerializeField]
    private float coyoteTime = 0.2f;
    [SerializeField]
    [Tooltip("Player's graphical transform - used for flipping the character's facing direction.")]
    private Transform graphics;
    [Tooltip("Reference to the player's Animator component")]
    [SerializeField]
    private Animator animator;
    [Tooltip("Reference to a transform object that resembles the center of the player. Used to check whether there's a wall near the player.")]
    [SerializeField]
    private Transform center;
    [Tooltip("When the player gets closer than this to an obstacle, they can't move further towards that direction.")]
    [SerializeField]
    private float movementLimitRange = 0.5f;
    [Tooltip("Reference to the players audio component.")]
    [SerializeField]
    private PlayerAudio playerAudio;

    private const float c_animationSpeedMultiplier = 20f;
    private const float c_movementSpeedMagicNumber = 0.01f;

    private bool grounded;
    private int jumpAmount;
    private Vector3 movementVector;
    private Vector3 movementVectorSpeed;
    private float gravity;
    private float currentCoyoteTime;
    private bool flipped;
    private Vector3 graphicsScale;
    private bool controlsDisabled;
    private GameObject occupiedObject;
    private bool onPlatform;

    public GameObject damageArea;


    private void Reset()
    {
        physicsRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAudio = GetComponent<PlayerAudio>();
    }

    private void Start()
    {
        gravity = physicsRB.gravityScale;
        currentCoyoteTime = coyoteTime;
        graphicsScale = graphics.localScale;
    }

    private void Update()
    {
        CheckIfGrounded();
        UpdateMovement();
    }

    private void CheckIfGrounded()
    {
        var wasGrounded = grounded;
        var hit = Physics2D.Raycast(transform.position, Vector3.down, 0.1f, collisionMask);
        if (hit.collider != null)
        {
            grounded = true;
            if (occupiedObject == null ||  hit.collider.gameObject != occupiedObject)
            {
                occupiedObject = hit.collider.gameObject;
                var platform = occupiedObject.GetComponent<MovingPlatformController>();
                onPlatform = platform != null;
                if (onPlatform)
                {
                    transform.SetParent(occupiedObject.transform, true);
                }
            }    
        }
        else
        {
            if (occupiedObject != null)
                occupiedObject = null;

            if (onPlatform)
            {
                onPlatform = false;
                transform.SetParent(null, true);
            }    
            grounded = false;
        }

        if (grounded)
        {
            if (!wasGrounded)
            {
                if (doubleJump)
                    jumpAmount = 0;
                physicsRB.gravityScale = 0f;
                currentCoyoteTime = coyoteTime;
                animator.SetBool("Jump", false);
            }
        }
        else
        {
            if (currentCoyoteTime > 0f)
                currentCoyoteTime -= Time.deltaTime;
            else
                ResetGravity();
        }
    }

    private Vector2 CheckMovementLimits()
    {
        var vector = new Vector2(-1f, 1f);

        if (Physics2D.Raycast(center.position, Vector3.left, movementLimitRange, collisionMask))
        {
            vector.x = 0f;
        }
        if (Physics2D.Raycast(center.position, Vector3.right, movementLimitRange, collisionMask))
        {
            vector.y = 0f;
        }

        return vector;
    }

    private void FixedUpdate()
    {
        transform.position += movementVector * movementSpeed * c_movementSpeedMagicNumber;
    }

    private void UpdateMovement()
    {
        var inputVector = Vector3.zero;

        if (!controlsDisabled)
        {
            var inputX = Input.GetAxisRaw("Horizontal");

            inputVector = Vector3.right * inputX;
        }

        var movementLimits = CheckMovementLimits();
        inputVector.x = Mathf.Clamp(inputVector.x, movementLimits.x, movementLimits.y);

        if (!airControl && grounded || airControl)
            movementVector = Vector3.SmoothDamp(movementVector, inputVector, ref movementVectorSpeed, movementAcceleration);

        if (Input.GetButtonDown("Jump"))
        {
            if (CanJump())
            {
                ResetGravity();
                physicsRB.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
                jumpAmount++;
                animator.SetBool("Jump", true);
                playerAudio.PlayAudioEvent(PlayerAudio.AudioEventTypes.Jump);
                currentCoyoteTime = 0f;
            }
        }

        if (movementVector.x < 0f)
        {
            if (!flipped)
                ToggleFlipGraphics(true);
        }
        else
        {
            if (flipped)
                ToggleFlipGraphics(false);
        }
        if (Input.GetButtonDown("Fire"))
        {
            animator.SetTrigger("attack");
            Attack();

        }
        
        else if (Input.GetButtonUp("Fire"))
        {
            animator.ResetTrigger("attack");
            
        } 

        var speedX = Mathf.Abs(movementVector.x) * c_animationSpeedMultiplier;
        animator.SetFloat("SpeedX", speedX);
    }


    private void Attack()
    {
        damageArea.SetActive(true);
        Invoke("DisableAttack", 0.1f);

    }
    private void DisableAttack()
    {
        damageArea.SetActive(false);
    }



    private void ToggleFlipGraphics(bool toggle)
    {
        flipped = toggle;
        var newScaleX = toggle ? -graphicsScale.x : graphicsScale.x;
        var newScale = graphicsScale;
        newScale.x = newScaleX;

        graphics.localScale = newScale;
    }

    private bool CanJump()
    {
        if (!doubleJump)
            return grounded || currentCoyoteTime > 0f;

        if (grounded || currentCoyoteTime > 0f)
            return true;

        return jumpAmount == 1;
    }

    public Rigidbody2D GetPhysicsRB()
    {
        return physicsRB;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public void ResetMovement()
    {
        movementVector = Vector3.zero;
    }

    public void DisableControls(float time)
    {
        controlsDisabled = true;
        StartCoroutine(EnableControlsRoutine(time));
    }

    private IEnumerator EnableControlsRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        controlsDisabled = false;
    }

    public PlayerAudio GetPlayerAudio()
    {
        return playerAudio;
    }

    public void ResetGravity()
    {
        physicsRB.gravityScale = gravity;
    }

    public bool IsGrounded()
    {
        return grounded;
    }
}
