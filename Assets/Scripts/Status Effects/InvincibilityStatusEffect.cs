using System.Collections.Generic;
using cards;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace.StatusEffects
{
    public class InvincibilityStatusEffect : BodyPartStatusEffect
    {
        protected new void Awake()
        {
            base.Awake();
            _eventActionDict.Add(
                EventManager.EVENT__HIT, LoseStackOnAttack);
            _eventActionDict.Add(
                EventManager.EVENT__START_TURN, obj => Number--); //enemies attack after EVENT__END_TURN
                //if I want a relic that gives invincibility, give 1 extra
            _description = "When attacked, lose 1 stack instead of HP.\nLose one stack each turn.";
        }


        public override int Number
        {
            get => base.Number;
            set
            {
                base.Number = value;
                if (BodyPart != null)
                    BodyPart.Invincibility = value;
            }
        }

        private void LoseStackOnAttack(object obj)
        {
            var attack = (BasicAttackCard.AttackStruct)obj;
            if (attack.GetHealthBar(false) == BodyPart) Number--;
        }
    }
}