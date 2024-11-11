using DefaultNamespace.Relics;
using UnityEngine;

[CreateAssetMenu]
public class StartingDeckScriptableObject : ScriptableObject
{
    [SerializeField] public BasicCard[] Cards;
    [SerializeField] public AbstractRelic StartingRelic;
}
