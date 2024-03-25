using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [Tooltip("How many seconds it takes to destroy this GameObject.")]
    [SerializeField]
    private float destroyTime = 2f;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
