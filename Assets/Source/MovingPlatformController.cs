using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    [Tooltip("Maximum speed of this platform.")]
    [SerializeField]
    private float maxSpeed = 10f;
    [Tooltip("The target transform of this platform. The platform will move to this target and then move back to its starting position.")]
    [SerializeField]
    private Transform targetPosition;
    [Tooltip("How long the platform will wait in place after reaching its destination.")]
    [SerializeField]
    private float waitTime;
    [Tooltip("How long it takes for this platform to reach its maxSpeed. Larger value means slower and smoother transition.")]
    [SerializeField]
    private float accelerationTime = 0.1f;

    private float targetSpeed;
    private float speedVelocity;
    private float currentSpeed;
    private bool waiting;
    private Vector3 currentTarget;
    private Vector3 startPosition;

    private void Start()
    {
        targetPosition.parent = null;
        currentTarget = targetPosition.position;
        startPosition = transform.position;
    }

    private void Update()
    {
        if (waiting)
            return;

        var distanceToTarget = Vector3.Distance(currentTarget, transform.position);
        targetSpeed = Mathf.Lerp(0f, maxSpeed, distanceToTarget);

        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, accelerationTime);

        var direction = (currentTarget - transform.position).normalized;
        transform.position += direction * currentSpeed * Time.deltaTime;

        if (transform.position == currentTarget)
        {
            StartCoroutine(WaitRoutine());
        }
    }

    private IEnumerator WaitRoutine()
    {
        waiting = true;
        yield return new WaitForSeconds(waitTime);
        waiting = false;

        currentTarget = currentTarget == targetPosition.position ? startPosition : targetPosition.position;
    }
}
