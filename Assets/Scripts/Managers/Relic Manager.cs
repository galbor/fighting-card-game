using System.Collections.Generic;
using DefaultNamespace.Relics;
using UnityEngine;

public class RelicManager : Singleton<RelicManager>
{
    [SerializeField] private Transform _relicParent;
    [SerializeField] private float _spacing;

    private List<AbstractRelic> _relics;

    protected RelicManager()
    {
        
    }

    private void Awake()
    {
        _relics = new List<AbstractRelic>();
    }

    public void AddRelic(AbstractRelic relicPrefab)
    {
        AbstractRelic abstractRelic = Instantiate(relicPrefab, _relicParent);
        abstractRelic.transform.localPosition += new Vector3(_spacing * _relics.Count, 0, 0);
        _relics.Add(relicPrefab);
    }
}