using Managers;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace.StatusEffects
{
    public class VulnerableStatusEffect : BodyPartStatusEffect
    {
        private float _damageMultiplier = 1.4f;

        public float DamageMultiplier {  get => _damageMultiplier; set => ChangeDamageMultiplier(value); }

        protected VulnerableStatusEffect() : base() { }

        protected new void Awake()
        {
            base.Awake();
            _eventActionDict = new Dictionary<string, UnityAction<object>>
            { { EventManager.EVENT__START_TURN, obj => Number--}};//enemies attack after start turn
            _description = string.Format("This body part takes {0}% extra damage for X turns", (int)(DamageMultiplier*100-100));
        }

        public override int Number
        {
            get => base.Number;
            set
            {
                int prevValue = base.Number;
                base.Number = value;
                if (BodyPart == null) return;
                if (value <= 0 && prevValue > 0) //if becomes 0
                    ChangeDamageMultiplierFromValue(1f, DamageMultiplier);
            }
        }


        private void ChangeDamageMultiplier(float value)
        {
            float prevMultiplier = _damageMultiplier;
            _damageMultiplier = value;
            ChangeDamageMultiplierFromValue(_damageMultiplier, prevMultiplier);
        }

        private void ChangeDamageMultiplierFromValue(float newValue, float prevValue)
        {
            if (BodyPart == null) return;
            BodyPart.HitDamageMultiplier *= (newValue / prevValue);
        }

        public override void OnFirstAdded()
        {
            base.OnFirstAdded();
            ChangeDamageMultiplierFromValue(DamageMultiplier, 1f);
        }
    }
}