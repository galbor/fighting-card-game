using System.Collections.Generic;
using cards;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace.StatusEffects
{
    public class SpikeStatusEffect : BodyPartStatusEffect
    {
        protected SpikeStatusEffect() : base() { }
        
        protected new void Awake()
        {
            base.Awake();
            _sprite = Resources.Load<Sprite>("Status Effects/Spike");
            _eventActionDict = new Dictionary<string, UnityAction<object>> { { EventManager.EVENT__HIT, inflictSpikeDamage } };
            _description = "When attacked, deal X damage to attacking body part";
        }
        
        
        private void inflictSpikeDamage(object obj)
        {
            var attack = (BasicAttackCard.AttackStruct)obj;
            if (attack.GetHealthBar(false) == BodyPart)
                attack.GetHealthBar(true).RemoveHealth(Number);
        }
    }
}