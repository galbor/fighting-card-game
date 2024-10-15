﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace.Managers
{
    public class CardDraftManager : Singleton<CardDraftManager>
    {
        [SerializeField] private BasicCard[] _possibleCards;

        [SerializeField] private GameObject _darkBackground;
        
        [SerializeField] private KeyCode _selectCard1 = KeyCode.Alpha1;
        [SerializeField] private KeyCode _selectCard2 = KeyCode.Alpha2;
        [SerializeField] private KeyCode _selectCard3 = KeyCode.Alpha3;
        [SerializeField] private KeyCode _selectSkip = KeyCode.Alpha0;

        [SerializeField] private CardDisplay[] _cardDisplays;

        public void StartCardDraft()
        {
            HandDisplayManager.Instance.HideHand();

            //chooses the 3 cards to draft
            List<BasicCard> chosenCards = new List<BasicCard>();
            utils.ChooseKRandomNumbersOrdered(_possibleCards.Length, 3).ForEach(x => chosenCards.Add(_possibleCards[x]));
            StartCoroutine(CardDraft(chosenCards));
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
            
            RoomManager.Instance.SetNextRoom();
            
            PlayerTurn.Instance.StartRound();
        }

        private void DisplayChoice(List<BasicCard> chosenCards)
        {
            HandDisplayManager.Instance.DisplayCardsMiddle(chosenCards);
            _darkBackground.SetActive(true);
        }

        private void HideChoice()
        {
            HandDisplayManager.Instance.HideMiscCards();
            _darkBackground.SetActive(false);
        }
    }
}