using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damege : MonoBehaviour
{
    [Tooltip("The damage this GameObject causes when hit by the player will be determined randomly between minDamage and maxDamage.")]
    [SerializeField]
    private float minDamage = 10f;
    [Tooltip("The damage this GameObject causes when hit by the player will be determined randomly between minDamage and maxDamage.")]
    [SerializeField]
    private float maxDamage = 20f;
    [Tooltip("How much touching this will knock the player back.")]
    [SerializeField]
    private float knockbackForce = 100f;
    [Tooltip("The time, for which touching this will disable player's controls. Set to 0 to disable.")]
    [SerializeField]
    private float disableControlsTime = 1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var playerHealth = collision.collider.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            var knockbackDirection = collision.GetContact(0).normal * -1f;
            var knockbackVector = knockbackDirection * knockbackForce;
            var damage = Random.Range(minDamage, maxDamage);
            var damageLocation = collision.GetContact(0).point;
            playerHealth.TakeDamage(damage, knockbackVector, damageLocation, disableControlsTime);
        }
    }
}
