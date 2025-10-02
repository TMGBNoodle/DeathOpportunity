using System;
using UnityEngine;

public class EnemyMovementFlyingExplosion : MonoBehaviour
{
    [SerializeField] private float visionRange = 10;
    [SerializeField] private float maxSpeed = 1;
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
    Rigidbody2D rb;
    SpriteRenderer sp;
    GameObject player;


    private Vector2 playerPos;
    private Vector2 enemyPos;
    private Vector2 directionToPlayer;

    Boolean exploding = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        player = PlayerMove.FindFirstObjectByType<PlayerMove>().gameObject;
        speed = maxSpeed;
        explodeTimer = explodeTime;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = player.transform.position;
        enemyPos = gameObject.transform.position;
        if (Vector2.Distance(playerPos, enemyPos) < visionRange)
        {
            directionToPlayer = playerPos - enemyPos;
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
            }
        }
        
    }

    void FixedUpdate()
    { 
        Move();
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
        directionToPlayer = playerPos - enemyPos;
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
        Collider[] objects = Physics.OverlapSphere(transform.position, explosionRadius);

        GameObject go = Instantiate(explosion, transform.position, Quaternion.identity);
        go.transform.localScale = 2 * explosionRadius * Vector3.one;
        Destroy(go, 2);
        
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].gameObject.transform.CompareTag("Player"))
            {
                //damage player

            }
        }


        Destroy(gameObject);
    }

    
}
