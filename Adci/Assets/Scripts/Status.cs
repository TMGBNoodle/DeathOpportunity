using System;
using UnityEngine;

public class Status : MonoBehaviour
{
    [SerializeField] float Health = 50;


    [SerializeField] float knockBackLength = 0.1f;

    public Boolean KnockedBack = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CheckDestroyed()
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage/*, float knockBackMultiplier = 1*/)
    {
        Health -= damage;
        CheckDestroyed();
        KnockedBack = true;
        Invoke("knockbackDone", knockBackLength);
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
