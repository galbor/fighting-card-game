using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Cards/Basic Card")]
public class BasicCard : ScriptableObject
{
    [SerializeField] private String _name = "";
    [SerializeField] private String _description = "";
    [SerializeField] Sprite _image;
    [SerializeField] int _cost = 1;
    [SerializeField] int _damage = 0;
    [SerializeField] int _bleed = 0;
    [SerializeField] int _defense = 0;
    [SerializeField] int _heal = 0;
    [FormerlySerializedAs("draw")] [SerializeField] int _draw = 0;
    [SerializeField] bool _singleEnemyTarget = true;
    [SerializeField] TargetTypeEnum _targetType = TargetTypeEnum.BODY_PART;
    [SerializeField] private Person.BodyPartEnum _preSelectedTarget;
    [SerializeField] AttackerTypeEnum[] _attackerType;
    
        
    private int _damageAdder = 0;
    private int _bleedAdder = 0;

    public TargetTypeEnum TargetType { get => _targetType; }
    public AttackerTypeEnum[] AttackerType { get => _attackerType; }
    public bool SingleEnemyTarget { get => _singleEnemyTarget; }
    
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

    private String _displayDescription;
    
    public String DisplayDescription
    {
        get
        {
            UpdateDescription();
            return _displayDescription;
        }
    }

    public Sprite Image { get => _image; }
    public String Name { get => _name; }

    public int Cost
    {
        get => _cost;
    }
    
    public enum TargetTypeEnum
    {
        PRE_SELECTED,
        BODY_PART,
        SIDE,
        UPPER_BODY
    }

    public enum AttackerTypeEnum
    {
        ARM,
        RIGHT_ARM,
        LEFT_ARM,
        LEG,
        RIGHT_LEG,
        LEFT_LEG,
        HEAD,
        TORSO
    }
    
    public static Person.BodyPartEnum GetBodyPart(AttackerTypeEnum attackerType)
    {
        switch (attackerType)
        {
            case AttackerTypeEnum.RIGHT_ARM:
                return Person.BodyPartEnum.RIGHT_ARM;
            case AttackerTypeEnum.LEFT_ARM:
                return Person.BodyPartEnum.LEFT_ARM;
            case AttackerTypeEnum.RIGHT_LEG:
                return Person.BodyPartEnum.RIGHT_LEG;
            case AttackerTypeEnum.LEFT_LEG:
                return Person.BodyPartEnum.LEFT_LEG;
            case AttackerTypeEnum.HEAD:
                return Person.BodyPartEnum.HEAD;
            case AttackerTypeEnum.TORSO:
                return Person.BodyPartEnum.TORSO;
            case AttackerTypeEnum.LEG:
                return Person.BodyPartEnum.NONE;
            case AttackerTypeEnum.ARM:
                return Person.BodyPartEnum.NONE;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    public class AttackModifier
    {
        public float DamageMultiplier;
        
    }

    private string ReplaceFirstOccurrence(string Source, string Find, string Replace)
    {
        Find = "{" + Find + "}";
        int Place = Source.IndexOf(Find);
        if (Place == -1)
            return Source;
        string result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
        return result;
    }
    
    
    public void UpdateDescription()
    {
        _displayDescription = _description;
        _displayDescription = ReplaceFirstOccurrence(_displayDescription, "damage", _damage.ToString());
        _displayDescription = ReplaceFirstOccurrence(_displayDescription, "bleed", _bleed.ToString());
        _displayDescription = ReplaceFirstOccurrence(_displayDescription, "defense", _defense.ToString());
        _displayDescription = ReplaceFirstOccurrence(_displayDescription, "heal", _heal.ToString());
    }

    public void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
        Person.BodyPartEnum affected_part)
    {
        Attack(user, attacking_parts, target, affected_part);
        PlayAbility(user, attacking_parts, target, affected_part);
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
                target.TakeDamage(affected_part, user.GetAttackDamage(attacking_part, _damage));
                target.Bleed(affected_part, user.GetAttackBleed(attacking_part, _bleed));
            }
    }

    public void PlayAbility(Person user, List<Person.BodyPartEnum> attacking_parts, Person target, Person.BodyPartEnum affected_part)
    {
        PlayerTurn playerTurn = PlayerTurn.Instance;
        for (int i = 0; i < _draw; i++)
        {
            if (!playerTurn.DrawCard()) break;
        }
        
        foreach (var part in attacking_parts)
            user.RemoveBlock(part);
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
