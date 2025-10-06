using UnityEngine;

public class Parallax : MonoBehaviour
{
    //credit to Dani on youtube for the parallax script
    float length, startpos;
    public GameObject cam;

    public float parallax;
    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float temp = (cam.transform.position.x * (1 - parallax));
        float dist = (cam.transform.position.x * parallax);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        if (temp > startpos + 2 * length) startpos += 3 * length;
        else if (temp < startpos - 2 *length) startpos -=  3 * length;
    }
}
