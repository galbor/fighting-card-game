using System.Collections.Generic;
using DefaultNamespace.Utility;
using Managers;
using Unity.Burst.CompilerServices;
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


        protected override AttackStruct Attack(Person user, Person.BodyPartEnum attacking_part, Person target,
            Person.BodyPartEnum affected_part)
        {
            var lastHit = Player.Instance.LastHit;
            var hit = base.Attack(user, attacking_part, target, affected_part);

            var lastSide = Person.GetSide(lastHit._playerPart);
            var attackingSide = Person.GetSide(attacking_part);

            if (attackingSide == lastSide)
            {
                PlayerTurn.Instance.Energy++;
            }

            return hit;
        }

        protected override int AttackDamage(Person user, Person.BodyPartEnum attacking_part)
        {
            var lastHit = Player.Instance.LastHit;
            var lastSide = Person.GetSide(lastHit._playerPart);
            var attackingSide = Person.GetSide(attacking_part);

            int mult = 1;

            if (MyUtils.OppositeSides(attackingSide, lastSide)) mult = 2;

            return base.AttackDamage(user, attacking_part) * mult;
        }


        protected override string GenerateThisDefaultUnformattedDescription()
        {
            string res = base.GenerateThisDefaultUnformattedDescription();
            res += "If the last attack this combat was this side, gain 1 energy. Otherwise deal double damage.\n";

            return res;
        }
    }
}