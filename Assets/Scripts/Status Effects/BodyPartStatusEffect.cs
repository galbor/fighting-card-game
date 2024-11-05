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
        
        private Type _type;
        private static Dictionary<Type, TypeParameters> _typeParametersMap;
        
        public enum Type
        {
            None,
            BLEED,
            KNIFE
        }

        private struct TypeParameters
        {
            public TypeParameters(Sprite sprite, Dictionary<string, UnityAction<object>> eventActionDict)
            {
                _sprite = sprite;
                _eventActionDict = eventActionDict;
            }
            
            public Sprite _sprite;
            public Dictionary<string, UnityAction<object>> _eventActionDict;
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
            _typeParametersMap ??= new Dictionary<Type, TypeParameters>();
        }

        private void OnDisable()
        {
            if (_type == Type.None) return;
            foreach (var pair in GetTypeParameters(_type)._eventActionDict)
            {
                EventManager.Instance.StopListening(pair.Key, pair.Value);
            }
        }

        public void SetType(Type type)
        {
            _type = type;
            var typeParams = GetTypeParameters(_type);
            _image.sprite = typeParams._sprite;
            foreach (var pair in typeParams._eventActionDict)
            {
                EventManager.Instance.StartListening(pair.Key, pair.Value);
            }
        }

        public Type GetStatusType()
        {
            return _type;
        }

        private TypeParameters GetTypeParameters(Type type)
        {
            if (_typeParametersMap.TryGetValue(type, out var res))
                return res;
            switch (type)
            {
                case Type.BLEED:
                    UnityAction<object> takeBleedDamage = obj =>
                    {
                        BodyPart.RemoveHealth(Number);
                        Number -= 1;
                    };
                    return new TypeParameters(
                        Resources.Load<Sprite>("Bleed"),
                        new Dictionary<string, UnityAction<object>>() { { EventManager.EVENT__END_TURN, takeBleedDamage } }
                    );
                case Type.KNIFE:
                    UnityAction<object> inflictBleed = obj =>
                    {
                        var attack = (EventManager.AttackStruct)obj;
                        if (attack._attackingHealthBar == BodyPart)
                            attack._affectedHealthBar.AddStatusEffect(Type.BLEED, Number);
                    };
                    return new TypeParameters(
                        Resources.Load<Sprite>("BloodyKnife"),
                        new Dictionary<string, UnityAction<object>>() { { EventManager.EVENT__HIT, inflictBleed } }
                        );
                default:
                    throw new Exception($"No coded TypeParameter for type {type}");
            }
        }
    }
}