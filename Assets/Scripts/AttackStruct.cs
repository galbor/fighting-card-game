using DefaultNamespace.UI;
using Managers;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public struct AttackStruct
    {
        public AttackStruct(Person enemy, Person.BodyPartEnum playerPart, Person.BodyPartEnum enemyPart, int damage,
            bool playerAttacker)
        {
            _enemy = enemy;
            _playerPart = playerPart;
            _enemyPart = enemyPart;
            _damage = damage;
            _playerAttacker = playerAttacker;
        }

        /**
         * factory method
         * returns attackStruct that is equivalent to null
         */
        public static AttackStruct None => new AttackStruct(null, Person.BodyPartEnum.NONE, Person.BodyPartEnum.NONE, -1, true);

        public Person _enemy;
        public Person.BodyPartEnum _playerPart;
        public Person.BodyPartEnum _enemyPart;
        public int _damage;
        public bool _playerAttacker; //true if attacker is player

        /**
         * if true get attacker health bar, otherwise the affected health bar
         */
        public HealthBar GetHealthBar(bool attacker)
        {
            if (_playerAttacker ^ !attacker)
                return Player.Instance.Person.GetHealthBar(_playerPart); // a XOR !b    ===    a <=> b
            return _enemy.GetHealthBar(_enemyPart);
        }

        /**
         * if true get the attacker's Person, otherwise get the victim's Person
         */
        public Person GetPerson(bool attacker)
        {
            return _playerAttacker ^ attacker ? _enemy : Player.Instance.Person;
        }

        /**
         * returns true iff the AttackStruct is fake
         */
        public bool IsNone()
        {
            return _damage < 0 || _enemyPart == Person.BodyPartEnum.NONE;
        }
    }
}