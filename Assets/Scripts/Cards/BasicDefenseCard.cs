using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefaultNamespace.Utility;
using UnityEngine;

namespace cards
{
    [CreateAssetMenu(menuName = "Cards/Basic Defense Card")]
    public class BasicDefenseCard : BasicCard
    {

        [SerializeField] private int _block;

        protected BasicDefenseCard()
        {
            _choiceOnEnemy = false;
            PreSelectedChoices = PreSelectedChoices.Take(1).ToArray();
        }

        public override void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
            List<Person.BodyPartEnum> affected_parts)
        {
            base.Play(user, attacking_parts, target, affected_parts);
            Block(user, attacking_parts, affected_parts[0]);
        }

        private void Block(Person user, List<Person.BodyPartEnum> attacking_parts, Person.BodyPartEnum affected_part)
        {
            attacking_parts.ForEach(x => user.Defend(x, _block));
            if (affected_part != Person.BodyPartEnum.NONE)
                attacking_parts.ForEach(x => user.SetProtection(x, affected_part));
        }

        protected override string FormatDescription(string unformattedStr)
        {
            string res = base.FormatDescription(unformattedStr);
            res = MyUtils.ReplaceAllBrackets(res, "block", _block.ToString());
            return res;
        }

        protected override string GenerateThisDefaultUnformattedDescription()
        {
            var res = new StringBuilder(base.GenerateThisDefaultUnformattedDescription());
            foreach (var protector in CardChoices)
            {
                res.AppendFormat("Defend your {0} ", PreSelectedChoices[0].ToString());
                res.AppendFormat("with your {0}. ", protector.ToString());
                res.Append("Gain {block} block.\n");
            }

            return res.ToString();
        }
    }
}