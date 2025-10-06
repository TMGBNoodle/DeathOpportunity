using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DeathScript", menuName = "Scriptable Objects/DeathScript")]
public class DeathScript : ScriptableObject
{
    [SerializeField] Enemies Name;
    [SerializeField] String AbilityDescription;
}
