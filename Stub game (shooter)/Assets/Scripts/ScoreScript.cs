using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreScript : MonoBehaviour
{
    public int k = 100;
    public int killWhileHoldingWall = 25;
    public int killWhileInAir = 50;
    public float pastKillTimer;
    public float currentKillTimer;
    float timer = 0.0f;
    public float killWindow = 5f;
    private float killTimeDifference;
    private float killMultiplier;
    public int killStreak = 1;
    public int Score;
    public Bullet bullet;
    public PlayerMovement playerMovement;
    public int killCount = 0;

    [SerializeField] private TextMeshProUGUI scoreTag;

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        timer += Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        scoreTag.text = "Score: " + Score;
        while (timer < killWindow)
        {
            if (killStreak >= 2)
            {
                currentKillTimer = Time.deltaTime;
                timer += (Time.deltaTime - currentKillTimer);
                killTimeDifference = (pastKillTimer / currentKillTimer);
                if (!playerMovement.isGrounded())
                {
                    killMultiplier = ((killTimeDifference + killStreak) * k) + killWhileInAir;
                }
                else if (playerMovement.isGrabbing)
                {
                    killMultiplier = ((killTimeDifference + killStreak) * k) + killWhileHoldingWall;
                }
                else
                {
                    killMultiplier = ((killTimeDifference + killStreak) * k);
                }
                pastKillTimer = currentKillTimer;
                //Score += (int)killMultiplier;
                //killStreak++;
            }
        }
        if (timer >= killWindow)
        {
            killStreak = 1;
        }
    }

    public void BulletHit()
    {
        pastKillTimer = Time.deltaTime;
        timer += (Time.deltaTime - pastKillTimer);
        Debug.Log(pastKillTimer);
        Debug.Log(currentKillTimer);
        if (!playerMovement.isGrounded())
        {
            Debug.Log("arial kill");
            Score += (k + killWhileInAir);
        }
        else if (playerMovement.isGrabbing)
        {
            Debug.Log("wallgrab kill");
            Score += (k + killWhileHoldingWall);
        }
        else
        {
            Debug.Log("Normal kill");
            killMultiplier = ((killStreak) * k);
            Score += (int)killMultiplier;
        }
        killStreak++;
        killCount++;
    }
}
