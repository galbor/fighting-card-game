using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDisplayManager : MonoBehaviour
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
        _middleX = (_cardDisplays[0].transform.position.x + _cardDisplays[_cardDisplays.Length - 1].transform.position.x) / 2;
        _spacing = _cardDisplays[1].transform.position.x - _cardDisplays[0].transform.position.x;
        _defaultScale = _cardDisplays[0].transform.localScale;
    }
    
    public void SetHand(List<BasicCard> cards)
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

        DisplayHand();
    }

    public void DisplayHand()
    {
        float middle = ((float)_currentHandSize-1) / 2;
        for (int i = 0; i < _currentHandSize; i++)
        {
            _cardDisplays[i].transform.position = new Vector2(_middleX + _spacing * (i - middle), _defaultY);
            _cardDisplays[i].transform.localScale = _defaultScale;
        }
        SetActiveNumbers(true);
    }
    
    public void ChooseCard(int index)
    {
        _cardDisplays[index].transform.position = _ChosenCardLocation;
        _cardDisplays[index].transform.localScale *= _ChosenCardSize;
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
