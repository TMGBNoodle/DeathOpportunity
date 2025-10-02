using UnityEngine;

public class EnemyMovementBasicMelee : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] private int maxSpeed = 3;
    [SerializeField] private float acceleration = 1;

    private Vector2 Direction = Vector2.left;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        print(rb);
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = maxSpeed * Direction + new Vector2(0, rb.linearVelocityY);
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
    }
}
