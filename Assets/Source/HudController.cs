using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public static HudController Instance;

    [Tooltip("Reference to the UI Text component that shows the amount of coins.")]
    [SerializeField]
    private Text coinsAmount;
    [Tooltip("Reference to the UI Slider component that shows the player's health.")]
    [SerializeField]
    private Slider healthSlider;
    [Tooltip("Reference to the GameObject that will be revealed when the player dies.")]
    [SerializeField]
    private GameObject gameOver;
    [SerializeField]
    private Text contextMessage;
    [SerializeField]
    private GameObject victoryScreen;

    private int coins;

    private void Start()
    {
        Instance = this;
    }

    public void UpdateHealth(float healthAmount)
    {
        healthSlider.value = healthAmount;

        if (healthAmount <= 0)
            gameOver.SetActive(true);
    }

    private void UpdateCoinsAmount()
    {
        coinsAmount.text = coins.ToString();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinsAmount();
    }

    public void SetContextMessage(string text, float time)
    {
        contextMessage.text = text;
        StartCoroutine(ResetContextMessageRoutine(time));
    }

    private IEnumerator ResetContextMessageRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        contextMessage.text = string.Empty;
    }

    public int GetCoinsAmount()
    {
        return coins;
    }

    public void VictoryScreen()
    {
        victoryScreen.gameObject.SetActive(true);
    }
}
