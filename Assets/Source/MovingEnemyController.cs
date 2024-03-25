using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemyController : MonoBehaviour
{
    private enum MovingDirections
    {
        Left,
        Right
    }

    [Tooltip("Maximum speed of this enemy.")]
    [SerializeField]
    private float movingSpeed;
    [SerializeField]
    [Tooltip("Reference to the transform of this enemy that resembles its top. Used for detecting walls and other obstacles.")]
    private Transform top;
    [Tooltip("How far from a wall this enemy will change directions.")]
    [SerializeField]
    private float changeDirectionRange;
    [SerializeField]
    private MovingDirections startingDirection;
    [Tooltip("How long it takes for the enemy to reach its full acceleration. Larger number means slower and smoother transition.")]
    [SerializeField]
    private float accelerationTime = 0.25f;
    [Tooltip("Used to determine which layers the enemy considers obstacles and will change directions.")]
    [SerializeField]
    private LayerMask collisionMask;
    [Tooltip("How far from the player this enemy will start or stop moving.")]
    [SerializeField]
    private float activationRange = 30f;
    [Tooltip("Check if the moving enemy's graphic faces left originally - ensures that the character flips towards the right direction.")]
    [SerializeField]
    private bool facingLeft;

    private float acceleration;
    private MovingDirections movingDirection;
    private float movingSpeedTarget;
    private float currentMovingDirection;
    private Transform player;
    private Vector3 originalScale;
    private bool flipped;

    private AudioSource audioSource;
    [SerializeField] float soundActivationRange;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        movingDirection = startingDirection;
        movingSpeedTarget = movingDirection == MovingDirections.Left ? -1f : 1f;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (Vector3.Distance(player.position, transform.position) < soundActivationRange)
        {
            audioSource.volume = 1;
        }
        else
        {
            audioSource.volume = 0f;
        }

        if (Vector3.Distance(player.position, transform.position) > activationRange)
        { 
            return;
        }

        CheckForCollision();

        currentMovingDirection = Mathf.SmoothDamp(currentMovingDirection, movingSpeedTarget, ref acceleration, accelerationTime);
        var speedX = currentMovingDirection * movingSpeed;
        transform.position += new Vector3(speedX, 0f, 0f) * Time.deltaTime;

        CheckFacing();
    }

    private void CheckForCollision()
    {
        RaycastHit2D hit;

        if (movingDirection == MovingDirections.Left)
            hit = Physics2D.Raycast(top.position, Vector3.left, changeDirectionRange, collisionMask);
        else
            hit = Physics2D.Raycast(top.position, Vector3.right, changeDirectionRange, collisionMask);

        if (hit.collider != null)
        {
            movingDirection = movingDirection == MovingDirections.Left ? MovingDirections.Right : MovingDirections.Left;
            movingSpeedTarget = movingDirection == MovingDirections.Left ? -1f : 1f;
        }
    }

    private void CheckFacing()
    {
        if (facingLeft)
        {
            if (currentMovingDirection < 0f && flipped)
            {
                FlipCharacter(false);
            }
            else if (currentMovingDirection > 0f && !flipped)
            {
                FlipCharacter(true);
            }
        }
        else
        {
            if (currentMovingDirection > 0f && flipped)
            {
                FlipCharacter(false);
            }
            else if (currentMovingDirection < 0f && !flipped)
            {
                FlipCharacter(true);
            }
        }
    }

    private void FlipCharacter(bool flip)
    {
        var newX = flip ? -originalScale.x : originalScale.x;

        transform.localScale = new Vector3(newX, originalScale.y, originalScale.z);

        flipped = flip;
    }
}
