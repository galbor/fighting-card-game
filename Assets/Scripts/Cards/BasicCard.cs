using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Basic Card")]
public class BasicCard : ScriptableObject
{
    [SerializeField] private string _name = "";
    [SerializeField] private string _description = "";
    [SerializeField] Sprite _image;
    [SerializeField] int _cost = 1;
    [SerializeField] int _draw = 0;
    
    [SerializeField] bool _singleEnemyTarget = true;
    [SerializeField] protected TargetTypeEnum _targetType = TargetTypeEnum.BODY_PART;
    [SerializeField] protected Person.BodyPartEnum _preSelectedTarget;
    [SerializeField] AttackerTypeEnum[] _attackerType;

    [SerializeField] private List<BasicCard> _cardsToPlay;
    
    
    public TargetTypeEnum TargetType { get => _targetType; }
    public Person.BodyPartEnum PreSelectedTarget {get => _preSelectedTarget;}
    public AttackerTypeEnum[] AttackerType { get => _attackerType; }
    public bool SingleEnemyTarget { get => _singleEnemyTarget; }
        
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

    private void Awake()
    {
        if (_cardsToPlay == null) _cardsToPlay = new List<BasicCard>();
    }

    protected string _displayDescription;
    
    public string DisplayDescription
    {
        get
        {
            UpdateDescription();
            return _displayDescription;
        }
    }

    public Sprite Image { get => _image; }
    public string Name { get => _name; }

    public int Cost
    {
        get => _cost;
    }

    public virtual void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
        Person.BodyPartEnum affected_part)
    {
        PlayAbility(user, attacking_parts, target, affected_part);
        _cardsToPlay.ForEach(card => card.Play(user, attacking_parts, target, affected_part));
    }
    
    public void PlayAbility(Person user, List<Person.BodyPartEnum> attacking_parts, Person target, Person.BodyPartEnum affected_part)
    {
        PlayerTurn playerTurn = PlayerTurn.Instance;
        for (int i = 0; i < _draw; i++)
        {
            if (!playerTurn.DrawCard()) break;
        }
    }
    
    
    public virtual void UpdateDescription()
    {
        string[] descriptionarray = _description.Split("\\n");
        StringBuilder descriptionbuilder = new StringBuilder();
        foreach (var str in descriptionarray)
        {
            descriptionbuilder.Append(str);
            descriptionbuilder.Append('\n');
        }
        _displayDescription = descriptionbuilder.ToString();
    }
}