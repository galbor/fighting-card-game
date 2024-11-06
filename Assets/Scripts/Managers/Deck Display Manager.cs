using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefaultNamespace.Managers;
using UnityEngine;

namespace Managers
{
    public class DeckDisplayManager : MonoBehaviour
    {
        [SerializeField] private GameObject _darkBackground;

        [SerializeField] private KeyCode _deckKey;
        [SerializeField] private KeyCode _discardPileKey;
        [SerializeField] private KeyCode _drawPileKey;

        private KeyCode _curKeyCode;
        private bool _isActive = false; //can't 

        private void Awake()
        {
            _isActive = false;
        }
        
        private void Update()
        {
            if (!_isActive) return;
            
            if (Input.GetKeyDown(_curKeyCode))
            {
                HideCardList();
                _curKeyCode = KeyCode.None;
                return;
            }

            if (Input.GetKeyDown(_deckKey))
                _curKeyCode = _deckKey;
            else if (CardDraftManager.Instance.Drafting)
                return;
            else if (Input.GetKeyDown(_discardPileKey))
                _curKeyCode = _discardPileKey;
            else if (Input.GetKeyDown(_drawPileKey))
                _curKeyCode = _drawPileKey;
            else return;
            
            ShowCardList(GetCardList(_curKeyCode));
        }

        /**
         * can't get the draw pile before the game started
         */
        public void Activate()
        {
            _isActive = true;
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
            SetActiveDraft(false);
            HandDisplayManager.Instance.HideMiscCards();
            HandDisplayManager.Instance.HideHand();
            SetActivePersons(false);
            _darkBackground.SetActive(true);
            PlayerTurn.Instance.StopAction();
            HandDisplayManager.Instance.DisplayCardsMiddle(cards);
        }

        private void HideCardList()
        {
            PlayerTurn.Instance.ResetAction(); //doesn't reset if drafting
            _darkBackground.SetActive(false);
            SetActivePersons(true);
            HandDisplayManager.Instance.DisplayHand();
            HandDisplayManager.Instance.HideMiscCards();
            SetActiveDraft(true);
        }
        
        /**
         * if drafting, returns to draft or hides draft
         */
        private void SetActiveDraft(bool active)
        {
            if (CardDraftManager.Instance.Drafting)
                CardDraftManager.Instance.SetDraftActive(active);
        }

        private void SetActivePersons(bool active)
        {
            Player.Instance.Person.gameObject.SetActive(active);
            RoomManager.Instance.Enemies.ToList().ForEach(x=>x.Person.gameObject.SetActive(active));
        }
    }
}