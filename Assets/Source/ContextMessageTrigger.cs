using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextMessageTrigger : MonoBehaviour
{
    public enum TriggerTypes
    {
        Message,
        End
    }

    [SerializeField]
    private TriggerTypes TriggerType;
    [SerializeField]
    private string message = "This is the default message";
    [SerializeField]
    private float messageTime = 5f;
    [SerializeField]
    private int coinsNeeded = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (TriggerType == TriggerTypes.End && HudController.Instance.GetCoinsAmount() >= coinsNeeded)
            {
                HudController.Instance.VictoryScreen();
                return;
            }

            HudController.Instance.SetContextMessage(message, messageTime);
        }
    }
}
