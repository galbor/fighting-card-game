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
        private Dictionary<Type, TypeParameters> _typeParametersMap;
        
        public enum Type
        {
            None,
            BLEED
        }

        private struct TypeParameters
        {
            public TypeParameters(Sprite sprite, Action<object> onEnable, Action<object> onDisable)
            {
                _sprite = sprite;
                _onEnableAction = onEnable;
                _onDisableAction = onDisable;
            }
            
            public Sprite _sprite;
            public Action<object> _onEnableAction;
            public Action<object> _onDisableAction;
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
            _typeParametersMap = new Dictionary<Type, TypeParameters>();
        }

        private void OnDisable()
        {
            if (_type == Type.None) return;
            GetTypeParameters(_type)._onDisableAction(null);
        }

        public void SetType(Type type)
        {
            _type = type;
            var _params = GetTypeParameters(_type);
            _params._onEnableAction(null);
            _image.sprite = _params._sprite;
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
                    UnityAction<object> action = obj =>
                    {
                        BodyPart.RemoveHealth(Number);
                        Number -= 1;
                    };
                    return new TypeParameters(
                        Resources.Load<Sprite>("Bleed"),
                        x => EventManagerScript.Instance.StartListening(EventManagerScript.EVENT__END_TURN, action),
                        x => EventManagerScript.Instance.StopListening(EventManagerScript.EVENT__END_TURN, action)
                    );
                default:
                    throw new Exception($"No coded TypeParameter for type {type}");
            }
        }
    }
}