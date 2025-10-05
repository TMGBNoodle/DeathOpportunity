using System;
using Unity.VisualScripting;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [SerializeField] Boolean hasTimeLimit = false;
    //[ShowIf("hasTimeLimit")]
    [SerializeField] float timeLimit = 5;

    void Start()
    {
        if (hasTimeLimit)
        {
            Destroy(gameObject, timeLimit);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.transform.CompareTag("Enemy"))
        {
            if (collision.gameObject.transform.CompareTag("Player"))
            {
                collision.transform.GetComponent<Status>().TakeDamage(10);
            }

            Destroy(gameObject);
        }
    }
}
