using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefaultNamespace.Relics;
using UnityEngine;

public class RelicManager : Singleton<RelicManager>
{
    private float RELICIMAGESIZE = 100f; //it's 100x100
    
    [SerializeField] private Transform _relicParent;
    [SerializeField] private float _spacing;
    [SerializeField] private AbstractRelic[] _possibleRelics;

    private Queue<AbstractRelic> _shuffledRelicsList;

    private List<AbstractRelic> _relics;

    protected RelicManager()
    { }

    private void Awake()
    {
        _relics = new List<AbstractRelic>();
        
        _shuffledRelicsList = new Queue<AbstractRelic>();
        GenerateShuffledRelicsList();
        
        _spacing *= RELICIMAGESIZE;
    }

    public void AddRelic(AbstractRelic relicPrefab)
    {
        AbstractRelic abstractRelic = Instantiate(relicPrefab, _relicParent);
        abstractRelic.transform.localPosition += new Vector3(_spacing * _relics.Count, 0, 0);
        _relics.Add(relicPrefab);
    }

    /**
     * has a shuffled list of relics, pops from the top
     * DOESN'T ADD THE RELIC TO THE RELIC LIST
     */
    public AbstractRelic GetRandomUnseenRelic()
    {
        var res = _shuffledRelicsList.Dequeue();
        if (_shuffledRelicsList.Count == 0) GenerateShuffledRelicsList();
        return res;
    }

    /**
     * Adds a random relic to the relic list
     */
    public AbstractRelic AddRandomUnseenRelic()
    {
        var res = GetRandomUnseenRelic();
        AddRelic(res);
        return res;
    }

    /**
     * sets _shuffledRelicsList
     */
    private void GenerateShuffledRelicsList()
    {
        AbstractRelic[] shuffledArray = ((AbstractRelic[])MyUtils.ShuffledArray(_possibleRelics));
        _shuffledRelicsList.Clear();
        shuffledArray.ToList().ForEach(x => _shuffledRelicsList.Enqueue(x));
    }
}