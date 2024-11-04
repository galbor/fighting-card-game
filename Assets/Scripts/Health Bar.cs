using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefaultNamespace.StatusEffects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Text _healthText;
    [SerializeField] private Text _blockText;
    [SerializeField] private Image _image;
    
    [SerializeField] private float _statusEffectSpacing;
    [SerializeField] private BodyPartStatusEffect _statusEffectPrefab;
    [SerializeField] private Transform _statusEffectParent;

    private static Pool<BodyPartStatusEffect> _pool;
    private List<BodyPartStatusEffect> _statusEffects;

    private int _maxHealth = 50;
    private int _currentHealth = 50;
    private int _defense = 0;
    private int _invincibility = 0;
    public int Defense { get => _defense;
        set => SetDefense(value);
    }

    private float _widthMaxHealthRatio;
    [SerializeField] private float _minimumWidth;
    [SerializeField] private float _maximumWidth;

    public int Health
    {
        get => _currentHealth;
        set => SetHealth(value);
    }
    
    public int MaxHealth
    {
        get => _maxHealth;
        set => SetMaxHealth(value);
    }

    public int Invincibility
    {
        get => _invincibility;
        set => _invincibility = Math.Max(value, 0);
    }
    
    void Awake()
    {
        _slider.maxValue = _maxHealth;
        _widthMaxHealthRatio = _rectTransform.sizeDelta.x / _maxHealth;
        SetHealth(_maxHealth);
        _pool ??= new Pool<BodyPartStatusEffect>(Instantiate(_statusEffectPrefab, _statusEffectParent)); //if null assign
        _statusEffects = new List<BodyPartStatusEffect>();
    }

    private void OnDisable()
    {
        RemoveAllStatusEffects();
    }

    public void SetBlockImage(Sprite sprite)
    {
        _image.sprite = sprite;
        _image.enabled = sprite != null;
    }
    
    /**
     * Set the max health
     * the amount of max health added/removed will be added/removed to the current health
     * if removes too much health, sets health to 1
     * @param health The new max health value
     */
    public void SetMaxHealth(int maxHealth)
    {
        int change = maxHealth - _maxHealth;
        _maxHealth = maxHealth;
        
        SetWidth(_maxHealth);
        
        _slider.maxValue = _maxHealth;
        if (!AddHealth(change)) //if dead
            SetHealth(1);
    }

    /**
     * Sets the width of the UI health bar
     */
    private void SetWidth(int newMaxHealth)
    {
        float newWidth = newMaxHealth * _widthMaxHealthRatio;
        newWidth = Mathf.Clamp(newWidth, _minimumWidth, _maximumWidth);
        _rectTransform.sizeDelta = new Vector2(newWidth, _rectTransform.sizeDelta.y);
    }
    
    /**
        * Set the health
        * @param health The new health value
        * @return True if the object is still alive, false otherwise
        */
    public bool SetHealth(int health)
    {
        health = Mathf.Clamp(health, 0, _maxHealth);
        int healthChange = health - _currentHealth;
        _currentHealth = health;
        _slider.value = _currentHealth;
        _healthText.text = _currentHealth + "/" + _maxHealth;

        if (healthChange < 0) EventManager.Instance.TriggerEvent(EventManager.EVENT__REMOVE_HEALTH, -healthChange);
        if (!IsAlive()) RemoveAllStatusEffects();
        
        return IsAlive();
    }

    private void SetDefense(int defense)
    {
        _defense = defense;
        _blockText.text = _defense.ToString();
        _blockText.transform.parent.gameObject.SetActive(defense != 0);
    }
    
    /**
     * 
     * @return The new damage value
     */
    public int ReduceDefense(int damage)
    {
        if (_defense == 0) return damage;
        
        int difference = damage - _defense;
        damage = Math.Max(difference, 0);
        SetDefense(Math.Max(-difference, 0));
        return damage;
    }
    
        
    public void AddDefense(int amt)
    {
        Defense += amt;
    }
    
    /**
    * Add health to the object
    * @param health The health to add
    * @return True if the object is still alive, false otherwise
    */
    public bool AddHealth(int health)
    {
        return SetHealth(_currentHealth + health);
    }
    
    /**
     * Remove health from the object
     * @param health The health to remove
     * @return True iff the object is still alive
     */
    public bool RemoveHealth(int damage)
    {
        if (Invincibility > 0)
        {
            Invincibility -= 1;
            return true;
        }
        damage = ReduceDefense(damage);
        return SetHealth(_currentHealth - damage);
    }
    
    public bool IsAlive()
    {
        return Health > 0;
    }

    /**
     * adds a status effect
     * if already exists, adds amt to the existing one
     * if the sum with amt is 0, removes the status effect
     */
    public void AddStatusEffect(BodyPartStatusEffect.Type type, int amt)
    {
        BodyPartStatusEffect status = _statusEffects.Find(x => x.GetStatusType() == type);
        if (status != null)
        {
            SetStatusEffect(type, status.Number + amt);
            return;
        }
        SetStatusEffect(type, amt);
    }

    /**
     * sets a status effect's amt
     * if type doesn't exist, creates it
     * if 0, removes it
     */
    public void SetStatusEffect(BodyPartStatusEffect.Type type, int amt)
    {
        BodyPartStatusEffect status = _statusEffects.Find(x => x.GetStatusType() == type);
        if (status != null)
        {
            status.Number = amt; //if amt == 0, removes self
            return;
        }

        status = _pool.GetFromPool();
        status.SetType(type);
        status.BodyPart = this;
        SetNewStatusPosition(status);
        _statusEffects.Add(status);
        status.Number = amt;
    }

    private void SetNewStatusPosition(BodyPartStatusEffect status)
    {
        status.transform.localPosition = new Vector3(_statusEffectSpacing * _statusEffects.Count, 0, 0);
        status.transform.SetParent(_statusEffectParent, false);
        status.transform.localScale = Vector3.one;
    }

    /**
     * @return true iff status effect existed
     */
    public bool RemoveStatusEffect(BodyPartStatusEffect.Type type)
    {
        BodyPartStatusEffect status = _statusEffects.Find(x => x.GetStatusType() == type);
        if (status == null) return false;
        RemoveStatusEffect(status);

        return true;
    }

    /**
     * removes status effect
     * ASSUMES STATUS IS IN LIST
     */
    public void RemoveStatusEffect(BodyPartStatusEffect status)
    {
        _pool.ReturnToPool(status);
        _statusEffects.Remove(status);
        for (int i = 0; i < _statusEffects.Count; i++)
        {
            _statusEffects[i].transform.position = 
                new Vector2(_statusEffectParent.position.x + i * _statusEffectSpacing, 0);
        }
    }

    private void RemoveAllStatusEffects()
    {
        _statusEffects.ForEach(x =>
        {
            x.transform.SetParent(EventManager.Instance._TextParent, true); //might as well be parent
            _pool.ReturnToPool(x);
        });
        _statusEffects.Clear();
    }
}
