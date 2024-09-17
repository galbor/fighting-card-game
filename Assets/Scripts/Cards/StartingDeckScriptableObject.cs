using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StartingDeckScriptableObject : ScriptableObject
{
    [SerializeField] public BasicCard[] Cards;
}
