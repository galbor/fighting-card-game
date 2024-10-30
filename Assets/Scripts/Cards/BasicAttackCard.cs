using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    
    public override void UpdateDescription()
    {
        base.UpdateDescription();
        _displayDescription = MyUtils.ReplaceFirstOccurrence(_displayDescription, "damage", _damage.ToString());
        _displayDescription = MyUtils.ReplaceFirstOccurrence(_displayDescription, "bleed", _bleed.ToString());
    }

    public override void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
        Person.BodyPartEnum affected_part)
    {
        base.Play(user, attacking_parts, target, affected_part);
        Attack(user, attacking_parts, target, affected_part);
    }
    
    public void Attack(Person user, List<Person.BodyPartEnum> attacking_parts, Person target, Person.BodyPartEnum affected_part)
    {
        if (_targetType == TargetTypeEnum.PRE_SELECTED)
            affected_part = _preSelectedTarget;
        if (_targetType == TargetTypeEnum.SIDE && (_preSelectedTarget == Person.BodyPartEnum.LEFT_LEG || _preSelectedTarget == Person.BodyPartEnum.RIGHT_LEG))
        {
            if (affected_part == Person.BodyPartEnum.LEFT_ARM) affected_part = Person.BodyPartEnum.LEFT_LEG;
            else affected_part = Person.BodyPartEnum.RIGHT_LEG;
        }
        foreach (var attacking_part in attacking_parts)
            {
                user.RemoveProtection(attacking_part);
                
                target.TakeDamage(affected_part, user.GetAttackDamage(attacking_part, _damage));
                target.Bleed(affected_part, user.GetAttackBleed(attacking_part, _bleed));
            }
    }
    //
    // public void Defend(Person player, Person.BodyPartEnum[] targetBodyParts){
    //     foreach (Person.BodyPartEnum targetBodyPart in targetBodyParts)
    //     {
    //         player.Defend(targetBodyPart, _defense);
    //     }
    // }
    //
    // public void Heal(Person player, Person.BodyPartEnum[] targetBodyParts){
    //     foreach (Person.BodyPartEnum targetBodyPart in targetBodyParts)
    //     {
    //         player.Heal(targetBodyPart, _heal);
    //     }
    // }
}
