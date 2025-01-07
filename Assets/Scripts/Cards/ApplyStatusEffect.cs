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

        protected string _defaultUnformattedDescription = "Apply {amt} stacks of {effect} on the enemy's {part}.\n";

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

        protected override string GenerateThisDefaultUnformattedDescription()
        {
            var res = new StringBuilder(base.GenerateThisDefaultUnformattedDescription());
            foreach (var targetType in CardChoices)
            {
                res.Append(MyUtils.ReplaceAllBrackets(_defaultUnformattedDescription, "part", targetType.ToString()));
            }

            return res.ToString();
        }

        protected override void UpdateDescription()
        {
            base.UpdateDescription();
        }

        protected override string FormatDescription(string unformattedStr)
        {
            string res = base.FormatDescription(unformattedStr);
            res = res = MyUtils.ReplaceAllBrackets(res, "effect", GetStatusName());
            res = MyUtils.ReplaceAllBrackets(res, "amt", _amt.ToString());
            return res;
        }
    }
}