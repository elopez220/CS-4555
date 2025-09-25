using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Game Settings")]
    public int startingBananas = 10;
    public float gameTimer = 180f;

    [Header("UI Elements")]
    public Text player1ScoreText;
    public Text player2ScoreText;
    public Text timerText;

    private int player1Currency = 0;
    private int player2Currency = 0;
    private float currentTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = gameTimer;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            EndGame();
        }
        UpdateUI();
    }
    //update so it can add to who ever shot
    public void AddCurrency(int amount)
    {
        player1Currency += amount;
    }
    void UpdateUI()
    {
        if (player1ScoreText != null)
        {
            player1ScoreText.text = "P1 Currency: " + player1Currency;
        }
        if (player2ScoreText != null)
        {
            player2ScoreText.text = "P2 Currency: " + player2Currency;
        }
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.Ceil(currentTime).ToString();
        }
    }
    void EndGame()
    {
        Debug.Log("Game Over");
    }
}
