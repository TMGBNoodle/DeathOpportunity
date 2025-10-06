using System;
using System.Linq;
using UnityEngine;

public enum Enemies
{
    Goomba,
    Fly,
    Boombee
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Boolean[] Seen = new Boolean[Enemies.GetNames(typeof(Enemies)).Length];
    // public Boolean[] DiedTo = new Boolean[Enemies.GetNames(typeof(Enemies)).Length];

    public Enemies[] diedTo = new Enemies[3];

    public int currentDeaths = 0;

    public int LivesLeft = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Persist across scene loads
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addKiller(Enemies type)
    {
        print("Adding killer");
        print(type);
        diedTo[currentDeaths] = type;
        currentDeaths += 1;
    }
}


