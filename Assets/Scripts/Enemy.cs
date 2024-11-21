using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using DefaultNamespace.StatusEffects;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    [SerializeField] private Person _personPrefab;
    [SerializeField] private int _maxHealth;
    [SerializeField] private string _name;
    
    [SerializeField] private AttackPattern _attackPattern;
    [SerializeField] private Action[] _possibleActions;

    [SerializeField] private List<StartingStatusEffect> _startingStatusEffects;

    [Serializable]
    private struct StartingStatusEffect
    {
        public Person.BodyPartEnum _bodyPart;
        public BodyPartStatusEffect.StatusType _statusEffect;
        public int _amt;

        public StartingStatusEffect(Person.BodyPartEnum bodyPart, BodyPartStatusEffect.StatusType statusEffect, int amt)
        {
            _bodyPart = bodyPart;
            _statusEffect = statusEffect;
            _amt = amt;
        }
    }
    
    private Person _person;
    private int _lastAttack = -1;
    private int _nextAttack;
    private BasicAttackCard _attack;
    
    public int MaxHealth { get => _maxHealth; }
    
    public Person Person {
        get
        {
            return _person;
        }
        set => _person = value;
    }
    public string Name { get => _name; }
    
    public void Init(Transform personParent)
    {
        _person = Instantiate(_personPrefab, personParent);
        _person.SetMaxHealth(MaxHealth);
        _attack = ScriptableObject.CreateInstance<BasicAttackCard>();
        ChooseAndDisplayNextAction();

        foreach (var triplet in _startingStatusEffects)
        {
            _person.GetHealthBar(triplet._bodyPart).AddStatusEffect(triplet._statusEffect, triplet._amt);
        }
        
                
        EventManager.Instance.StartListening(EventManager.EVENT__HIT, objHit =>
        {
            var attack = (BasicAttackCard.AttackStruct)objHit;
            if (attack.GetPerson(false) == Person)
                DisplayNextAction();
        });
    }

    public void Attack()
    {
        if (!_person.IsAlive()) return;
        
        _attack.Damage = _possibleActions[_nextAttack].Damage;
        _attack.Bleed = _possibleActions[_nextAttack].Bleed;
        _attack.Play(_person, _possibleActions[_nextAttack].AttackingParts.ToList(), Player.Instance.Person, _possibleActions[_nextAttack].AffectedPart);
        _lastAttack = _nextAttack;

        ChooseAndDisplayNextAction();
    }

    /**
     * chooses the next action and displays it on the PlannedAttackDisplay
     */
    private void ChooseAndDisplayNextAction()
    {
        _nextAttack = GetNextAction();
        DisplayNextAction();
    }

    private void DisplayNextAction()
    {
        var attack = _possibleActions[_nextAttack];
        _person.DisplayPlannedAttack(attack.AttackingParts[0],
            attack.AffectedPart
            , attack.Damage);
    }

    private int GetNextAction()
    {
        if (_possibleActions.Length == 1) return 0;
        switch (_attackPattern)
        {
            case AttackPattern.RANDOM:
                return GetRandomAction();
            case AttackPattern.RANDOM_NO_REPEAT:
                return GetRandomNoRepeatAction();
            case AttackPattern.SEQUENTIAL:
                return GetSequentialAction();
            default:
                return GetRandomAction();
        }
    }
    
    private int GetRandomAction()
    {
        return Random.Range(0, _possibleActions.Length);
    }
    
    private int GetRandomNoRepeatAction()
    {
        int action = Random.Range(0, _possibleActions.Length-1);
        if (action >= _lastAttack) action++;

        return action;
    }
    
    private int GetSequentialAction()
    {
        return (_lastAttack + 1) % _possibleActions.Length;
    }
    
    
    private enum AttackPattern
    {
        RANDOM,
        RANDOM_NO_REPEAT,
        SEQUENTIAL
    }
    
    [Serializable]
    private struct Action
    {
        public Person.BodyPartEnum[] AttackingParts;
        public Person.BodyPartEnum AffectedPart;
        public int Damage;
        public int Bleed;
    }
}