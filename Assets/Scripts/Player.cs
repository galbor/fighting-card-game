using System;
using System.Collections.Generic;
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

        private void Start()
        {
            _playerTurn = PlayerTurn.Instance;
            _player = _playerTurn.Player;
            // _player.Init();
            InitDeck();
            InitPlayerTurn();
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
            _playerTurn.Init(_defaultEnergy, _maxHandSize, _basicHandSize, _deck);
        }
        
        public Person Person
        {
            get => _player;
        }
    }
}