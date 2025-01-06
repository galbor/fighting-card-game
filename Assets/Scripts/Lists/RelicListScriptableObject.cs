using Relics;
using UnityEngine;
using cards;

[CreateAssetMenu]
public class RelicListScriptableObject : ScriptableObject
{
    [SerializeField] public AbstractRelic[] Relics;
}
