using System;
using System.Collections.Generic;
using DefaultNamespace.Relics;
using Unity.VisualScripting;
using UnityEngine;

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

        public List<BasicCard> Deck
        {
            get => _deck;
        }
        
        private void Awake()
        {
            _player = Instantiate(_personPrefab, _personParent);
            _player.gameObject.SetActive(false);
            _player.transform.localPosition = _personPosition;
            _player.SetEnemyNumberActive(false);
            InitDeck();
            InitPlayerTurn();
        }

        protected Player()
        {
        }

        private void InitDeck()
        {
            _deck = new List<BasicCard>();
            foreach (var card in _startingDeck.Cards)
            {
                _deck.Add(card);
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