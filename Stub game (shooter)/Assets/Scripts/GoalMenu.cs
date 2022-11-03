using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GoalMenu : MonoBehaviour
{

    public GameObject goalMenu;
    public static bool isAtGoal;
    public ScoreScript scoreTotal;
    public PlayerMovement playerMovement;

    [SerializeField] private TextMeshProUGUI enemiesKilled;
    [SerializeField] private TextMeshProUGUI livesLeft;
    // [SerializeField] private TextMeshProUGUI secretFound;
    [SerializeField] private TextMeshProUGUI timeTotal;
    [SerializeField] private TextMeshProUGUI totalScore;
    [SerializeField] private TextMeshProUGUI rankScore;
    private int totalPoints;

    void Start()
    {
        scoreTotal = FindObjectOfType<ScoreScript>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        goalMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Goal"))
        {
            goalMenu.SetActive(true);
            Time.timeScale = 0f;

            enemiesKilled.text = "Enemies Killed: " + scoreTotal.killCount + "/30";
            livesLeft.text = "Lives left: " + playerMovement.currentLives;
            timeTotal.text = "Time: " + (int)playerMovement.timer + " seconds";
            totalPoints = scoreTotal.Score * playerMovement.currentLives;
            totalScore.text = "Total: " + totalPoints + " points";

            if (scoreTotal.Score >= 0 || scoreTotal.Score <= 5000)
            {
                rankScore.text = "Rank: C";
            }
            else if (scoreTotal.Score >= 5001 || scoreTotal.Score <= 10000)
            {
                rankScore.text = "Rank: B";
            }
            else if (scoreTotal.Score>= 10000 || scoreTotal.Score <= 15000)
            {
                rankScore.text = "Rank: A";
            }
            else if (scoreTotal.Score >= 15000 || scoreTotal.Score <= 20000)
            {
                rankScore.text = "Rank: S";
            }
            else if (scoreTotal.Score >= 20001)
            {
                rankScore.text = "Rank: S+"; 
            }
        }
    }
}
