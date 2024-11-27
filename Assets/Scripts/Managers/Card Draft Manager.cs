using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Utility;
using UnityEngine;
using cards;

namespace Managers
{
    public class CardDraftManager : Singleton<CardDraftManager>
    {
        [SerializeField] private BasicCard[] _possibleCards;

        [SerializeField] private GameObject _darkBackground;

        [SerializeField] private GameObject _skipText;
        
        [SerializeField] private KeyCode _selectCard1 = KeyCode.Alpha1;
        [SerializeField] private KeyCode _selectCard2 = KeyCode.Alpha2;
        [SerializeField] private KeyCode _selectCard3 = KeyCode.Alpha3;
        [SerializeField] private KeyCode _selectSkip = KeyCode.Alpha0;

        private List<BasicCard> _chosenCards;
        private IEnumerator _draftCoroutine;

        protected CardDraftManager() { }
        
        
        public void StartCardDraft()
        {
            //chooses the 3 cards to draft
            StateManager.Instance.AddState(this);
            
            _chosenCards = new List<BasicCard>();
            MyUtils.ChooseKRandomNumbersOrdered(_possibleCards.Length, 3).ForEach(x => _chosenCards.Add(_possibleCards[x]));
            SetDraftActive(true);
        }

        IEnumerator CardDraft(List<BasicCard> chosenCards)
        {
            int selectedCard = -1;
            
            DisplayChoice(chosenCards);
            
            while (true)
            {
                yield return null;
                if (Input.GetKeyDown(_selectCard1)) selectedCard = 0;
                else if (Input.GetKeyDown(_selectCard2)) selectedCard = 1;
                else if (Input.GetKeyDown(_selectCard3)) selectedCard = 2;
                else if (Input.GetKeyDown(_selectSkip)) selectedCard = -2;
                else selectedCard = -1;
                if (selectedCard != -1)
                    break;
            }
            if (selectedCard >= 0) Player.Instance.AddToDeck(chosenCards[selectedCard]);
            
            HideChoice();
            
            // Player.Instance.Person.gameObject.SetActive(true);
            
            StateManager.Instance.RemoveState();

            StateManager.Instance.AddState(MapManager.Instance);
        }

        private void DisplayChoice(List<BasicCard> chosenCards)
        {
            _darkBackground.SetActive(true); //TODO combine dark background and skip-text?
            _skipText.SetActive(true);
            PlayerTurn.Instance.Energy = 10; //assumes all cards have a price <= 10
            HandDisplayManager.Instance.DisplayCardsMiddle(chosenCards);
        }

        private void HideChoice()
        {
            HandDisplayManager.Instance.HideMiscCards();
            _darkBackground.SetActive(false);
            _skipText.SetActive(false);
        }

        /**
         * if active starts draft anew
         * else      hides draft
         *
         * SHOULD NOT BE CALLED BEFORE StartDraft
         */
        public void SetDraftActive(bool active)
        {
            if (_draftCoroutine != null) StopCoroutine(_draftCoroutine);
            if (active && _chosenCards != null)
            {
                _draftCoroutine = CardDraft(_chosenCards);
                StartCoroutine(_draftCoroutine);
                return;
            }
            HideChoice();
        }
    }
}