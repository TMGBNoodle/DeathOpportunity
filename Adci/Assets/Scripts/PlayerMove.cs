using System.Collections;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public Rigidbody2D rb;
    public PlayerInput playerInput;

    float horizontalMove = 0;
    float vertMove = 0;

    public float speed = 5;

    public float dashDist = 3;

    public float dashTime = 0.1f;

    public float jumpPower = 5;
    bool isGrounded = true;

    float lastDash = -3;

    public float dashCooldown = 3;
    [SerializeField]
    public (float, float) dashInfo;

    bool dashFlag = false;

    bool playerControl = true;

    Vector3 jumpDetectOffset = new Vector3(0, 0.5f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerInput = gameObject.GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        // float timeSinceJump = Time.time - jumptime;
        // float timeSinceDash = Time.time - dashTime;
        // bool canJump = timeSinceJump >= jumpDebounce;
        // bool canDash = timeSinceDash >= dashDebounce && dashCount < maxDashes;
        if (playerControl)
        {
            Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position - jumpDetectOffset, 0.2f);
            bool floorFound = false;
            foreach (Collider2D collision in collisions)
            {
                if (collision.gameObject.tag == "Floor")
                {
                    floorFound = true;
                    if (Mathf.Abs(rb.linearVelocityY) <= 0.01f)
                    {
                        floorFound = true;
                        rb.linearVelocityY = 0;
                    }
                }
            }
            isGrounded = floorFound;
            rb.linearVelocityX = horizontalMove * speed;
        }
        else if (dashFlag)
        {
            print(dashInfo);
            // dist remaining / time remaining (s)  = dist/s required
            // time.deltaTime * dist req = tomove
            float timeLeft = dashInfo.Item1;
            float distLeft = dashInfo.Item2;
            if (timeLeft <= 0)
            {
                dashFlag = false;
                playerControl = true;
            }
            else
            {
                float movePs = (distLeft / timeLeft) * Time.deltaTime;
                rb.MovePosition(transform.position + new Vector3(movePs, 0));
                dashInfo = (timeLeft - Time.deltaTime, distLeft -= movePs);
            }
        }
    }

    void pointChar()
    {

    }

    public void Move(InputAction.CallbackContext ctx)
    {
        Vector2 moveVector = ctx.ReadValue<Vector2>();
        horizontalMove = moveVector.x;
        vertMove = moveVector.y;
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if ((Time.time - lastDash) > dashCooldown)
        {
            rb.linearVelocity = new Vector2(0, 0);
            playerControl = false;
            lastDash = Time.time;
            dashFlag = true;
            dashInfo = (dashTime, dashDist * horizontalMove);
        }
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (isGrounded)
        {
            rb.linearVelocityY = jumpPower;
        }
    }
}
