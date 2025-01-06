using Managers;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace.StatusEffects
{
    public class VulnerableStatusEffect : BodyPartStatusEffect
    {
        private int _percentageBonus = 40;

        public int PercentageBonus {  get => _percentageBonus; set => ChangeDamageMultiplier(value); }

        protected VulnerableStatusEffect() : base() { }

        protected new void Awake()
        {
            base.Awake();
            _eventActionDict.Add(
                EventManager.EVENT__START_TURN, obj => Number--);//enemies attack after start turn
            FormatDescription();
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
                    ChangePercentageBonusFromValue(0, PercentageBonus);
            }
        }


        private void ChangeDamageMultiplier(int value)
        {
            int prevMultiplier = _percentageBonus;
            _percentageBonus = value;
            ChangePercentageBonusFromValue(_percentageBonus, prevMultiplier);
            FormatDescription();
        }

        private void ChangePercentageBonusFromValue(int newValue, int prevValue)
        {
            if (BodyPart == null) return;
            BodyPart.HitDamageTakenMultiplier *= ((float)(100+newValue) / (100+prevValue));
        }

        public override void OnFirstAdded()
        {
            base.OnFirstAdded();
            ChangePercentageBonusFromValue(PercentageBonus, 0);
        }

        //sets the description to display the correct percentage bonus
        private void FormatDescription()
        {
            _description = string.Format("Take {0}% extra damage for X turns", PercentageBonus);
        }
    }
}