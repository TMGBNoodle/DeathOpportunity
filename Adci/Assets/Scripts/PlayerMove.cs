using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Mathematics;

//using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{

    public static PlayerMove Instance { get; private set; }

    public List<GameObject> hitEnemies
    = new List<GameObject>();

    public int abilCount = 0;

    public float projectileSpeed = 5;

    public GameObject flyProjectile;

    public int activeAbilInd = 0;
    public GameObject explosionPrefab;
    public LineRenderer[] rayDebug;
    public Rigidbody2D rb;
    public PlayerInput playerInput;
    public Animator anim;
    public SpriteRenderer sr;
    [SerializeField] GameObject slash;

    public float horizontalMove = 0;
    public float vertMove = 0;

    public float attackRange = 1.5f;

    public float speed = 5;

    public float dashDist = 3;

    public float dashTime = 0.1f;

    public float jumpPower = 5;
    bool isGrounded = true;

    float lastDash = -3;

    public float dashCooldown = 3;
    public (float, float) dashInfo;

    bool dashFlag = false;

    bool playerControl = true;
    int facing = 1;

    public float weaponOffset = 0.55f;

    public int attackCasts = 5;

    public float knockbackMult = 10;

    public float castWidth = .66f;

    public Vector2 knockBackDir = new Vector2(0.7f, 0.5f);
[SerializeField]
    public GameObject weapon;
[SerializeField]
    public abilities[] abils = new abilities[3] { abilities.flyProj, abilities.none, abilities.none };
