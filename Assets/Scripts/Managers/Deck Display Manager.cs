using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class DeckDisplayManager : Singleton<DeckDisplayManager>
    {
        [SerializeField] private GameObject _darkBackground;

        [SerializeField] private KeyCode _deckKey;
        [SerializeField] private KeyCode _discardPileKey;
        [SerializeField] private KeyCode _drawPileKey;

        private KeyCode _curKeyCode;

        protected DeckDisplayManager() { }
        
        private void Awake()
        {
            DeckDisplayManager.Instance.enabled = false;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(_curKeyCode))
            {
                HideCardList();
                _curKeyCode = KeyCode.None;
                return;
            }

            if (Input.GetKeyDown(_deckKey))
                _curKeyCode = _deckKey;
            else if (!PlayerTurn.Instance.enabled) return;
            else if (Input.GetKeyDown(_discardPileKey))
                _curKeyCode = _discardPileKey;
            else if (Input.GetKeyDown(_drawPileKey))
                _curKeyCode = _drawPileKey;
            else return;
            
            ShowCardList(GetCardList(_curKeyCode));
        }

        /**
         * like a dictionary but the value can change
         */
        private List<BasicCard> GetCardList(KeyCode key)
        {
            if (key == _deckKey)
                return Player.Instance.Deck;
            if (key == _discardPileKey) 
                return PlayerTurn.Instance.DiscardPile;
            if (key == _drawPileKey)
                return PlayerTurn.Instance.DrawPile;
            return null;
        }

        private void ShowCardList(List<BasicCard> cards)
        {
            StateManager.Instance.AddState(this);
            
            SetActivePersons(false);
            _darkBackground.SetActive(true);
            HandDisplayManager.Instance.DisplayCardsMiddle(cards, displayNumbers: false);
        }

        private void HideCardList()
        {
            HandDisplayManager.Instance.HideMiscCards();
            _darkBackground.SetActive(false);
            SetActivePersons(true);
            
            StateManager.Instance.RemoveState();
        }

        private void SetActivePersons(bool active)
        {
            Player.Instance.Person.gameObject.SetActive(active);
            RoomManager.Instance.Enemies.ToList().ForEach(x=>x.Person.gameObject.SetActive(active));
        }
    }
}