using System.Collections.Generic;
using System.Text;
using cards;
using DefaultNamespace.StatusEffects;
using DefaultNamespace.UI;
using DefaultNamespace.Utility;
using UnityEditor;
using UnityEngine;

namespace cards
{
    [CreateAssetMenu(menuName = "Cards/Apply Status Effect Card")]
    public class ApplyStatusEffect : BasicCard
    {
        [SerializeField] protected BodyPartStatusEffect.StatusType _statusEffect;
        [SerializeField] protected int _amt;

        protected string _defaultDescriptionFormat = "Apply {0} stacks of {1} on the enemy's {2}.\n";

        protected ApplyStatusEffect()
        {
            _choiceOnEnemy = true;
        }

        public override void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
            List<Person.BodyPartEnum> affected_parts)
        {
            base.Play(user, attacking_parts, target, affected_parts);

            attacking_parts.ForEach(attacking_part => {
                affected_parts.ForEach(affected_part => AddStatusEffect(user, user.GetHealthBar(attacking_part), target, affected_part));
            });
        }

        protected virtual void AddStatusEffect(Person user, HealthBar attacking_part, Person target,
            Person.BodyPartEnum affected_part)
        {
            target.GetHealthBar(affected_part).AddStatusEffect(BodyPartStatusEffect.GetTypeOfStatusType(_statusEffect), _amt);
            
        }

        protected string GetStatusName()
        {
            return _statusEffect.ToString();
        }

        protected override string GenerateThisDescription()
        {
            var res = new StringBuilder(base.GenerateThisDescription());
            foreach (var targetType in CardChoices)
            {
                res.Append(FormattedSingleTargetTypeDescription(targetType));
            }

            return res.ToString();
        }
        
        /**
         * formats a single AttackerType's description
         */
        protected virtual string FormattedSingleTargetTypeDescription(CardChoiceEnum targetType)
        {
            return string.Format(_defaultDescriptionFormat,
                    _amt, GetStatusName(), targetType.ToString());
        }

        protected override void UpdateDescription()
        {
            base.UpdateDescription();
            _displayDescription = MyUtils.ReplaceAllBrackets(_displayDescription, "status", GetStatusName());
            _displayDescription = MyUtils.ReplaceAllBrackets(_displayDescription, "amount", _amt.ToString());
        }
    }
}