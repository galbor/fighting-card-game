using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

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
    private float _spacing;
    private Vector3 _defaultScale;

    // Start is called before the first frame update
    void Awake()
    {
        _handCardDisplays = new List<CardDisplay>();
        foreach (Transform child in _cardDisplayParent)
        {
            _handCardDisplays.Add(child.GetComponent<CardDisplay>());
        }

        _miscCardDisplays = new List<CardDisplay>();

        _cardDisplayPool = new Pool<CardDisplay>(Instantiate(_handCardDisplays[0]), _cardDisplayParent);
        _defaultY = _handCardDisplays[0].transform.position.y;
        _middleX = (_handCardDisplays[0].transform.position.x + _handCardDisplays[^1].transform.position.x) / 2; //^1 is -1 index like python
        _spacing = _handCardDisplays[1].transform.position.x - _handCardDisplays[0].transform.position.x;
        _defaultScale = _handCardDisplays[0].transform.localScale;
        _ChosenCardLocation = Camera.main.WorldToScreenPoint(_ChosenCardLocation);
    }
    
    public void SetHand(List<BasicCard> cards)
    {
        SetDisplay(cards,_handCardDisplays, new Vector2(_middleX, _defaultY), 1f);
    }

    private void SetDisplay(List<BasicCard> cards, List<CardDisplay> cardDisplays, Vector2 middle, float scale)
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
            cardDisplays[i].SetCardNumber(i+1);
            cardDisplays[i].gameObject.SetActive(true);
        }

        DisplayCards(cardDisplays, amt, middle, scale);
    }

    public void DisplayHand()
    {
        DisplayCards(_handCardDisplays, _currentHandSize, new Vector2(_middleX, _defaultY), 1f);
    }
    
    //for card drafting
    public void DisplayCardsMiddle(List<BasicCard> cards)
    {
        SetDisplay(cards, _miscCardDisplays, _ChosenCardLocation, _ChosenCardSize);
    }

    private void DisplayCards(List<CardDisplay> cardDisplays, int amt, Vector2 middleVector, float scale)
    {
        float middle = ((float)amt-1) / 2;
        for (int i = 0; i < amt; i++)
        {
            cardDisplays[i].transform.position = new Vector2(middleVector.x + _spacing * scale * (i - middle), middleVector.y);
            cardDisplays[i].transform.localScale = _defaultScale * scale;
        }
        SetActiveNumbers(true);
    }

    public void HideHand()
    {
        HideCardsList(_handCardDisplays);
    }

    public void HideMiscCards()
    {
        HideCardsList(_miscCardDisplays);
        while (_miscCardDisplays.Count > 0)
        {
            _cardDisplayPool.ReturnToPool(_miscCardDisplays[0]);
            _miscCardDisplays.RemoveAt(0);
        }
    }

    private void HideCardsList(List<CardDisplay> cardDisplays)
    {
        cardDisplays.ForEach(x => x.gameObject.SetActive(false));
    }
    
    public void ChooseCard(int index)
    {
        _handCardDisplays[index].transform.position = _ChosenCardLocation;
        _handCardDisplays[index].transform.localScale = _defaultScale * _ChosenCardSize;
        SetActiveNumbers(false);
    }

    private void SetActiveNumbers(bool active)
    {
        for (int i = 0; i<_currentHandSize; i++)
        {
            _handCardDisplays[i].SetNumberActive(active);
        }
    }

    public void SetEnergyCostColors()
    {
        // _handCardDisplays.ForEach(x => x.EnergyCostColor());
        for (int i = 0; i<_currentHandSize; i++)
        {
            _handCardDisplays[i].EnergyCostColor();
        }
    }
}
