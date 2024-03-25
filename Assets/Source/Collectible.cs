using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Tooltip("How much health is gained by collecting this collectible.")]
    [SerializeField]
    private float healthGain;
    [Tooltip("How many coins are gained by collecting this collectible")]
    [SerializeField]
    private int coinsGain;
    [Tooltip("Optional VFX GameObject for collecting this collectible")]
    [SerializeField]
    private GameObject collectedVfx;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            if (healthGain > 0f)
            {
                player.GetComponent<PlayerHealth>().Heal(healthGain);
            }
            if (coinsGain > 0)
            {
                HudController.Instance.AddCoins(1);
                player.GetComponent<PlayerAudio>().PlayAudioEvent(PlayerAudio.AudioEventTypes.Coin);
            }
        }

        var vfx = Instantiate(collectedVfx, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
