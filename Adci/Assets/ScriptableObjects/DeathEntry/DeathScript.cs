using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DeathScript", menuName = "Scriptable Objects/DeathScript")]
public class DeathScript : ScriptableObject
{
    [SerializeField] public Enemies Name;
    [SerializeField] public String AbilityDescription;
}
