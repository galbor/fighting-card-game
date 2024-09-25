using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CardDisplay : MonoBehaviour
{
    private BasicCard _card;
    private Camera _camera;
    [SerializeField] private SpriteRenderer _cardImage;
    [SerializeField] private SpriteRenderer _cardSprite;
    [SerializeField] private TextCarrier _numberText;
    [SerializeField] private TextCarrier _nameText;
    [SerializeField] private TextCarrier _energyText;
    [SerializeField] private TextCarrier _descriptionText;
    [SerializeField] private float _margins;

    private Vector3 _prevExtends = Vector3.zero;
    private Vector3 _extends;

    private void Update()
    {
        _extends = _cardSprite.bounds.extents;
        if (_prevExtends != _extends)
        {
            SetScale();
            _prevExtends = _extends; //setScale also does this, but I think this code makes more sense if I do it here too
        }
    }

    public void SetCard(BasicCard card)
    {
        if (card == null)
        {
            gameObject.SetActive(false);
        }
        if (_camera == null) _camera = Camera.main;
        
        this._card = card;
        _cardImage.sprite = card.Image; 
        _nameText.Text = card.Name;
        _descriptionText.Text = card.DisplayDescription;
        _energyText.Text = card.Cost.ToString();
        SetScale();
        EnergyCostColor();
    }
    
    public void EnergyCostColor()
    {
        _energyText.Color = PlayerTurn.Instance.Energy < _card.Cost ? Color.red : Color.white;
    }

    public void SetNumberActive(bool active)
    {
        _numberText.SetActiveDisplay(active);
    }

    /**
     * sets the width of the text to be smaller than the width of the sprite renderer
     * 
     */
    private void SetWidth(TextCarrier textCarrier)
    {
        textCarrier.SetAligntment(TextAlignmentOptions.Top);
        
        textCarrier.SetWidth(_camera.WorldToScreenPoint(new Vector3(_cardSprite.bounds.extents.x - _margins, 0,0 )).x - _camera.WorldToScreenPoint(Vector3.zero).x);
    }
    
    private void SetHeight(TextCarrier textCarrier)
    {
        textCarrier.SetAligntment(TextAlignmentOptions.Top);
        
        textCarrier.Distance = _cardSprite.bounds.extents.y - _margins;
    }

    private void SetScale()
    {
        SetHeight(_nameText);
        SetWidth(_nameText);
        SetWidth(_descriptionText);
        _prevExtends = _extends;
    }
}
