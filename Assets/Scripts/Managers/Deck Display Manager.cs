using System.Collections.Generic;
using UnityEngine;
using cards;

namespace Managers
{
    public class DeckDisplayManager : Singleton<DeckDisplayManager>
    {
        [SerializeField] private GameObject _darkBackground;

        [SerializeField] private KeyCode _deckKey;
        [SerializeField] private KeyCode _discardPileKey;
        [SerializeField] private KeyCode _drawPileKey;

        private KeyCode _curKeyCode;
        private bool _showing = false;
        private bool _inCombat;

        protected DeckDisplayManager() { }
        
        private void Awake()
        {
            DeckDisplayManager.Instance.enabled = false;
            EventManager.Instance.StartListening(EventManager.EVENT__START_COMBAT, obj => _inCombat = true);
            EventManager.Instance.StartListening(EventManager.EVENT__END_COMBAT, obj => _inCombat = false);
        }
        
        private void Update()
        {
            if (_showing && Input.GetKeyDown(_curKeyCode))
            {
                HideCardList();
                _curKeyCode = KeyCode.None;
                return;
            }

            if (Input.GetKeyDown(_deckKey))
                _curKeyCode = _deckKey;
            else if (!_inCombat) return;
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
            if (!_showing)
                StateManager.Instance.AddState(this);
            _showing = true;
            
            _darkBackground.SetActive(true);
            HandDisplayManager.Instance.DisplayCardsMiddle(cards, displayNumbers: false);
        }

        private void HideCardList()
        {
            HandDisplayManager.Instance.HideMiscCards();
            _darkBackground.SetActive(false);
            _showing = false;
            
            StateManager.Instance.RemoveState();
        }
    }
}