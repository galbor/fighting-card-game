using Relics;
using UnityEngine;
using cards;

[CreateAssetMenu]
public class StartingDeckScriptableObject : ScriptableObject
{
    [SerializeField] public BasicCard[] Cards;
    [SerializeField] public AbstractRelic StartingRelic;
}
