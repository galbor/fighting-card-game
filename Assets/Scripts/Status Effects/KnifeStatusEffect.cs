
using System.Collections.Generic;
using cards;
using DefaultNamespace.Utility;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace.StatusEffects
{
    public class KnifeStatusEffect : BodyPartStatusEffect
    {
        protected KnifeStatusEffect() : base() { }
        
        protected new void Awake()
        {
            base.Awake();
            _sprite = Resources.Load<Sprite>("BloodyKnife");
            _eventActionDict = new Dictionary<string, UnityAction<object>> { { EventManager.EVENT__HIT, InflictBleed } };
            _description = "When attacking, adds X bleed to the attacked body part";
        }

        private void InflictBleed(object obj)
        {
            var attack = (BasicAttackCard.AttackStruct)obj;
            if (attack.GetHealthBar(true) == BodyPart)
                attack.GetHealthBar(false).AddStatusEffect(typeof(BleedStatusEffect), Number);
        }
    }
}