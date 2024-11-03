using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.StatusEffects
{
    public class BodyPartStatusEffect : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _text;

        private int _number = 1;
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                _text.text = value.ToString();
            }
        }
        
        public string ID { get; set; }

        public Sprite Sprite
        {
            get => _image.sprite;
            set => _image.sprite = value;
        }

        //TODO statuseffect does an action
        // public Action<object> action { get; set; }

        private void Awake()
        {
            Number = _number;
        }
    }
}