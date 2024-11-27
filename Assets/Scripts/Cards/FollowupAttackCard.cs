using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace cards
{
    [CreateAssetMenu(menuName = "Cards/Follow-up Attack Card")]

    /**
     * gets 1 energy if the last attack was this side (L/R), deals double damage otherwise
     */
    public class FollowupAttackCard : BasicAttackCard
    {
        private Person.SideEnum _lastSide;
        private AttackStruct _lastHit;


        protected override AttackStruct[] Attack(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
            Person.BodyPartEnum affected_part)
        {
            var lastHit = Player.Instance.LastHit;
            var hits = base.Attack(user, attacking_parts, target, affected_part); //lastSide should change 

            for (int i = 0; i < hits.Length; i++)
            {
                var lastSide = Person.GetSide(lastHit._playerPart);
                var attackingSide = Person.GetSide(hits[i]._playerPart);

                if (attackingSide == lastSide)
                {
                    PlayerTurn.Instance.Energy++;
                }
                else if ((attackingSide == Person.SideEnum.RIGHT && lastSide == Person.SideEnum.LEFT) ||
                         (attackingSide == Person.SideEnum.LEFT && lastSide == Person.SideEnum.RIGHT))
                {
                    target.TakeDamage(hits[i]._enemyPart, hits[i]._damage);
                    hits[i]._damage *= 2;
                }

                lastHit = hits[i];
            }

            return hits;
        }


        protected override string GenerateThisDescription()
        {
            string res = base.GenerateThisDescription();
            res += "If the last attack this combat was this side, gain 1 energy. Otherwise deal double damage.\n";

            return res;
        }
    }
}