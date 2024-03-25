using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Tooltip("Player's maximum health.")]
    [SerializeField]
    private float maxHealth = 100f;
    [Tooltip("Reference to the PlayerController component.")]
    [SerializeField]
    private PlayerController playerController;
    [Tooltip("Reference to the player's damage VFX Particle System component.")]
    [SerializeField]
    private ParticleSystem damageVfx;
    [SerializeField]
    private float restartLevelTimer = 5f;

    private Rigidbody2D physicsRb;
    private Animator animator;
    private float currentHealth;
    private bool alive = true;
    private PlayerAudio playerAudio;

    private void Reset()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        physicsRb = playerController.GetPhysicsRB();
        animator = playerController.GetAnimator();
        playerAudio = playerController.GetPlayerAudio();
    }

    public void TakeDamage(float damageAmount, Vector3 knockbackVector, Vector3 damagePosition, float disableControlsTime = 0f)
    {
        if (!alive)
            return;

        animator.SetTrigger("Damage");
        UpdateHealth(-damageAmount);
        physicsRb.AddForce(knockbackVector, ForceMode2D.Impulse);

        damageVfx.transform.position = damagePosition;
        damageVfx.Emit((int)damageAmount);

        if (disableControlsTime > 0f)
            playerController.DisableControls(disableControlsTime);

        playerController.ResetMovement();

        playerAudio.PlayAudioEvent(PlayerAudio.AudioEventTypes.Damage);
    }

    private void UpdateHealth(float damageDelta)
    {
        currentHealth = Mathf.Min(currentHealth + damageDelta, maxHealth);

        if (currentHealth <= 0)
        {
            Kill();
        }

        var healthNormalized = currentHealth / maxHealth;
        HudController.Instance.UpdateHealth(healthNormalized);
    }

    private void Kill()
    {
        animator.SetBool("Dead", true);
        alive = false;
        playerController.ResetMovement();
        playerController.ResetGravity();
        playerController.enabled = false;
        playerAudio.PlayAudioEvent(PlayerAudio.AudioEventTypes.Death);
        StartCoroutine(RestartLevelRoutine());
    }

    private IEnumerator RestartLevelRoutine()
    {
        yield return new WaitForSeconds(restartLevelTimer);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void Heal(float amount)
    {
        if (!alive)
            return;

        UpdateHealth(amount);

        playerAudio.PlayAudioEvent(PlayerAudio.AudioEventTypes.Healing);
    }
}
