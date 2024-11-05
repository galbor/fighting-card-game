using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DefaultNamespace.StatusEffects
{
    public class BodyPartStatusEffect : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _text;

        public HealthBar BodyPart { get; set; }
        
        private StatusType _statusType;
        private TypeParameters _typeParameters;
        
        public enum StatusType
        {
            NONE,
            BLEED,
            KNIFE
        }

        private struct TypeParameters
        {
            public TypeParameters(string spriteName, Dictionary<string, UnityAction<object>> eventActionDict)
            {
                Sprite = Resources.Load<Sprite>(spriteName);
                EventActionDict = eventActionDict;
            }
            
            public Sprite Sprite { get; private set; }
            public Dictionary<string, UnityAction<object>> EventActionDict { get; private set; }
        }

        private int _number = 1;
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                _text.text = value.ToString();
                if (_number == 0) BodyPart.RemoveStatusEffect(this);
            }
        }

        private void Awake()
        {
            Number = _number;
        }

        private void OnDisable()
        {
            if (_statusType == StatusType.NONE) return;
            foreach (var pair in _typeParameters.EventActionDict)
            {
                EventManager.Instance.StopListening(pair.Key, pair.Value);
            }
        }

        public void SetType(StatusType statusType)
        {
            if (statusType == StatusType.NONE) return;
            
            _statusType = statusType;
            ObtainTypeParameters();
            _image.sprite = _typeParameters.Sprite;
            foreach (var pair in _typeParameters.EventActionDict)
            {
                EventManager.Instance.StartListening(pair.Key, pair.Value);
            }
        }

        public StatusType GetStatusType()
        {
            return _statusType;
        }

        /**
         * puts the new TypeParameters in a variable
         */
        private void ObtainTypeParameters()
        {
            switch (_statusType)
            {
                case StatusType.BLEED:
                    UnityAction<object> takeBleedDamage = obj =>
                    {
                        BodyPart.RemoveHealth(Number);
                        Number -= 1;
                    };
                    _typeParameters = new TypeParameters("Bleed",
                        new Dictionary<string, UnityAction<object>>() { { EventManager.EVENT__END_TURN, takeBleedDamage } }
                    );
                    return;
                case StatusType.KNIFE:
                    UnityAction<object> inflictBleed = obj =>
                    {
                        var attack = (EventManager.AttackStruct)obj;
                        if (attack._attackingHealthBar == BodyPart)
                            attack._affectedHealthBar.AddStatusEffect(StatusType.BLEED, Number);
                    };
                    _typeParameters = new TypeParameters("BloodyKnife",
                        new Dictionary<string, UnityAction<object>>() { { EventManager.EVENT__HIT, inflictBleed } }
                        );
                    return;
                default:
                    throw new Exception($"No coded TypeParameter for type {_statusType}");
            }
        }
    }
}