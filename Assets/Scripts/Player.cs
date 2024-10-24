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
        private Person _player;
        private List<BasicCard> _deck;
        //private List<relic> _relics

        public List<BasicCard> Deck
        {
            get => _deck;
        }
        
        private void Awake()
        {
            _player = PlayerTurn.Instance.PlayerPerson;
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
            PlayerTurn.Instance.GetDeck();
            PlayerTurn.Instance.SetParameters(_defaultEnergy, _maxHandSize, _basicHandSize);
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