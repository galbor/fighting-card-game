using System.Collections.Generic;
using System.Linq;
using Relics;
using DefaultNamespace.Utility;
using UnityEngine;

namespace Managers
{
    public class RelicManager : Singleton<RelicManager>
    {
        [SerializeField] private Transform _relicParent;
        [SerializeField] private float _spacing; //relic distance in relics
        [SerializeField] private RelicListScriptableObject _commonRelics;

        private AbstractRelic[] _possibleRelics;

        private Queue<AbstractRelic> _shuffledRelicsList;

        private List<AbstractRelic> _relics;

        protected RelicManager()
        {
        }

        private void Awake()
        {
            _relics = new List<AbstractRelic>();

            _possibleRelics = _commonRelics.Relics;

            _shuffledRelicsList = new Queue<AbstractRelic>();
            GenerateShuffledRelicsList();

            _spacing *= _possibleRelics[0].GetComponent<RectTransform>().sizeDelta.x;
        }

        public void AddRelic(AbstractRelic relicPrefab)
        {
            AbstractRelic abstractRelic = Instantiate(relicPrefab, _relicParent);
            abstractRelic.transform.localPosition += new Vector3(_spacing * _relics.Count, 0, 0);
            _relics.Add(abstractRelic);
            abstractRelic.OnAddRelic();
        }

        /**
         * has a shuffled list of relics, pops from the top
         * DOESN'T ADD THE RELIC TO THE RELIC LIST
         */
        public AbstractRelic PopRandomUnseenRelic()
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
            var res = PopRandomUnseenRelic();
            AddRelic(res);
            return res;
        }

        /**
         * sets _shuffledRelicsList
         */
        private void GenerateShuffledRelicsList()
        {
            AbstractRelic[] shuffledArray = (AbstractRelic[])MyUtils.ShuffledArray(_possibleRelics);
            _shuffledRelicsList.Clear();
            shuffledArray.ToList().ForEach(x => _shuffledRelicsList.Enqueue(x));
        }
    }
}