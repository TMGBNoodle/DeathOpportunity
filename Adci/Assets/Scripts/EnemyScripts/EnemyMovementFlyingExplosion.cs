using System;
using UnityEngine;

public class EnemyMovementFlyingExplosion : MonoBehaviour
{
    [SerializeField] private float visionRange = 10;
    [SerializeField] private float maxSpeed = 5;
    private float speed;
    [SerializeField] private float explodeTime = 3;
    private float explodeTimer;
    [SerializeField] private float explosionRadius = 5;
    [SerializeField] private float radiusOfStartExploding = 1f;
    [SerializeField] private float blinkingRate = 5;
    [SerializeField] private GameObject explosion;
    private Boolean blinked = false;
    private Boolean blinking = false;
    [SerializeField] private Color blinkColor;
    [SerializeField] float KnockBackAmount = 2;
    bool attack = false;
    Rigidbody2D rb;
    SpriteRenderer sp;
    Status stat;
    GameObject player;


    private Vector2 playerPos;
    private Vector2 enemyPos;
    private Vector2 directionToPlayer;

    Boolean exploding = false;

    void Start()
    {
        stat = GetComponent<Status>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        player = PlayerMove.FindFirstObjectByType<PlayerMove>().gameObject;
        speed = maxSpeed;
        explodeTimer = explodeTime;
    }

    //Update is called once per frame
    void Update()
    {
        if (!stat.KnockedBack)
        {
            playerPos = player.transform.position;
            enemyPos = gameObject.transform.position;
            if(attack)
            {
                directionToPlayer = (playerPos - enemyPos).normalized;
                checkExplode();
                if (exploding)
                {
                    if (explodeTimer < 0)
                    {
                        Explode();
                    }
                    else
                    {
                        if (!blinking)
                        {
                            InvokeRepeating("Blink", 0, 1f / blinkingRate);
                            blinking = true;
                        }
                        explodeTimer -= Time.deltaTime;
                    }
                    rb.linearVelocity = Vector2.zero;
                }
            }
        }
        else
        {
            Vector2 direction = (gameObject.transform.position -
            player.transform.position).normalized;
            rb.AddForce(KnockBackAmount * direction);
            //Debug.Log("Here");
            Vector2 pos = gameObject.transform.position;
            Debug.DrawLine(gameObject.transform.position, pos + (direction * 3));
            /*Debug.Log("" + gameObject.transform.position + " : " + pos + (direction * 3)
            + " : " + direction);*/
        }
    }

    void FixedUpdate()
    {
        if (!exploding && attack)
        {
            Move();
        }
    }

    void Blink()
    {
        blinked = !blinked;
        if (blinked)
        {
            sp.color = blinkColor;
        }
        else
        {
            sp.color = Color.white;
        }
    }

    void Move()
    {
        directionToPlayer = (playerPos - enemyPos).normalized;
        rb.linearVelocity = directionToPlayer * speed;
    }

    void checkExplode()
    {
        if (Vector2.Distance(playerPos, enemyPos) < radiusOfStartExploding)
        {
            exploding = true;
        }
    }

    private void Explode()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        GameObject go = Instantiate(explosion, transform.position, Quaternion.identity);
        go.transform.localScale = 2 * explosionRadius * Vector3.one;
        Destroy(go, 2);

        for (int i = 0; i < objects.Length; i++)
        {
            Collider2D obj = objects[i];
            if (objects[i].gameObject.transform.CompareTag("Player"))
            {
                Status stats = obj.gameObject.GetComponent<Status>();
                if (stats)
                {
                    stats.TakeDamage(30, Enemies.Boombee);
                }
            }
        }


        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            attack = true;
        }
    }


}
