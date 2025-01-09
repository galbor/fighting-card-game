using System;
using System.Collections.Generic;
using System.Text;
using DefaultNamespace;
using Managers;
using DefaultNamespace.Utility;
using UnityEngine;

namespace cards
{
    [CreateAssetMenu(menuName = "Cards/Basic Attack Card")]
    public class BasicAttackCard : BasicCard
    {
        [SerializeField] int _damage = 0;
        [SerializeField] int _bleed = 0;

        protected BasicAttackCard()
        {
            _choiceOnEnemy = true;
        }


        public int Damage
        {
            get => _damage;
            set
            {
                _damage = value;
                UpdateDescription();
            }
        }

        public int Bleed
        {
            get => _bleed;
            set
            {
                _bleed = value;
                UpdateDescription();
            }
        }


        protected override string FormatDescription(string unformattedStr)
        {
            string res = base.FormatDescription(unformattedStr);
            for (int i = 0; i < PreSelectedChoices.Length; i++) {
                var damage = Player.Instance.Person.GetAttackDamage(PreSelectedChoices[i], Damage);
                res = MyUtils.ReplaceFirstBracket(res, "damage", damage.ToString());
                res = MyUtils.ReplaceFirstBracket(res, "base_damage", Damage.ToString());
            }
            res = MyUtils.ReplaceAllBrackets(res, "bleed", Bleed.ToString());
            return res;
        }

        protected override string GenerateThisDefaultUnformattedDescription()
        {
            StringBuilder res = new StringBuilder(base.GenerateThisDefaultUnformattedDescription());
            for (int i = 0; i < PreSelectedChoices.Length; i++)
            {
                if (i > 0) res.Append(", ");
                res.AppendFormat("{0}", AttackerTypeName(PreSelectedChoices[i]));
            }

            res.AppendFormat(" the enemy's "); //_singleEnemyTarget
            for (int i = 0; i < CardChoices.Length; i++)
            {
                if (i > 0) res.Append(", ");
                res.AppendFormat("{0}", CardChoices[i]);
            }
            res.Append(".\n");

            if (Damage > 0)
            {
                for (int i = 0; i < PreSelectedChoices.Length; i++)
                {
                    res.Append("Deal ({base_damage}) {damage} damage. ");
                }
            }
            if (Bleed > 0) res.Append("Apply {bleed} bleed.");
            res.Append("\n");

            return res.ToString();
        }

        public override void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
            List<Person.BodyPartEnum> affected_parts)
        {
            base.Play(user, attacking_parts, target, affected_parts);

            attacking_parts.ForEach(attacking_part =>
            {
                affected_parts.ForEach(affected_part =>
                {
                    AttackStruct hit = Attack(user, attacking_part, target, affected_part);
                    if (!hit.IsNone()) EventManager.Instance.TriggerEvent(EventManager.EVENT__HIT, hit);
                }); 
            });
        }

        protected virtual AttackStruct Attack(Person user, Person.BodyPartEnum attacking_part, Person target,
            Person.BodyPartEnum affected_part)
        {
            var userIsPlayer = user == Player.Instance.Person;
            if (user.GetHealthBar(attacking_part).Health == 0) return AttackStruct.None; //return is like continue in this case

            user.RemoveProtection(attacking_part);

            int hitDamage = AttackDamage(user, attacking_part);

            var cur_affected_part = target.TakeDamage(affected_part, hitDamage);
            target.Bleed(cur_affected_part, user.GetAttackBleed(attacking_part, Bleed));

            return new AttackStruct(
                userIsPlayer ? target : user,
                userIsPlayer ? attacking_part : cur_affected_part,
                userIsPlayer ? cur_affected_part : attacking_part,
                hitDamage,
                userIsPlayer);
        }

        /**
         * @returns would-be attack damage
         */
        protected virtual int AttackDamage(Person user, Person.BodyPartEnum attacking_part)
        {
            return user.GetAttackDamage(attacking_part, Damage);
        }


        private string AttackerTypeName(Person.BodyPartEnum bodyPart)
        {
            switch (bodyPart)
            {
                case Person.BodyPartEnum.HEAD:
                    return "Headbutt";
                case Person.BodyPartEnum.TORSO:
                    return "Chestbump";
                case Person.BodyPartEnum.LEFT_ARM:
                    return "Left punch";
                case Person.BodyPartEnum.RIGHT_ARM:
                    return "Right punch";
                case Person.BodyPartEnum.LEFT_LEG:
                    return "Left kick";
                case Person.BodyPartEnum.RIGHT_LEG:
                    return "Right kick";
            }

            throw new ArgumentException("attacker type has no name");
        }
    }
}