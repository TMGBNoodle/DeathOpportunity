using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public GameObject player;
    public float followDist = 1.5f;

    public float maxSpeed = 2;

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

    void Update()
    {
        Vector3 dist = transform.position - player.transform.position;
        float speedMod = followDist - dist.magnitude;
        Vector3 toMove = (Time.deltaTime * speedMod * maxSpeed * dist.normalized);
        toMove.z = 0;
        toMove.y = 0;
        transform.position += toMove;
    }
}
