using System;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    [SerializeField] float Health = 50;


    [SerializeField] float knockBackLength = 0.1f;

    //things that grant Iframes: Knockback, Shift dash, Not attack dash
    public bool KnockedBack = false;

    public Dictionary<effects, bool> currentEffects = new Dictionary<effects, bool>
    {
        {effects.dash, false},
        {effects.attacking, false}
    };

    public bool canKnockback = true;

    (float, int) lastKnockbackDone = (0, 0);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addImmor(effects effect)
    {
        currentEffects[effect] = true;
    }

    public void removeImmor(effects effect)
    {
        currentEffects[effect] = false;
    }

    private void CheckDestroyed()
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage, float knockBackMultiplier, int direction)
    {
        if (!currentEffects[effects.attacking])
        {
            if (!currentEffects[effects.dash] && !KnockedBack)
            {
                Health -= damage;
                CheckDestroyed();
                if (canKnockback)
                {
                    Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
                    if (rb)
                    {
                        KnockedBack = true;
                        lastKnockbackDone = (knockBackMultiplier, direction);
                        Invoke("knockbackDone", knockBackLength);
                    }
                }
            }
        }
    }

    public (float, int) getKnockBackInfo()
    {
        return lastKnockbackDone;
    }

    public void TakeDamageTest(float damage)
    {
        Health -= damage;
        CheckDestroyed();
        KnockedBack = true;
        Invoke("knockbackDone", knockBackLength);
    }

    private void knockbackDone()
    {
        KnockedBack = false;
    }
}

public enum effects
{
    dash,
    attacking
}
