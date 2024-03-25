using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public enum AudioEventTypes
    {
        Jump,
        Damage,
        Death,
        Coin,
        Healing
    }

    [System.Serializable]
    public struct AudioEvent
    {
        public AudioEventTypes AudioEventType;
        public AudioClip Clip;
    }

    [Tooltip("Reference to the player GameObject's AudioSource component.")]
    [SerializeField]
    private AudioSource source;
    [Tooltip("Array of different audio events, defined by AudioEventType.")]
    [SerializeField]
    private AudioEvent[] audioEvents;

    public void PlayAudioEvent(AudioEventTypes audioEventType)
    {
        foreach (var item in audioEvents)
        {
            if (item.AudioEventType == audioEventType)
            {
                source.PlayOneShot(item.Clip);
                return;
            }
        }
    }
}
