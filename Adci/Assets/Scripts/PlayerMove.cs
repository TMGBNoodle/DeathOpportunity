using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public LineRenderer[] rayDebug;
    public Rigidbody2D rb;
    public PlayerInput playerInput;

    float horizontalMove = 0;
    float vertMove = 0;

    public float attackRange = 1.5f;

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
    int facing = 1;

    public float weaponOffset = 0.55f;

    public int attackCasts = 5;

    public float castWidth = .66f;

    public GameObject weapon;

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
            if (horizontalMove != 0)
            {
                facing = (int)Mathf.Sign(horizontalMove);
                weapon.transform.position = transform.position + new Vector3(facing * weaponOffset, 0);
            }
            if (vertMove == 1)
            {
                Jump(new InputAction.CallbackContext());
            }
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

    public void Attack(InputAction.CallbackContext ctx)
    {
        print("Attack");
        //facing angle will either be (1, 0) or (-1, 0)
        //angle for facing angle is s = o/h
        // sin(x) = 1/h h = attackRange
        // sin(x) = 1/1.5
        // sin(pi/2)
        // max = (pi/2) - (castTotalAngle/2)
        // min = (pi/2) + (castTotalAngle/2)
        float castRad = Mathf.Abs((castWidth / 2));
        float midPoint;
        if (facing == 1)
        {
            midPoint = Mathf.PI / 2;
        }
        else
        {
            midPoint = (3 * Mathf.PI) / 2;
        }
        float topAngle = midPoint - castRad;
        float bottomAngle = midPoint + castRad;
        float step = castRad / attackCasts;
        print(topAngle);
        print(bottomAngle);
        print(midPoint);
        //Attack casts counts how many rays will be fired. This is the count for EACH SIDE, top and bottom
        //So with attackCasts = 3 there will be 3 rays on top of the straight ray, and 3 on the bottom
        //for a total of 7. 
        List<GameObject> hitEnemies
         = new List<GameObject>();
        for (int i = 0; i < attackCasts; i++)
        {
            print(topAngle);
            float angle = topAngle + (i * step);
            Vector3 offset = attackRange * new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.position + offset);
            Debug.DrawLine(transform.position, transform.position + offset, Color.cyan, 5f);
            print("Drawing ray");
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.tag == "Attackables")
                {
                    if (!hitEnemies.Contains(hit.transform.gameObject))
                    {
                        hitEnemies.Add(hit.transform.gameObject);
                        //enemy.takedamage
                    }
                }
            }
        }
        Vector3 sOffset = attackRange * new Vector3(Mathf.Sin(midPoint), Mathf.Cos(midPoint));
        RaycastHit2D[] sHits = Physics2D.RaycastAll(transform.position, transform.position + sOffset);
        Debug.DrawLine(transform.position, transform.position + sOffset, Color.red, 5f);
        foreach (RaycastHit2D hit in sHits)
        {
            if (hit.transform.tag == "Attackables")
            {
                if (!hitEnemies.Contains(hit.transform.gameObject))
                {
                    hitEnemies.Add(hit.transform.gameObject);
                    //enemy.takedamage
                }
            }
        }
        for (int i = 0; i < attackCasts; i++)
        {
            float angle = bottomAngle - (i * step);
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.position + new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)));
            Vector3 offset = attackRange * new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
            Debug.DrawLine(transform.position, transform.position + offset, Color.green, 5f);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.tag == "Attackables")
                {
                    if (!hitEnemies.Contains(hit.transform.gameObject))
                    {
                        hitEnemies.Add(hit.transform.gameObject);
                        //enemy.takedamage
                    }
                }
            }
        }
    }
}
