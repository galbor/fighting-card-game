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
    private const string BLEEDID = "Bleed";
    
    [SerializeField] private Slider _slider;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Text _healthText;
    [SerializeField] private Text _blockText;
    [SerializeField] private Image _image;
    
    [SerializeField] private float _statusEffectSpacing;
    [SerializeField] private BodyPartStatusEffect _statusEffectPrefab;
    [SerializeField] private Transform _statusEffectParent;
    [SerializeField] private Sprite _bleedImage; //TODO find another way to get this image

    private Pool<BodyPartStatusEffect> _pool;
    private List<BodyPartStatusEffect> _statusEffects;
    
    private int _maxHealth = 50;
    private int _currentHealth = 50;
    private int _defense = 0;
    private int _invincibility = 0;
    private int _bleed;
    public int Defense { get => _defense;
        set => SetDefense(value);
    }
    public int Bleed { get => _bleed;
        private set
        {
            int newBleed = Math.Max(value, 0);
            if (newBleed == 0)
            {
                RemoveStatusEffect(BLEEDID);
            }
            else
            {
                AddStatusEffect(BLEEDID, newBleed-_bleed, _bleedImage);
            }

            _bleed = newBleed;
        }
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
        _pool = new Pool<BodyPartStatusEffect>(Instantiate(_statusEffectPrefab, _statusEffectParent));
        _statusEffects = new List<BodyPartStatusEffect>();
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
        _currentHealth = health;
        _slider.value = _currentHealth;
        _healthText.text = _currentHealth + "/" + _maxHealth;
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
    
    public void AddBleed(int amt)
    {
        Bleed += amt;
    }
    
    /**
     * Bleed cannot go below 0
     */
    public void RemoveBleed(int amt)
    {
        Bleed -= amt;
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

    /**
     * Removes health based on amount of bleed
     * removes 1 bleed
     * @return True iff the object is still alive 
     */
    public bool TakeBleedDamage()
    {
        RemoveHealth(Bleed);
        RemoveBleed(1);
        return IsAlive();
    }
    
    public bool IsAlive()
    {
        return Health > 0;
    }

    /**
     * adds a status effect
     * if ID already exists, adds amt to the existing one
     * if the sum with amt is 0, removes the status effect
     */
    public void AddStatusEffect(string ID, int amt, Sprite sprite)
    {
        BodyPartStatusEffect status = _statusEffects.Find(x => x.ID == ID);
        if (status != null)
        {
            status.Sprite = sprite;
            status.Number += amt;
            if (status.Number == 0) RemoveStatusEffect(status);
            return;
        }

        status = _pool.GetFromPool();
        (status.ID, status.Number, status.Sprite) = (ID, amt, sprite);
        status.transform.position += new Vector3(_statusEffectSpacing * _statusEffects.Count, 0, 0);
        _statusEffects.Add(status);
    }

    /**
     * @return true iff status effect existed
     */
    public bool RemoveStatusEffect(string ID)
    {
        BodyPartStatusEffect status = _statusEffects.Find(x => x.ID == ID);
        if (status == null) return false;
        RemoveStatusEffect(status);

        return true;
    }

    /**
     * removes status effect
     * ASSUMES STATUS EFFECT EXISTED
     */
    private void RemoveStatusEffect(BodyPartStatusEffect status)
    {
        _pool.ReturnToPool(status);
        _statusEffects.Remove(status);
        for (int i = 0; i < _statusEffects.Count; i++)
        {
            _statusEffects[i].transform.position = 
                new Vector2(_statusEffectParent.position.x + i * _statusEffectSpacing, 0);
        }
    }
}
