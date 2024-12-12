using System;
using System.Collections.Generic;
using DefaultNamespace.UI;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using cards;
using DefaultNamespace.Utility;

namespace DefaultNamespace.StatusEffects
{
    public class BodyPartStatusEffect : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _stacks;

        [FormerlySerializedAs("_description")] [SerializeField] private DescriptionViewer _descriptionViewer;

        protected static Dictionary<Type, Pool<BodyPartStatusEffect>> _pools;

        protected Sprite _sprite;
        protected Dictionary<string, UnityAction<object>> _eventActionDict;
        protected string _description;

        public HealthBar BodyPart { get; set; }
        
        public Pool<BodyPartStatusEffect> Pool => _pools[GetStatusType()];

        public enum StatusType
        {
            KNIFE,
            SPIKE,
            INVINCIBLE
        }

        /**
         * returns inheritor type of BodyPartStatusEffect from StatusType enum
         */
        public static Type GetTypeOfStatusType(StatusType statusType) => statusType switch
        {
            StatusType.KNIFE =>
                typeof(KnifeStatusEffect),
            StatusType.SPIKE =>
                typeof(SpikeStatusEffect),
            StatusType.INVINCIBLE =>
                typeof(InvincibilityStatusEffect),
            _ => //default
                throw new Exception("StatusType doesn't appear in this switch-case")
        };

        private int _number = 1;
        public virtual int Number
        {
            get => _number;
            set
            {
                _number = value;
                _stacks.text = value.ToString();
                if (_number == 0) BodyPart.RemoveStatusEffect(this);
            }
        }

        protected virtual void Awake()
        {
            Number = _number;
            
            _pools ??= new Dictionary<Type, Pool<BodyPartStatusEffect>>(); //if null;
            if (!_pools.ContainsKey(GetType()))
            {
                _pools.Add(GetType(), new Pool<BodyPartStatusEffect>(this));
            }
        }

        /**
         * disables listeners
         */
        private void OnDisable()
        {
            StopListening();
        }

        public void OnEnable()
        {
            StartListening();
            
            _image.sprite = _sprite;
            _descriptionViewer.Text = _description;
        }

        private void StopListening()
        {
            foreach (var pair in _eventActionDict)
            {
                EventManager.Instance.StopListening(pair.Key, pair.Value);
            }
        }

        private void StartListening()
        {
            foreach (var pair in _eventActionDict)
            {
                EventManager.Instance.StartListening(pair.Key, pair.Value);
            }
        }

        public static Pool<BodyPartStatusEffect> GetPool<TStatus>() where TStatus : BodyPartStatusEffect
        {
            return _pools[typeof(TStatus)];
        }

        public static Pool<BodyPartStatusEffect> GetPool(BodyPartStatusEffect status)
        {
            return _pools[status.GetStatusType()];
        }

        public static Pool<BodyPartStatusEffect> GetPool(Type type)
        {
            return _pools[type];
        }

        public Type GetStatusType()
        {
            return GetType();
        }
    }
}