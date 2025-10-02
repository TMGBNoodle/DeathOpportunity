using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BestiaryEntry", menuName = "Scriptable Objects/BestiaryEntry")]
public class BestiaryEntry : ScriptableObject
{


    [SerializeField] String Name;
    [SerializeField] String FlavourText;
    [SerializeField] String AbilityDescription;
    [SerializeField] Texture2D sprite;

}
