using System.Collections.Generic;
using System.Text;
using DefaultNamespace.StatusEffects;
using DefaultNamespace.Utility;
using UnityEngine;

namespace cards
{
    [CreateAssetMenu(menuName = "Cards/Gain Status Effect Card")]
    public class GainStatusEffectCard : BasicCard
    {
        [SerializeField] private BodyPartStatusEffect.StatusType _statusEffect;
        [SerializeField] private int _amt;

        private string GetStatusName()
        {
            return _statusEffect.ToString();
        }
        
        public override void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
            Person.BodyPartEnum affected_part)
        {
            base.Play(user, attacking_parts, target, affected_part);
            
            attacking_parts.ForEach(x => user.GetHealthBar(x).AddStatusEffect(_statusEffect, _amt));
        }
        
        protected override void UpdateDescription()
        {
            base.UpdateDescription();
            _displayDescription = MyUtils.ReplaceAllBrackets(_displayDescription, "status", GetStatusName());
            _displayDescription = MyUtils.ReplaceAllBrackets(_displayDescription, "amount", _amt.ToString());
        }

        protected override string GenerateThisDescription()
        {
            var res = new StringBuilder(base.GenerateThisDescription());
            foreach (var attackerType in AttackerType)
            {
                res.AppendFormat("Gain {0} stacks of {1} on your {2}.\n",
                    _amt , GetStatusName(), attackerType.ToString());
            }

            return res.ToString();
        }
    }
}