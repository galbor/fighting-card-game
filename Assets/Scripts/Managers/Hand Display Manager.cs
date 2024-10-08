using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandDisplayManager : Singleton<HandDisplayManager>
{
    [SerializeField] private CardDisplay[] _cardDisplays;
    [SerializeField] private Vector2 _ChosenCardLocation;
    [SerializeField] private float _ChosenCardSize = 1.5f;

    private int _currentHandSize;
    
    private float _middleX;
    private float _defaultY;
    private float _spacing;
    private Vector3 _defaultScale;

    // Start is called before the first frame update
    void Awake()
    {
        _defaultY = _cardDisplays[0].transform.position.y;
        _middleX = (_cardDisplays[0].transform.position.x + _cardDisplays[^1].transform.position.x) / 2; //^1 is -1 index like python
        _spacing = _cardDisplays[1].transform.position.x - _cardDisplays[0].transform.position.x;
        _defaultScale = _cardDisplays[0].transform.localScale;
    }
    
    public void SetHand(List<BasicCard> cards)
    {
        SetHand(cards, new Vector2(_middleX, _defaultY), 1f);
    }

    private void SetHand(List<BasicCard> cards, Vector2 middle, float scale)
    {
        _currentHandSize = Math.Min(cards.Count, _cardDisplays.Length);
        for (int i = 0; i < _currentHandSize; i++)
        {
            _cardDisplays[i].SetCard(cards[i]);
            _cardDisplays[i].gameObject.SetActive(true);
        }
        for (int i = _currentHandSize; i < _cardDisplays.Length; i++)
        {
            _cardDisplays[i].gameObject.SetActive(false);
        }

        DisplayHand(_currentHandSize, middle, scale);
    }

    public void DisplayHand()
    {
        DisplayHand(_currentHandSize, new Vector2(_middleX, _defaultY), 1f);
    }

    public void DisplayHand(int amt, Vector2 middleVector, float scale)
    {
        float middle = ((float)amt-1) / 2;
        for (int i = 0; i < amt; i++)
        {
            _cardDisplays[i].transform.position = new Vector2(middleVector.x + _spacing * scale * (i - middle), middleVector.y);
            _cardDisplays[i].transform.localScale = _defaultScale * scale;
        }
        SetActiveNumbers(true);
    }

    //for card drafting
    public void DisplayCardsMiddle(List<BasicCard> cards)
    {
        SetHand(cards, _ChosenCardLocation, _ChosenCardSize);
    }

    public void HideHand()
    {
        _cardDisplays.ToList().ForEach(x => x.gameObject.SetActive(false));
    }
    
    public void ChooseCard(int index)
    {
        _cardDisplays[index].transform.position = _ChosenCardLocation;
        _cardDisplays[index].transform.localScale = _defaultScale * _ChosenCardSize;
        SetActiveNumbers(false);
    }

    private void SetActiveNumbers(bool active)
    {
        for (int i = 0; i<_currentHandSize; i++)
        {
            _cardDisplays[i].SetNumberActive(active);
        }
    }

    public void SetEnergyCostColors()
    {
        for (int i = 0; i<_currentHandSize; i++)
        {
            _cardDisplays[i].EnergyCostColor();
        }
    }
}
