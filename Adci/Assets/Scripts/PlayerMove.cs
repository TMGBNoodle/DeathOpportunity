using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    public float knockbackMult = 10;

    public float castWidth = .66f;

    public Vector2 knockBackDir = new Vector2(0.7f, 0.5f);

    public GameObject weapon;

    public float playerDamage = 10;
    public float lastAttack = -10;

    public float attackCooldown = 0.5f;

    public Status stats;

    private bool knockbackDone = false;

    private float knockBackRes = 1f;

    public bool onWall = false;

    int wallDir = 0;

    float lastWallJump = -10;

    float wallJumpCD = 0.5f;

    public Vector2 wallJumpParams = new Vector2(3, 8);
    Vector3 jumpDetectOffset = new Vector3(0, 0.5f);
    Vector3 wallDetectOffset = new Vector3(0.5f, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        stats = gameObject.GetComponent<Status>();
    }

    // Update is called once per frame
    void Update()
    {
        // float timeSinceJump = Time.time - jumptime;
        // float timeSinceDash = Time.time - dashTime;
        // bool canJump = timeSinceJump >= jumpDebounce;
        // bool canDash = timeSinceDash >= dashDebounce && dashCount < maxDashes;
        if (stats.KnockedBack)
        {
            print("Player Knocked Back");
            if (!knockbackDone)
            {
                (float, int) knockbackInfo = stats.getKnockBackInfo();
                knockbackDone = true;
                int direction = knockbackInfo.Item2;
                float knockBackAmount = knockbackInfo.Item1 * knockBackRes;
                rb.linearVelocity = knockBackAmount * new Vector2(knockBackDir.x * direction, knockBackDir.y);
            }
        }
        else if (playerControl)
        {
            knockbackDone = false;
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
            onWall = false;
            if (!isGrounded)
            {
                onWall = checkWall();
            }
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
            // dist remaining / time remaining (s)  = dist/s required
            // time.deltaTime * dist req = tomove
            float timeLeft = dashInfo.Item1;
            float distLeft = dashInfo.Item2;
            if (timeLeft <= 0)
            {
                stats.removeImmor(effects.dash);
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

    private bool checkWall()
    {
        Collider2D[] collisions1 = Physics2D.OverlapCircleAll(transform.position - wallDetectOffset, 0.2f);
        Collider2D[] collisions2 = Physics2D.OverlapCircleAll(transform.position + wallDetectOffset, 0.2f);
        foreach (Collider2D collision in collisions1)
        {
            if (collision.gameObject.tag == "Wall")
            {
                wallDir = 1;
                return true;
            }
        }
        foreach (Collider2D collision in collisions2)
        {
            if (collision.gameObject.tag == "Wall")
            {
                wallDir = -1;
                return true;
            }
        }
        return false;
    }
    public void Move(InputAction.CallbackContext ctx)
    {
        Vector2 moveVector = ctx.ReadValue<Vector2>();
        horizontalMove = moveVector.x;
        vertMove = moveVector.y;
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if ((Time.time - lastDash) > dashCooldown && !knockbackDone)
        {
            stats.addImmor(effects.dash);
            rb.linearVelocity = new Vector2(0, 0);
            playerControl = false;
            lastDash = Time.time;
            dashFlag = true;
            dashInfo = (dashTime, dashDist * horizontalMove);
        }
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (knockbackDone)
        {
            return;
        }
        else if (isGrounded)
        {
            rb.linearVelocityY = jumpPower;
        }
        else if (onWall && Time.time - lastWallJump > wallJumpCD)
        {
            print("Wall Jump");
            lastWallJump = Time.time;
            rb.linearVelocity = new Vector2(wallDir, 1) * wallJumpParams;
        }
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        if ((Time.time - lastAttack) > attackCooldown)
        {
            print("Attack");
            lastAttack = Time.time;
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
            //Attack casts counts how many rays will be fired. This is the count for EACH SIDE, top and bottom
            //So with attackCasts = 3 there will be 3 rays on top of the straight ray, and 3 on the bottom
            //for a total of 7. 
            List<GameObject> hitEnemies
            = new List<GameObject>();
            for (int i = 0; i < attackCasts; i++)
            {
                float angle = topAngle + (i * step);
                Vector3 offset = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, offset, attackRange);
                Debug.DrawRay(transform.position, offset * attackRange, Color.cyan, 5f);
                foreach (RaycastHit2D hit in hits)
                {
                    Status status = hit.transform.gameObject.GetComponent<Status>();
                    if (status && hit.transform.tag != "Player")
                    {
                        if (!hitEnemies.Contains(hit.transform.gameObject))
                        {
                            hitEnemies.Add(hit.transform.gameObject);
                            print("top hit");
                            status.TakeDamage(playerDamage, knockbackMult, facing);
                        }
                    }
                }
            }
            Vector3 sOffset = new Vector3(Mathf.Sin(midPoint), Mathf.Cos(midPoint));
            RaycastHit2D[] sHits = Physics2D.RaycastAll(transform.position, sOffset, attackRange);
            Debug.DrawRay(transform.position, attackRange * sOffset, Color.red, 5f);
            foreach (RaycastHit2D hit in sHits)
            {
                Status status = hit.transform.gameObject.GetComponent<Status>();
                if (status && hit.transform.tag != "Player")
                {
                    if (!hitEnemies.Contains(hit.transform.gameObject))
                    {
                        hitEnemies.Add(hit.transform.gameObject);
                        print("str hit");
                        status.TakeDamage(playerDamage, knockbackMult, facing);
                    }
                }
            }
            for (int i = 0; i < attackCasts; i++)
            {
                float angle = bottomAngle - (i * step);
                Vector3 offset = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle));
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, offset, attackRange);
                Debug.DrawRay(transform.position, offset * attackRange, Color.green, 5f);
                foreach (RaycastHit2D hit in hits)
                {
                    Status status = hit.transform.gameObject.GetComponent<Status>();
                    if (status && hit.transform.tag != "Player")
                    {
                        if (!hitEnemies.Contains(hit.transform.gameObject))
                        {
                            print("bottom hit");
                            hitEnemies.Add(hit.transform.gameObject);
                            status.TakeDamage(playerDamage, knockbackMult, facing);
                        }
                    }
                }
            }
        }
    }
}
