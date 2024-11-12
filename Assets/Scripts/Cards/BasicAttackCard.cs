using System.Collections.Generic;
using DefaultNamespace;
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
        if (_targetType == TargetTypeEnum.SIDE && (_preSelectedTarget == Person.BodyPartEnum.LEFT_LEG || _preSelectedTarget == Person.BodyPartEnum.RIGHT_LEG))
        {
            if (affected_part == Person.BodyPartEnum.LEFT_ARM) affected_part = Person.BodyPartEnum.LEFT_LEG;
            else affected_part = Person.BodyPartEnum.RIGHT_LEG;
        }

        bool userIsPlayer = user == Player.Instance.Person;
        foreach (var attacking_part in attacking_parts)
        {
            if (user.GetHealthBar(attacking_part).Health == 0) continue;
            
            user.RemoveProtection(attacking_part);

            int hitDamage = user.GetAttackDamage(attacking_part, _damage);
            
            var cur_affected_part = target.TakeDamage(affected_part, hitDamage);
            target.Bleed(cur_affected_part, user.GetAttackBleed(attacking_part, _bleed));
            
            EventManager.Instance.TriggerEvent(EventManager.EVENT__HIT, 
                new EventManager.AttackStruct(
                    userIsPlayer ? target : user,
                    userIsPlayer ? attacking_part : cur_affected_part,
                    userIsPlayer ? cur_affected_part : attacking_part,
                    hitDamage,
                    userIsPlayer
                ));
        }
    }
    // public void Heal(Person player, Person.BodyPartEnum[] targetBodyParts){
    //     foreach (Person.BodyPartEnum targetBodyPart in targetBodyParts)
    //     {
    //         player.Heal(targetBodyPart, _heal);
    //     }
    // }
}
