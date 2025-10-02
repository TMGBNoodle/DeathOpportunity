using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EnemyMovementBasicFlyingRanged : MonoBehaviour
{

    [SerializeField] float visionRange = 10;
    [SerializeField] GameObject player;

    Vector2 playerPos;
    Vector2 enemyPos;

    Vector2 directionToPlayer;

    [SerializeField] float maxSpeed = 1;

    float speed;
    [SerializeField] int minDistFromPlayer = 4;
    [SerializeField] int maxDistFromPlayer = 5;

    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 0.5f;

    private float attackDelay = 2;
    private float attackTimer = 0;

    Rigidbody2D rb;

    Boolean approaching = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerMove.FindFirstObjectByType<PlayerMove>().gameObject;
        speed = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = player.transform.position;
        enemyPos = gameObject.transform.position;
        if (Vector2.Distance(playerPos, enemyPos) < visionRange)
        {
            directionToPlayer = playerPos - enemyPos;
            Move();
            attack();
        }
    }

    void Move()
    {
        if (approaching)
        {
            directionToPlayer = playerPos - enemyPos;
            rb.linearVelocity = directionToPlayer * speed;
        }
        else
        {
            //Vector2 direction = ;
            rb.linearVelocity = Vector2.zero;
            //rb.angularDamping = 1000;
        }

        if (Vector2.Distance(playerPos, enemyPos) < minDistFromPlayer)
        {
            approaching = false;
        }
        else if (!approaching && Vector2.Distance(playerPos, enemyPos) > maxDistFromPlayer)
        {
            approaching = true;
            //rb.angularDamping = 0;
        }
    }

    void attack()
    {
        if (!approaching && attackTimer < 0.0001)
        {
            GameObject go = Instantiate(projectile, enemyPos, Quaternion.Euler(directionToPlayer.x,
             directionToPlayer.y, 0));
            go.GetComponent<Rigidbody2D>().linearVelocity = directionToPlayer * speed;
            attackTimer = attackDelay;
        }
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;    
        }
    }
}
