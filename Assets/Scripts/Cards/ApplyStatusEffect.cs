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

        public override void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
            Person.BodyPartEnum affected_part)
        {
            base.Play(user, attacking_parts, target, affected_part);

            attacking_parts.ForEach(x => AddStatusEffect(user, user.GetHealthBar(x), target, affected_part));
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
            foreach (var attackerType in AttackerType)
            {
                res.Append(FormattedSingleAttackerTypeDescription(attackerType));
            }

            return res.ToString();
        }
        
        /**
         * formats a single AttackerType's description
         */
        protected virtual string FormattedSingleAttackerTypeDescription(AttackerTypeEnum attackerType)
        {
            return string.Format(_defaultDescriptionFormat,
                    _amt, GetStatusName(), TargetType.ToString());
        }

        protected override void UpdateDescription()
        {
            base.UpdateDescription();
            _displayDescription = MyUtils.ReplaceAllBrackets(_displayDescription, "status", GetStatusName());
            _displayDescription = MyUtils.ReplaceAllBrackets(_displayDescription, "amount", _amt.ToString());
        }
    }
}