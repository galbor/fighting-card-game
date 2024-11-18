using System;
using System.Collections.Generic;
using DefaultNamespace.Utility;
using DefaultNamespace.UI;
using UnityEngine;

namespace Managers
{
    public class HandDisplayManager : Singleton<HandDisplayManager>
    {
        [SerializeField] private Transform _cardDisplayParent;
        [SerializeField] private Vector2 _ChosenCardLocation;
        [SerializeField] private float _ChosenCardSize = 1.5f;

        private List<CardDisplay> _handCardDisplays;
        private List<CardDisplay> _miscCardDisplays;
        private Pool<CardDisplay> _cardDisplayPool;

        private int _currentHandSize;

        private float _middleX;
        private float _defaultY;
        private float _horizontalSpacing;
        private float _verticalSpacing;
        private Vector3 _defaultScale;

        protected HandDisplayManager()
        {
        }

        private void Awake()
        {
            _handCardDisplays = new List<CardDisplay>();
            foreach (Transform child in _cardDisplayParent)
            {
                _handCardDisplays.Add(child.GetComponent<CardDisplay>());
            }

            _miscCardDisplays = new List<CardDisplay>();

            _cardDisplayPool = new Pool<CardDisplay>(Instantiate(_handCardDisplays[0], _cardDisplayParent));
            _defaultY = _handCardDisplays[0].transform.position.y;
            _middleX = (_handCardDisplays[0].transform.position.x + _handCardDisplays[^1].transform.position.x) /
                       2; //^1 is -1 index like python
            _defaultScale = _handCardDisplays[0].transform.localScale;
            _horizontalSpacing = _handCardDisplays[1].transform.position.x - _handCardDisplays[0].transform.position.x;
            _verticalSpacing =
                CalculateVerticalSpacing(_handCardDisplays[0].GetCardSize(), _horizontalSpacing, _defaultScale);
            _ChosenCardLocation = Camera.main.WorldToScreenPoint(_ChosenCardLocation);
        }

        public void SetHand(List<BasicCard> cards)
        {
            _cardDisplayParent.gameObject.SetActive(true);
            SetDisplay(cards, _handCardDisplays, new Vector2(_middleX, _defaultY), 1f);
        }

        private void SetDisplay(List<BasicCard> cards, List<CardDisplay> cardDisplays, Vector2 middle, float scale,
            bool displayNumber = true)
        {
            int amt = cards.Count;
            if (cardDisplays == _handCardDisplays)
            {
                amt = _currentHandSize = Math.Min(amt, _handCardDisplays.Count);

                for (int i = _currentHandSize; i < cardDisplays.Count; i++)
                {
                    cardDisplays[i].gameObject.SetActive(false);
                }
            }
            else
            {
                while (cardDisplays.Count < amt)
                {
                    cardDisplays.Add(_cardDisplayPool.GetFromPool());
                }
            }

            for (int i = 0; i < amt; i++)
            {
                cardDisplays[i].SetCard(cards[i]);
                cardDisplays[i].SetCardNumber(i + 1);
                cardDisplays[i].SetNumberActive(displayNumber);
            }

            DisplayCards(cardDisplays, amt, middle, scale);
        }

        public void DisplayHand()
        {

            DisplayCards(_handCardDisplays, _currentHandSize, new Vector2(_middleX, _defaultY), 1f);
            SetActiveNumbers(true);
        }

        /**
         * shows _cards_ in the middle
         * @param displayNumbers true means the card numbers are on
         */
        public void DisplayCardsMiddle(List<BasicCard> cards, bool displayNumbers = true)
        {
            SetDisplay(cards, _miscCardDisplays, _ChosenCardLocation, _ChosenCardSize, displayNumbers);
        }

        /**
         * places the cardDisplays in their place
         */
        private void DisplayCards(List<CardDisplay> cardDisplays, int amt, Vector2 middleVector, float scale)
        {
            //doesn't work well with resolution change after game was started

            int rows = (int)Math.Ceiling(_horizontalSpacing * scale * amt / Screen.width);
            int cols = (int)Math.Floor(Screen.width / (_horizontalSpacing * scale));

            float middleRow = ((float)rows - 1) / 2;
            float middleCol;

            int i = 0;
            for (int row = 0; row < rows; row++)
            {
                cols = Math.Min(cols, amt - row * cols);
                middleCol = ((float)cols - 1) / 2;

                for (int col = 0; col < cols; col++)
                {
                    cardDisplays[i].gameObject.SetActive(true);
                    cardDisplays[i].transform.position =
                        new Vector2(middleVector.x + _horizontalSpacing * scale * (col - middleCol),
                            middleVector.y - _verticalSpacing * scale * (row - middleRow));

                    cardDisplays[i].transform.localScale = _defaultScale * scale;
                    if (++i >= amt) return;
                }
            }
        }

        public void HideHand()
        {
            HideCardsList(_handCardDisplays);
        }

        public void HideMiscCards()
        {
            HideCardsList(_miscCardDisplays);
            _miscCardDisplays.ForEach(x => _cardDisplayPool.ReturnToPool(x));
            _miscCardDisplays.Clear();
        }

        private void HideCardsList(List<CardDisplay> cardDisplays)
        {
            cardDisplays.ForEach(x => x.gameObject.SetActive(false));
        }

        /**
         * places the chosen card in the center of the screen
         */
        public void ChooseCard(int index)
        {
            _handCardDisplays[index].transform.position = _ChosenCardLocation;
            _handCardDisplays[index].transform.localScale = _defaultScale * _ChosenCardSize;
            SetActiveNumbers(false);
        }

        private void SetActiveNumbers(bool active)
        {
            for (int i = 0; i < _currentHandSize; i++)
            {
                _handCardDisplays[i].SetNumberActive(active);
            }
        }

        public void SetEnergyCostColors()
        {
            // _handCardDisplays.ForEach(x => x.EnergyCostColor());
            for (int i = 0; i < _currentHandSize; i++)
            {
                _handCardDisplays[i].EnergyCostColor();
            }
        }

        /**
         * the cards: ****_****
         * the horizontalSpacing would be **_**
         * so the space between the cards would be **_**  -   ****   =   _
         */
        private float CalculateVerticalSpacing(Vector2 cardSize, float horizontalSpacing, Vector2 defaultScale)
        {
            float spaceBetweenCards = horizontalSpacing - cardSize.x * defaultScale.x;
            return spaceBetweenCards + cardSize.y * defaultScale.y;
        }
    }
}