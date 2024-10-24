using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    public class Player : Singleton<Player>
    {
        [SerializeField] private int _defaultEnergy = 3;
        [SerializeField] private int _maxHandSize = 10;
        [SerializeField] private int _basicHandSize = 5;
        [SerializeField] private StartingDeckScriptableObject _startingDeck;
        private PlayerTurn _playerTurn;
        private Person _player;
        private List<BasicCard> _deck;
        //private List<relic> _relics

        public List<BasicCard> Deck
        {
            get => _deck;
        }
        
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _playerTurn = PlayerTurn.Instance;
            _player = _playerTurn.PlayerPerson;
            // _player.Init();
            InitDeck();
            InitPlayerTurn();
        }

        protected Player() { }

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
            _playerTurn.SetParameters(_defaultEnergy, _maxHandSize, _basicHandSize);
        }
        
        public Person Person
        {
            get => _player;
        }
        
        public void AddToDeck(BasicCard card)
        {
            _deck.Add(card);
            PlayerTurn.Instance.GetDeck();
        }
    }
}