using System;
using Unity.VisualScripting;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [SerializeField] Boolean hasTimeLimit = false;
    //[ShowIf("hasTimeLimit")]
    [SerializeField] float timeLimit = 5;

    public Enemies shotBy;

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
                Status stat = collision.transform.GetComponent<Status>();
                if (stat)
                {
                    stat.TakeDamage(10, shotBy);
                }
            }
            else
            {
                return;
            }
            Destroy(gameObject);
        }
    }
}
