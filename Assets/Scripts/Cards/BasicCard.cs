using System;
using System.Collections.Generic;
using Managers;
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
    
    [SerializeField] protected bool _singleEnemyTarget = true;
    [SerializeField] protected TargetTypeEnum _targetType = TargetTypeEnum.BODY_PART;
    [SerializeField] protected Person.BodyPartEnum _preSelectedTarget;
    [SerializeField] AttackerTypeEnum[] _attackerType = Array.Empty<AttackerTypeEnum>();

    [SerializeField] private List<BasicCard> _cardsToPlay;
    
    
    public TargetTypeEnum TargetType { get => _targetType; }
    public Person.BodyPartEnum PreSelectedTarget {get => _preSelectedTarget;}
    public AttackerTypeEnum[] AttackerType { get => _attackerType; protected set => _attackerType = value; }
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

    protected void Awake()
    {
        _cardsToPlay ??= new List<BasicCard>();
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
        PlayerTurn playerTurn = PlayerTurn.Instance;
        for (int i = 0; i < _draw; i++)
        {
            if (!playerTurn.DrawCard()) break;
        }
    }

    /**
     * plays all cards in the _cardsToPlay list
     */
    public void PlayExtraCards(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
        Person.BodyPartEnum affected_part)
    {
        _cardsToPlay.ForEach(card => card.Play(user, attacking_parts, target, affected_part));
    }

    /**
     * either generates description or takes the description from the _description field if it's filled
     */
    protected virtual string GetThisDescription()
    {
        if (_description == "") return GenerateThisDescription();
        return _description.Replace("\\n", "\n");;
    }

    /**
     * creates a description from known parameters of the card and its class
     */
    protected virtual string GenerateThisDescription()
    {
        StringBuilder res = new StringBuilder();
        if (_draw > 0) res.AppendFormat("Draw {0} cards.\n", _draw);

        return res.ToString();
    }
    
    protected virtual void UpdateDescription()
    {
        // _displayDescription = _description.Replace("\\n", "\n");
        _displayDescription = GenerateDescription();
    }

    /**
     * generates description from this card's description and the cards it plays' descriptions2
     */
    private String GenerateDescription()
    {
        var res = new StringBuilder(GetThisDescription());
        foreach (var card in _cardsToPlay)
        {
            res.Append(card.GetThisDescription());
        }

        return res.ToString();
    }

    protected string TargetTypeName(TargetTypeEnum targetType)
    {
        switch (targetType)
        {
            case TargetTypeEnum.UPPER_BODY:
                return "Upper body";
            case TargetTypeEnum.BODY_PART:
                return "Any body part";
            case TargetTypeEnum.PRE_SELECTED:
                return PreSelectedTarget.ToString();
            case TargetTypeEnum.SIDE:
                if (PreSelectedTarget == Person.BodyPartEnum.LEFT_LEG ||
                    PreSelectedTarget == Person.BodyPartEnum.RIGHT_LEG)
                    return "Leg";
                return "Arm";
        }

        throw new ArgumentException("target type has no name");
    }
}