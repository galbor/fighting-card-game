using System.Collections.Generic;
using UnityEngine;

namespace cards
{
    [CreateAssetMenu(menuName = "Cards/Side to Side Card")]

    /**
     * Like a basic card, but if the affected part is X side then the attacking part is also X side
     */
    public class Side2SideAttackCard : BasicAttackCard
    {
        public override void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
            List<Person.BodyPartEnum> affected_parts)
        {
            Person.SideEnum side = Person.GetSide(affected_parts[0]);
            for (int i = 0; i < attacking_parts.Count; i++)
            {
                Person.BodyPartEnum attackingPart = attacking_parts[i];
                if (side == Person.SideEnum.RIGHT)
                {
                    if (attackingPart == Person.BodyPartEnum.LEFT_ARM)
                        attacking_parts[i] = Person.BodyPartEnum.RIGHT_ARM;
                    else if (attackingPart == Person.BodyPartEnum.LEFT_LEG)
                        attacking_parts[i] = Person.BodyPartEnum.RIGHT_LEG;
                }
                else if (side == Person.SideEnum.LEFT)
                {
                    if (attackingPart == Person.BodyPartEnum.RIGHT_ARM)
                        attacking_parts[i] = Person.BodyPartEnum.LEFT_ARM;
                    else if (attackingPart == Person.BodyPartEnum.RIGHT_LEG)
                        attacking_parts[i] = Person.BodyPartEnum.LEFT_LEG;
                }
            }

            base.Play(user, attacking_parts, target, affected_parts);
            // Attack(user, attacking_parts, target, affected_part);
            // PlayAbility(user, attacking_parts, target, affected_part);
        }
    }
}