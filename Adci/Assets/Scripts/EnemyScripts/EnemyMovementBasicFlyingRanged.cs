using System;
//using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EnemyMovementBasicFlyingRanged : MonoBehaviour
{
    [SerializeField] enemyType enemyType = enemyType.Fly;
    [SerializeField] float visionRange = 10;
    GameObject player;

    Vector2 playerPos;
    Vector2 enemyPos;

    Vector2 directionToPlayer;

    [SerializeField] float maxSpeed = 1;

    float speed;
    [SerializeField] float minDistFromPlayer = 4;
    [SerializeField] float maxDistFromPlayer = 5;
    [SerializeField] float shootingRange = 7;
    [SerializeField] float floatHeight = 3;
    [SerializeField] LayerMask whatToFloatFrom;

    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 0.5f;
    [SerializeField] float KnockBackAmount = 5;

    private float attackDelay = 2;
    private float attackTimer = 0;


    Rigidbody2D rb;
    Status stat;
    Animator anim;
    SpriteRenderer sp;

    Boolean approaching = true;

    bool knockbackDone = false;

    private enemyState currentState = enemyState.wander;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = maxSpeed;
        player = PlayerMove.Instance.gameObject;
        stat = GetComponent<Status>();
        anim = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
    }

    void flipSprite()
    {
        if (directionToPlayer.x > 0)
        {
            sp.flipX = true;
        }
        else
        {
            sp.flipX = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == enemyState.wander)
        {
            if ((transform.position - player.transform.position).magnitude < visionRange)
            {
                currentState = enemyState.hunt;
            }
        }
        if (!stat.KnockedBack)
        {
            flipSprite();
            knockbackDone = false;
            if (currentState == enemyState.hunt && PlayerMove.Instance)
            {
                playerPos = player.transform.position;
                enemyPos = gameObject.transform.position;
                switch (enemyType)
                {
                    case enemyType.Eye:
                        anim.SetBool("Chasing", true);
                        break;
                    case enemyType.BAA:
                        anim.SetBool("Activated", true);
                        break;
                    case enemyType.Fly:
                        break;
                }

                directionToPlayer = (playerPos - enemyPos).normalized;
                Move();
                attack();

            }
            else if (currentState == enemyState.wander)
            {
                switch (enemyType)
                {
                    case enemyType.Eye:
                        anim.SetBool("Chasing", false);
                        break;
                    case enemyType.BAA:
                        anim.SetBool("Activated", false);
                        break;
                    case enemyType.Fly:
                        break;
                }
            }
            rb.linearVelocity += Floating();
        }
        else
        {
            knockbackDone = true;
            directionToPlayer = (gameObject.transform.position -
            player.transform.position).normalized;
            print(directionToPlayer);
            rb.linearVelocity = KnockBackAmount * directionToPlayer;
            //Debug.Log("Here");
            // Vector2 pos = gameObject.transform.position;
            // Debug.DrawLine(gameObject.transform.position, pos + (directionToPlayer * 3));
            /*Debug.Log("" + gameObject.transform.position + " : " + pos + (direction * 3)
            + " : " + direction);*/
        }
    }

    Vector2 Floating()
    {
        Vector2 addVel = new Vector2(0, 0);
        RaycastHit2D hit = Physics2D.Raycast(enemyPos, -gameObject.transform.up, floatHeight, whatToFloatFrom);
        Debug.DrawRay(enemyPos, -gameObject.transform.up * floatHeight, Color.red);
        //Debug.Log(hit.collider.gameObject.tag);
        if (hit)
        {
            if (!hit.collider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Here");
                addVel.y += maxSpeed;
            }
        }
        return addVel;
    }

    void Move()
    {

        if (approaching)
        {
            rb.linearVelocity = directionToPlayer * speed;
        }
        else
        {
            //Vector2 direction = ;
            rb.linearVelocity = -directionToPlayer * speed;
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
        if (Vector2.Distance(playerPos, enemyPos) < shootingRange && attackTimer < 0.0001)
        {
            if (enemyType == enemyType.Fly)
            {
                anim.SetTrigger("Attacking");
            }
            GameObject go = Instantiate(projectile, enemyPos, Quaternion.Euler(directionToPlayer.x,
             directionToPlayer.y, 0));
            go.GetComponent<Rigidbody2D>().linearVelocity = directionToPlayer.normalized * projectileSpeed;
            switch (enemyType)
                {
                    case enemyType.Eye:
                        break;
                    case enemyType.BAA:
                        break;
                    case enemyType.Fly:
                    go.GetComponent<DamageOnTouch>().shotBy = Enemies.Fly;
                        break;
                }
            attackTimer = attackDelay;
        }
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }
}

public enum enemyType
{
    Fly,
    Eye,
    BAA,
    Worm
}
