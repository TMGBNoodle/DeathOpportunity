using System;
using UnityEngine;

public class EnemyMovementBasicMelee : MonoBehaviour
{

    Rigidbody2D rb;
    Status stat;
    private GameObject player;
    SpriteRenderer sp;
    Animator anim;


    [SerializeField] private int maxSpeed = 3;
    [SerializeField] private float acceleration = 1;

    [SerializeField] float KnockBackMult = 1.5f;

    public Vector2 knockBackDir = new Vector2(1, 1);

    public float damageKnockbackMult = 1;

    public float attackLength = 2;

    public float attackFatigue = 0;

    private Vector2 Direction = new Vector2(-1, 0);
    bool knockbackDone = false;

    public float damage = 10;

    public float fatigueLength = 0.5f;

    public GameObject currentTarget;

    public enemyState currentState = enemyState.wander;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stat = GetComponent<Status>();
        player = PlayerMove.FindFirstObjectByType<PlayerMove>().gameObject;
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void flipSprite()
    {
        if (rb.linearVelocityX > 0)
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
        if (!stat.KnockedBack)
        {
            if (attackFatigue <= 0)
            {
                if (currentState == enemyState.hunt)
                {
                    Direction.x = Math.Sign(transform.position.x - currentTarget.transform.position.x);
                }
                flipSprite();
                knockbackDone = false;
                rb.linearVelocity = maxSpeed * Direction + new Vector2(0, rb.linearVelocityY);
            }
            else
            {
                attackFatigue -= Time.deltaTime;
            }
        }
        else if (!knockbackDone)
        {
            (float, int) knockbackInfo = stat.getKnockBackInfo();
            knockbackDone = true;
            int direction = knockbackInfo.Item2;
            float knockBackAmount = knockbackInfo.Item1 * KnockBackMult;

            rb.linearVelocity = knockBackAmount * new Vector2(knockBackDir.x * direction, knockBackDir.y);
            //Debug.Log("Here");
            // Vector2 pos = gameObject.transform.position;
            // Debug.DrawLine(gameObject.transform.position, pos + (direction * 3));
            /*Debug.Log("" + gameObject.transform.position + " : " + pos + (direction * 3)
            + " : " + direction);*/
        }
    }

    void changeDirection()
    {
        Direction = -Direction;
    }

    public void StartHunt(GameObject target)
    {
        currentTarget = target;
        currentState = enemyState.hunt;
    }

    void attack()
    {
        anim.SetTrigger("Attack");
        attackFatigue = fatigueLength;
        RaycastHit2D[] hits = Physics2D.RaycastAll(rb.position, Direction, attackLength);
        Debug.DrawRay(rb.position, Direction * attackLength, Color.black, 1);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit)
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    hit.transform.GetComponent<Status>().TakeDamage(damage, damageKnockbackMult, (int)Direction.x);
                }
            }
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            int collisionDir = (int)Mathf.Sign(transform.position.x - collision.transform.position.x);
            if (collisionDir != Direction.x)
            {
                changeDirection();
            }
            Debug.Log(transform.position.x - collision.transform.position.x);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            if (attackFatigue <= 0)
            {
                attack();
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            player = collision.gameObject;
            currentState = enemyState.hunt;
        }
    }
}

public enum enemyState
{
    wander,
    hunt
}
