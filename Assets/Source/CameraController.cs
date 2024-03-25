using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("Target transform for the camera to follow. By default, this is found under the Player GameObject's Sprite as CameraFollowTarget.")]
    [SerializeField]
    private Transform cameraTarget;
    [Tooltip("The time it takes for the camera to move to its new position when the target is moved. Larger number means slower, smoother movement and vice versa.")]
    [SerializeField]
    private float smoothTime = 0.1f;
    [Tooltip("Toggle, if you want to use an optional offset for the camera and its follow target. This is determined at the begin of runtime.")]
    [SerializeField]
    private bool useOffset;

    private Vector3 cameraPositionSpeed;
    private Vector3 offset;
    private float cameraPositionZ;

    private void Reset()
    {
        cameraTarget = GameObject.Find("Player").transform;
    }

    private void Start()
    {
        if (useOffset)
            offset = transform.position - cameraTarget.position;
        cameraPositionZ = transform.position.z;
    }

    private void FixedUpdate()
    {
        var cameraTargetPosition = cameraTarget.position + offset;
        cameraTargetPosition.z = cameraPositionZ;
        transform.position = Vector3.SmoothDamp(transform.position, cameraTargetPosition, ref cameraPositionSpeed, smoothTime);
    }
}
