using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float xSpeed = 0f;
    public float ySpeed = 0f;

    public bool enemyHit;
    public ScoreScript score;
    private void Awake()
    {
        Destroy(gameObject, 0.25f);
    }

    void Start()
    {
        score = FindObjectOfType<ScoreScript>();
        enemyHit = false;
    }

    void Update()
    {
        Vector2 position = transform.position;
        position.x += xSpeed * Time.deltaTime;
        position.y += ySpeed * Time.deltaTime;
        transform.position = position;
        //Debug.Log("update test");

        // transform.position += new Vector3(speed, 0f, 0f) * Time.deltaTime;
        // OnTriggerEnter2D(Collision2d);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("test");
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("hit ground");
            Destroy(gameObject, 0f);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            enemyHit = true;
            //Debug.Log("bullet hit");
            score.BulletHit();
            //Debug.Log("hit enemy");
            enemyHit = false;
            Destroy(gameObject, 0f);
        }
    }
}
