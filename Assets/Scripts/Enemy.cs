using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    [SerializeField] private string _personPrefab;
    [SerializeField] private int _maxHealth;
    [SerializeField] private string _name;
    
    [SerializeField] private AttackPattern _attackPattern;
    [SerializeField] private Action[] _possibleActions;
    
    private Person _person;
    private int _lastAttack = -1;
    private BasicCard _attack;
    
    public int MaxHealth { get => _maxHealth; }
    
    //TODO attack pattern
    
    public Person Person {
        get
        {
            return _person;
        }
        set => _person = value;
    }
    public string Name { get => _name; }
    
    public void Init()
    {
        _person = Resources.Load<Person>(_personPrefab);
        _attack = ScriptableObject.CreateInstance<BasicCard>();
    }

    public void Attack()
    {
        if (!_person.IsAlive()) return;
        int action = GetNextAction();
        _attack.Damage = _possibleActions[action].Damage;
        _attack.Bleed = _possibleActions[action].Bleed;
        _attack.Play(_person, _possibleActions[action].AttackingParts.ToList(), Player.Instance.Person, _possibleActions[action].AffectedPart);
        _lastAttack = action;
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