using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreenController : MonoBehaviour
{
    [SerializeField] DeathScript[] scripts;

    [SerializeField] TextMeshProUGUI KilledBy;
    [SerializeField] TextMeshProUGUI Description;
    [SerializeField] TextMeshProUGUI LivesLeft;
    [SerializeField] Button RestartButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance.currentDeaths <= 0)
        { 
            RestartButton.interactable = false;
        }
        KilledBy.text = "Killed By: " + scripts[(int)GameManager.Instance.diedTo
            [GameManager.Instance.currentDeaths - 1]].name;
        Description.text = scripts[(int)GameManager.Instance.diedTo
            [GameManager.Instance.currentDeaths]].AbilityDescription;
        LivesLeft.text = "Lives Left: " + GameManager.Instance.LivesLeft;
    }

    // Update is called once per frame
    void Update()
    {

    }
    

}
