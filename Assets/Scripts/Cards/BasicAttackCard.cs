using System;
using System.Collections.Generic;
using System.Text;
using Managers;
using DefaultNamespace.Utility;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Basic Attack Card")]
public class BasicAttackCard : BasicCard
{
    [SerializeField] int _damage = 0;
    [SerializeField] int _bleed = 0;
    
    private int _damageAdder = 0;
    private int _bleedAdder = 0;
    
    public int Damage
    {
        get => _damage;
        set {_damage = value;
            UpdateDescription();
        }
    }
    
    public int Bleed
    {
        get => _bleed;
        set {_bleed = value;
            UpdateDescription();
        }
    }
    
    public int DamageAdder
    {
        get => _damageAdder;
        set {_damageAdder = value;
            UpdateDescription();
        }
    }
    
    public int BleedAdder
    {
        get => _bleedAdder;
        set {_bleedAdder = value;
            UpdateDescription();
        }
    }

    public class AttackModifier
    {
        public float DamageMultiplier;
        
    }
    
    
    protected override void UpdateDescription()
    {
        base.UpdateDescription();
        _displayDescription = MyUtils.ReplaceAllBrackets(_displayDescription, "damage", Damage.ToString());
        _displayDescription = MyUtils.ReplaceAllBrackets(_displayDescription, "bleed", Bleed.ToString());
    }

    protected override string GenerateThisDescription()
    {
        StringBuilder res = new StringBuilder(base.GenerateThisDescription());
        for (int i = 0; i < AttackerType.Length; i++)
        {
            if (i > 0) res.Append(", ");
            res.AppendFormat("{0}", AttackerTypeName(AttackerType[i]));
        }
        res.AppendFormat(" the enemy's "); //_singleEnemyTarget
        res.AppendFormat("{0}.\n", TargetTypeName(TargetType));
        
        if (Damage > 0) res.AppendFormat("Deal {0} damage. ", Damage);
        if (Bleed > 0) res.AppendFormat("Apply {0} bleed.", Bleed);
        res.Append("\n");

        return res.ToString();
    }

    public override void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
        Person.BodyPartEnum affected_part)
    {
        base.Play(user, attacking_parts, target, affected_part);
        Attack(user, attacking_parts, target, affected_part);
    }
    
    public void Attack(Person user, List<Person.BodyPartEnum> attacking_parts, Person target, Person.BodyPartEnum affected_part)
    {
        if (TargetType == TargetTypeEnum.SIDE && (PreSelectedTarget == Person.BodyPartEnum.LEFT_LEG || PreSelectedTarget == Person.BodyPartEnum.RIGHT_LEG))
        {
            if (affected_part == Person.BodyPartEnum.LEFT_ARM) affected_part = Person.BodyPartEnum.LEFT_LEG;
            else affected_part = Person.BodyPartEnum.RIGHT_LEG;
        }
        else if (TargetType == TargetTypeEnum.PRE_SELECTED)
            affected_part = PreSelectedTarget;

        var userIsPlayer = user == Player.Instance.Person;
        attacking_parts.ForEach(attacking_part =>
        {
            if (user.GetHealthBar(attacking_part).Health == 0) return; //return is like continue in this case

            user.RemoveProtection(attacking_part);

            int hitDamage = user.GetAttackDamage(attacking_part, Damage);

            var cur_affected_part = target.TakeDamage(affected_part, hitDamage);
            target.Bleed(cur_affected_part, user.GetAttackBleed(attacking_part, Bleed));

            EventManager.Instance.TriggerEvent(EventManager.EVENT__HIT,
                new EventManager.AttackStruct(
                    userIsPlayer ? target : user,
                    userIsPlayer ? attacking_part : cur_affected_part,
                    userIsPlayer ? cur_affected_part : attacking_part,
                    hitDamage,
                    userIsPlayer
                ));
        });
    }


    private string AttackerTypeName(AttackerTypeEnum attackerType)
    {
        switch (attackerType)
        {
            case AttackerTypeEnum.ARM:
                return "Punch";
            case AttackerTypeEnum.LEG:
                return "Kick";
            case AttackerTypeEnum.HEAD:
                return "Headbutt";
            case AttackerTypeEnum.TORSO:
                return "Chestbump";
            case AttackerTypeEnum.LEFT_ARM:
                return "Left punch";
            case AttackerTypeEnum.RIGHT_ARM:
                return "Right punch";
            case AttackerTypeEnum.LEFT_LEG:
                return "Left kick";
            case AttackerTypeEnum.RIGHT_LEG:
                return "Right kick";
        }

        throw new ArgumentException("attacker type has no name");
    }
}
