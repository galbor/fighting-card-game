using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefaultNamespace.StatusEffects;
using UnityEngine;

namespace cards
{
    [CreateAssetMenu(menuName = "Cards/Vampire Card")]
    class VampireCard : BasicCard
    {
        protected override string GenerateThisDefaultUnformattedDescription()
        {
            StringBuilder res = new StringBuilder(base.GenerateThisDefaultUnformattedDescription());

            res.AppendLine("Remove bleed from attacked body part and gain health equal to amount removed.");

            return res.ToString();
        }


        public override void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
            List<Person.BodyPartEnum> affected_parts)
        {
            var amt = affected_parts.Sum(part => target.GetHealthBar(part).RemoveStatusEffect<BleedStatusEffect>());
            user.Heal(amt);
            base.Play(user, attacking_parts, target, affected_parts);
        }
    }
}
