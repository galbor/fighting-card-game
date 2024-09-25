using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Carrier : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] protected RectTransform _carriedSubject = null;
    [SerializeField] private Vector3 _distance;
    [SerializeField] private Vector3 _size = Vector3.zero;
    
    private Vector3 _prevPosition;
    private Vector3 _prevScale;
    private Vector3 _scale;

    private bool _healthBarNull = true;
    
    public float Distance
    {
        get => _distance.magnitude;
        set
        {
            _distance = _distance.normalized * value; 
            MoveHealthBar();
        }
    }

    public void SetDisplay(RectTransform display)
    {
        _carriedSubject = display;
        _healthBarNull = display == null;
        if (_healthBarNull) return;
     
        if (_camera == null) _camera = Camera.main;
        _carriedSubject.gameObject.SetActive(true);
        if (_size != Vector3.zero)
        {
            _carriedSubject.sizeDelta = _size;
        }
        MoveHealthBar();
    }
    
    public void SetActiveDisplay(bool active)
    {
        if (_healthBarNull) return;
        _carriedSubject.gameObject.SetActive(active);
    }

    private void OnDisable()
    {
        SetActiveDisplay(false);
    }

    private void OnEnable()
    {
        SetActiveDisplay(true);
    }

    // Start is called before the first frame update
    public void Awake()
    {
        // Vector3 healthBarPosition = _camera.ScreenToWorldPoint(_healthBar.position);
        // _distance = healthBarPosition - transform.position;
        _camera = Camera.main;
        SetDisplay(_carriedSubject);
        _prevPosition = transform.position + Vector3.one;
        _prevScale = transform.lossyScale;
    }

    private void Update()
    {
        if (_healthBarNull) return;
        if (_prevPosition != transform.position)
        {
            MoveHealthBar();
            _prevPosition = transform.position;
        }
        _scale = transform.lossyScale;
        if (_prevScale != _scale)
        {
            // _carriedSubject.localScale = transform.localScale;
            ChangeScale(_scale.y / _prevScale.y);
            _prevScale = _scale;
            MoveHealthBar();
        }

    }

    public void MoveHealthBar()
    {
        if (!_healthBarNull)
            _carriedSubject.position = _camera.WorldToScreenPoint(transform.position + _distance);
    }

    protected virtual void ChangeScale(float mult)
    {
        Vector2 size = _carriedSubject.sizeDelta;
        _carriedSubject.sizeDelta = new Vector2(size.x * mult, size.y * mult);
    }

    private void OnDestroy()
    {
        if (_healthBarNull) return;
        Destroy(_carriedSubject.gameObject);
    }
}
