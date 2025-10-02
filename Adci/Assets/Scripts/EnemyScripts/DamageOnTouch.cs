using Unity.VisualScripting;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [SerializeField] 

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.CompareTag("Player"))
        {
            //damage player
            Destroy(gameObject);
        }
    }
}
