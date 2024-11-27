using System.Collections.Generic;
using System.Text;
using DefaultNamespace.Utility;
using UnityEngine;

namespace cards
{
    [CreateAssetMenu(menuName = "Cards/Basic Defense Card")]
    public class BasicDefenseCard : BasicCard
    {

        [SerializeField] private int _block;

        public override void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
            Person.BodyPartEnum affected_part)
        {
            base.Play(user, attacking_parts, target, affected_part);
            Block(user, attacking_parts, affected_part);
        }

        private void Block(Person user, List<Person.BodyPartEnum> attacking_parts, Person.BodyPartEnum affected_part)
        {
            attacking_parts.ForEach(x => user.Defend(x, _block));
            if (affected_part != Person.BodyPartEnum.NONE)
                attacking_parts.ForEach(x => user.SetProtection(x, affected_part));
        }

        protected override void UpdateDescription()
        {
            base.UpdateDescription();
            _displayDescription = MyUtils.ReplaceAllBrackets(_displayDescription, "block", _block.ToString());
        }

        protected override string GenerateThisDescription()
        {
            var res = new StringBuilder(base.GenerateThisDescription());
            foreach (var attackerType in AttackerType)
            {
                res.AppendFormat("Defend your {0} ", TargetTypeName(TargetType));
                res.AppendFormat("with your {0}.\n", attackerType.ToString());
                res.AppendFormat("Gain {0} block", _block);
            }

            return res.ToString();
        }
    }
}