[SerializeField]
    public abilities[] activeAbils = new abilities[3] { abilities.none, abilities.none, abilities.none };

    public float playerDamage = 10;
    public float lastAttack = -10;

    public float attackCooldown = 0.5f;
    public float slashEffectLife = 0.2f;

    public int maxCats = 1;

    public int currentCats = 0;

    public float attacking = 0;
    public Status stats;

    private bool knockbackDone = false;

    private float knockBackRes = 1f;

    public bool onWall = false;

    public float attackDuration = 0.3f;

    int wallDir = 0;

    float lastWallJump = -10;

    float attackTime = 0;

    bool aFlag = false;



    float wallJumpCD = 0.5f;

    public Vector2 wallJumpParams = new Vector2(3, 8);
    Vector3 jumpDetectOffset = new Vector3(0, 0.5f);
    Vector3 wallDetectOffset = new Vector3(0.5f, 0);

    SpriteRenderer slashRend;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        slashRend = slash.GetComponent<SpriteRenderer>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        stats = gameObject.GetComponent<Status>();
        anim = gameObject.GetComponentInChildren<Animator>();
        sr = gameObject.GetComponentInChildren<SpriteRenderer>();
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
            if (!knockbackDone)
            {
                anim.SetBool("Damaged", true);
                (float, int) knockbackInfo = stats.getKnockBackInfo();
                knockbackDone = true;
                int direction = knockbackInfo.Item2;
                float knockBackAmount = knockbackInfo.Item1 * knockBackRes;
                rb.linearVelocity = knockBackAmount * new Vector2(knockBackDir.x * direction, knockBackDir.y);
            }
        }
        else if (playerControl)
        {

            anim.SetBool("Damaged", false);
            knockbackDone = false;
            Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position - jumpDetectOffset, 0.05f);
            bool floorFound = false;
            foreach (Collider2D collision in collisions)
            {
                if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
                {
                    floorFound = true;
                    // if (Mathf.Abs(rb.linearVelocityY) <= 0.01f)
                    // {
                    //     rb.linearVelocityY = 0;
                    // }
                    currentCats = 0;
                    break;
                }
            }
            isGrounded = floorFound;
            anim.SetBool("Jumping", !isGrounded);
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
            if (vertMove == 1 && isGrounded)
            {
                Jump(new InputAction.CallbackContext());
                isGrounded = false;
            }
            rb.linearVelocityX = horizontalMove * speed;
            anim.SetBool("Walking", Math.Abs(rb.linearVelocityX) > 0.001);
            if (attacking > 0)
            {
                aFlag = true;
                print(attacking);
                attacking -= Time.deltaTime;
                attackTime += Time.deltaTime;
                if (attackTime > (attackDuration / 2.05))
                {
                    attackTime = 0;
                    doRaycastAttack();
                }
                float mod = (attackDuration - attacking) / attackDuration;
                slashRend.color = new Color(mod * 255, mod * 255, mod * 255, mod * 1);
            }
            else if (aFlag)
            {
                aFlag = false;
                hitEnemies.Clear();
            }
        }
        else if (dashFlag)
        {
            // dist remaining / time remaining (s)  = dist/s required
            // time.deltaTime * dist req = tomove
            attacking = 0;
            slash.SetActive(false);

            float timeLeft = dashInfo.Item1;
            float distLeft = dashInfo.Item2;
            if (timeLeft <= 0)
            {
                stats.removeImmor(effects.dash);
                anim.SetBool("Dashing", false);
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
        if (rb.linearVelocityX > 0)
        {
            sr.flipX = false;
        }
        else if (rb.linearVelocityX < 0)
        {
            sr.flipX = true;
        }
    }

    public void addAbil(abilities ability)
    {
        if (abilCount < 3)
        {
            switch (ability)
            {
                case abilities.swordUp:
                    attackRange = 5;
                    return;
                case abilities.bomber:
                    activeAbils[activeAbilInd] = ability;
                    activeAbilInd += 1;
                    return;
                case abilities.flyProj:
                    return;
            }
            abilCount += 1;
            abils[abilCount] = ability;
        }

    }

    public void bomb()
    {
        Collider2D[] explosionC = Physics2D.OverlapCircleAll(transform.position, 10);
        foreach (Collider2D hit in explosionC)
        {
            Status status = hit.transform.gameObject.GetComponent<Status>();
            if (status && hit.transform.tag != "Player")
            {
                if (!hitEnemies.Contains(hit.transform.gameObject))
                {
                    hitEnemies.Add(hit.transform.gameObject);
                    status.TakeDamage(playerDamage, 15, facing);
                }
            }
        }
    }

    private bool checkWall()
    {
        Collider2D[] collisions1 = Physics2D.OverlapCircleAll(transform.position - wallDetectOffset, 0.2f);
        Collider2D[] collisions2 = Physics2D.OverlapCircleAll(transform.position + wallDetectOffset, 0.2f);
        foreach (Collider2D collision in collisions1)
        {
            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
            {
                wallDir = 1;
                anim.SetBool("Jumping", false);
                return true;
            }
        }
        foreach (Collider2D collision in collisions2)
        {
            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
            {
                wallDir = -1;
                anim.SetBool("Jumping", false);
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
            anim.SetBool("Dashing", true);
            stats.addImmor(effects.dash);
            rb.linearVelocity = new Vector2(0, 0);
            playerControl = false;
            lastDash = Time.time;
            dashFlag = true;
            dashInfo = (dashTime, dashDist * Math.Sign(horizontalMove));
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
            isGrounded = false;
            anim.SetBool("Jumping", true);
            rb.linearVelocityY = jumpPower;
            isGrounded = false;
        }
        else if (onWall && Time.time - lastWallJump > wallJumpCD && currentCats < maxCats)
        {
            anim.SetBool("Jumping", true);
            currentCats += 1;
            lastWallJump = Time.time;
            rb.linearVelocity = new Vector2(wallDir, 1) * wallJumpParams;
        }
    }

    public void Abil1(InputAction.CallbackContext ctx)
    {
        switch (activeAbils[0])
        {
            case abilities.bomber:
                bomb();
                return;
        }
    }

    public void Abil2(InputAction.CallbackContext ctx)
    {
        switch (activeAbils[1])
        {
            case abilities.bomber:
                bomb();
                return;
        }
    }
    public void Attack(InputAction.CallbackContext ctx)
    {
        if ((Time.time - lastAttack) > attackDuration)
        {
            lastAttack = Time.time;
            hitEnemies.Clear();
            print("Attack");
            attacking = attackDuration;
            slash.SetActive(true);
            slash.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.3f);
            Vector2 pos = gameObject.transform.position;
            pos.x += (attackRange - 0.1f) * facing;
            slash.transform.position = pos;
            if (facing == 1)
            {
                slash.transform.rotation = Quaternion.Euler(0, 0, 45);
            }
            else
            {
                slash.transform.rotation = Quaternion.Euler(0, 180, 45);
            }
            lastAttack = Time.time;
            Invoke("turnOffSlash", slashEffectLife);
            if (abils.Contains(abilities.flyProj))
            {
                print("ShootFlyProj");
                Vector3 unProcessed = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 mousePos = new Vector3(unProcessed.x, unProcessed.y, 0);
                Vector3 directionToMouse = (mousePos - gameObject.transform.position).normalized;
                GameObject go = Instantiate(flyProjectile, transform.position, Quaternion.Euler(directionToMouse.x, directionToMouse.y, 0));
                go.GetComponent<Rigidbody2D>().linearVelocity = directionToMouse.normalized * projectileSpeed;
            }
            doRaycastAttack();
            
            //facing angle will either be (1, 0) or (-1, 0)
            //angle for facing angle is s = o/h
            // sin(x) = 1/h h = attackRange
            // sin(x) = 1/1.5
            // sin(pi/2)
            // max = (pi/2) - (castTotalAngle/2)
            // min = (pi/2) + (castTotalAngle/2)
        }
    }

    public void doRaycastAttack()
    {
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
                    print("Top attempting hit");
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
            print(hit.collider.gameObject.name);
            Status status = hit.transform.gameObject.GetComponent<Status>();
            if (status && !hit.transform.CompareTag("Player"))
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
                print(hit.collider.gameObject.name);
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

    public void turnOffSlash()
    {
        slash.SetActive(false);
    }
}

[Serializable]
public enum abilities
{
    swordUp,
    flyProj,
    bomber,
    none
}