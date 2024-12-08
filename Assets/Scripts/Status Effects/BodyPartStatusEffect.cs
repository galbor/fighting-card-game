﻿using System;
using System.Collections.Generic;
using DefaultNamespace.UI;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using cards;

namespace DefaultNamespace.StatusEffects
{
    public class BodyPartStatusEffect : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [FormerlySerializedAs("_text")] [SerializeField] private TMP_Text _stacks;

        [SerializeField] private DescriptionViewer _description;

        public HealthBar BodyPart { get; set; }
        
        private StatusType _statusType;
        private TypeParameters _typeParameters;
        
        public enum StatusType
        {
            NONE,
            BLEED,
            KNIFE,
            SPIKES
        }

        private struct TypeParameters
        {
            public TypeParameters(string spriteName, Dictionary<string, UnityAction<object>> eventActionDict, string description)
            {
                Sprite = Resources.Load<Sprite>(spriteName);
                EventActionDict = eventActionDict;
                Description = description;
            }
            
            public Sprite Sprite { get; private set; }
            public Dictionary<string, UnityAction<object>> EventActionDict { get; private set; }
            public string Description { get; private set; }
        }

        private int _number = 1;
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                _stacks.text = value.ToString();
                if (_number == 0) BodyPart.RemoveStatusEffect(this);
            }
        }

        private void Awake()
        {
            Number = _number;
        }

        /**
         * disables listeners
         */
        public void Disable()
        {
            if (_statusType == StatusType.NONE) return;
            StopListening();
        }

        public void SetType(StatusType statusType)
        {
            if (statusType == StatusType.NONE) return;
            
            _statusType = statusType;
            ObtainTypeParameters();
            _image.sprite = _typeParameters.Sprite;
            StartListening();

            _description.Text = _typeParameters.Description;
        }

        private void StopListening()
        {
            foreach (var pair in _typeParameters.EventActionDict)
            {
                EventManager.Instance.StopListening(pair.Key, pair.Value);
            }
        }

        private void StartListening()
        {
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
                        new Dictionary<string, UnityAction<object>>() { { EventManager.EVENT__END_TURN, takeBleedDamage } },
                        "At the end of the turn, deals X damage and removes 1 stack"
                    );
                    return;
                case StatusType.KNIFE:
                    UnityAction<object> inflictBleed = obj =>
                    {
                        var attack = (BasicAttackCard.AttackStruct)obj;
                        if (attack.GetHealthBar(true) == BodyPart)
                            attack.GetHealthBar(false).AddStatusEffect(StatusType.BLEED, Number);
                    };
                    _typeParameters = new TypeParameters("BloodyKnife",
                        new Dictionary<string, UnityAction<object>>() { { EventManager.EVENT__HIT, inflictBleed } },
                        "When attacking, adds X bleed to the attacked body part"
                        );
                    return;
                case StatusType.SPIKES:
                    UnityAction<object> inflictSpikeDamage = obj =>
                    {
                        var attack = (BasicAttackCard.AttackStruct)obj;
                        if (attack.GetHealthBar(false) == BodyPart)
                            attack.GetHealthBar(true).RemoveHealth(Number);
                    };
                    _typeParameters = new TypeParameters("Spike",
                        new Dictionary<string, UnityAction<object>>() { { EventManager.EVENT__HIT, inflictSpikeDamage } },
                        "When attacked, deal X damage to attacking body part"
                    );
                    return;
                default:
                    throw new Exception($"No coded TypeParameter for type {_statusType}");
            }
        }
    }
}