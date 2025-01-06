using Relics;
using UnityEngine;
using cards;

[CreateAssetMenu]
public class CardDraftScriptableObject : ScriptableObject
{
    [SerializeField] public BasicCard[] Cards;
}
