using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Text _healthText;
    [SerializeField] private Text _blockText;
    [SerializeField] private Image _image;
    
    private int _maxHealth = 50;
    private int _currentHealth = 50;
    private int _defense = 0;
    private int _bleed = 0;
    private int _invincibility = 0;

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
        newWidth = Math.Min(newWidth, _maximumWidth);
        newWidth = Math.Max(newWidth, _minimumWidth);
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
        return health > 0;
    }

    public void SetDefense(int defense)
    {
        _defense = defense;
        _blockText.text = _defense.ToString();
        _blockText.transform.parent.gameObject.SetActive(defense != 0);
    }
    
    /**
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
    
        
    public void AddDefense(int defense)
    {
        SetDefense(_defense + defense);
    }

    public void SetBleed(int bleed)
    {
        _bleed = bleed;
    }
    
    public void AddBleed(int bleed)
    {
        SetBleed(_bleed + bleed);
    }
    
    public void RemoveBleed(int bleed)
    {
        SetBleed(_bleed - bleed);
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
     * @return True if the object is still alive, false otherwise
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
}
