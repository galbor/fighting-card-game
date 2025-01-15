using System.Collections.Generic;
using System.ComponentModel;
using DefaultNamespace.StatusEffects;
using Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
    // UI component of health bar
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Text _healthText;
        [SerializeField] private Text _blockText;
        [SerializeField] private Image _image;

        [SerializeField] private float _statusEffectSpacing;
        [SerializeField] private Transform _statusEffectParent;

        private float _widthMaxHealthRatio;
        [SerializeField] private float _minimumWidth;
        [SerializeField] private float _maximumWidth;

        private int _maxHealth;
        private int _curHealth;

        private Sprite _blockSprite = null;

        private List<BodyPartStatusEffect> _statusEffects;

        public int MaxHealth { get => _maxHealth; set => SetMaxHealth(value); }
        public int Health { get => _curHealth; set => SetHealth(value); }

        public void Init(int maxHealth, List<BodyPartStatusEffect> statusEffects)
        {
            _widthMaxHealthRatio = _rectTransform.sizeDelta.x / maxHealth;
            MaxHealth = maxHealth;
            _statusEffects = statusEffects;
        }

        private void Awake()
        {
            SetBlockImage(_blockSprite);
        }


        private void SetMaxHealth(int value)
        {
            _maxHealth = value;
            SetWidth(value);
            _slider.maxValue = _maxHealth;
        }

        private void SetHealth(int value)
        {
            _curHealth = value;
            _slider.value = _curHealth;
            _healthText.text = _curHealth + "/" + _maxHealth;
        }

        public void SetBlockImage(Sprite sprite)
        {
            _blockSprite = sprite;
            if (_image == null) return;
            _image.sprite = sprite;
            _image.gameObject.SetActive(sprite != null);
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

        public void SetDefense(int defense)
        {
            _blockText.text = defense.ToString();
            _blockText.transform.parent.gameObject.SetActive(defense != 0);
        }

        public void SetStatusPosition(BodyPartStatusEffect status, int pos)
        {
            Transform statusTransform = status.transform;
            statusTransform.localPosition = new Vector3(_statusEffectSpacing * pos, 0, 0);
            statusTransform.SetParent(_statusEffectParent, false);
            statusTransform.localScale = Vector3.one;
        }

        /**
         * also sets parent
         */
        public void SetNewStatusPosition(BodyPartStatusEffect status)
        {
            SetStatusPosition(status, _statusEffects.Count);
        }

        public void RemoveStatusEffect(int index, BodyPartStatusEffect status)
        {
            for (int i = index; i < _statusEffects.Count; i++)
            {
                SetStatusPosition(_statusEffects[i], i);
            }
            status.transform.SetParent(EventManager.Instance._TextParent, true); //might as well be parent
        }
    }
}