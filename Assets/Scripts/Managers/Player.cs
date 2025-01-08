using System.Collections.Generic;
using UnityEngine;
using cards;

namespace Managers
{
    public class Player : Singleton<Player>
    {
        [SerializeField] private int _defaultEnergy = 3;
        [SerializeField] private int _maxHandSize = 10;
        [SerializeField] private int _basicHandSize = 5;
        [SerializeField] private StartingDeckScriptableObject _startingDeck;
        [SerializeField] private Person _personPrefab;
        [SerializeField] private Transform _personParent;
        [SerializeField] private Vector2 _personPosition;
        private Person _player;
        private List<BasicCard> _deck;
        
        //last hit this combat
        public BasicAttackCard.AttackStruct LastHit { get; private set; }

        public List<BasicCard> Deck
        {
            get => _deck;
        }
        
        private void Awake()
        {
            _player = Instantiate(_personPrefab, _personParent);
            _player.transform.localPosition = _personPosition;
            _player.SetEnemyNumberActive(false);
            InitDeck();
            InitPlayerTurn();
        }

        private void Start()
        {
            EventManager.Instance.StartListening(EventManager.EVENT__START_COMBAT, obj => LastHit = BasicAttackCard.AttackStruct.None);
            EventManager.Instance.StartListening(EventManager.EVENT__HIT, obj =>
            {
                var hit = (BasicAttackCard.AttackStruct)obj;
                if (!hit._playerAttacker) return;
                LastHit = hit;
            });
        }

        protected Player()
        {
        }

        private void InitDeck()
        {
            _deck = new List<BasicCard>();
            foreach (var card in _startingDeck.Cards)
            {
                _deck.Add(Instantiate(card));
            }
        }

        private void InitPlayerTurn()
        {
            PlayerTurn.Instance.ObtainDeck();
            PlayerTurn.Instance.SetParameters(_defaultEnergy, _maxHandSize, _basicHandSize, _player);
        }
        
        public Person Person
        {
            get => _player;
        }
        
        public void AddToDeck(BasicCard card)
        {
            _deck.Add(card);
            PlayerTurn.Instance.ObtainDeck();
        }

        public void AddStartingRelic()
        {
            RelicManager.Instance.AddRelic(_startingDeck.StartingRelic);
        }
    }
}