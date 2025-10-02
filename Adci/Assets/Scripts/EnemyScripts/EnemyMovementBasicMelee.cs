using UnityEngine;

public class EnemyMovementBasicMelee : MonoBehaviour
{
    Rigidbody2D rb;
    Status stat;
    private GameObject player;

    [SerializeField] private int maxSpeed = 3;
    [SerializeField] private float acceleration = 1;

    [SerializeField] float KnockBackAmount = 2;

    private Vector2 Direction = Vector2.left;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stat = GetComponent<Status>();
        player = PlayerMove.FindFirstObjectByType<PlayerMove>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stat.KnockedBack)
        {
            rb.linearVelocity = maxSpeed * Direction + new Vector2(0, rb.linearVelocityY);
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

    void changeDirection()
    {
        Direction = -Direction;
    }

    void attack()
    {

        RaycastHit2D hit = Physics2D.Raycast(rb.position, -transform.right, 1);
        if (hit)
        {
            if (!hit.collider.gameObject.CompareTag("Player"))
            {
                //todo make hurt player
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            changeDirection();
            Debug.Log("Hit");
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            attack();
        }
    }
}
