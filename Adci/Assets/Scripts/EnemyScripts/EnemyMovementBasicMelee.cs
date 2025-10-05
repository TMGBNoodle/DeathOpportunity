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

    private Vector2 Direction = new Vector2(-1, 0);
    bool knockbackDone = false;

    public float damage = 10;

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
            flipSprite();
            knockbackDone = false;
            rb.linearVelocity = maxSpeed * Direction + new Vector2(0, rb.linearVelocityY);
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

    void attack()
    {
        anim.SetTrigger("Attack");
        RaycastHit2D[] hits = Physics2D.RaycastAll(rb.position, Direction, 2);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit)
            {
                print("Raycast hit something");
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    print("HitPlayer");
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
            print("Player encounter");
            attack();
        }
    }
}
