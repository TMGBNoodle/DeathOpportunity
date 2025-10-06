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
        if (!collision.gameObject != gameObject)
        {
            if (!collision.gameObject.CompareTag(gameObject.tag))
            {
                collision.transform.GetComponent<Status>().TakeDamage(10, 0, -1);
            }
            else
            {
                return;
            }
            Destroy(gameObject);
        }
    }
}
