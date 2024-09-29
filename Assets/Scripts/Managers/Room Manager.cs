using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;

public class RoomManager : Singleton<RoomManager>
{
    [SerializeField] private Room _room;
    [SerializeField] private Transform _enemyParent;
    [SerializeField] private Vector2 _center;
    [SerializeField] private float _spacing;
    private Enemy[] _enemies;

    private BasicAttackCard _prize;

    void Start ()
    {
        SetRoom(_room);
    }
    
    public void SetRoom(Room room)
    {
        _enemies = room.Enemies;
        for (int i = 0; i < _enemies.Length; i++)
        {
            _enemies[i].Init();
            Person enemy = Instantiate(_enemies[i].Person, _enemyParent);
            enemy.SetMaxHealth(_enemies[i].MaxHealth);
            _enemies[i].Person = enemy;
        }
        
        PlaceEnemies();
    }

    private void PlaceEnemies()
    {
        for (int i = 0; i < _enemies.Length; i++){
            _enemies[i].Person.transform.position = new Vector3(_center.x + (i - (float)(_enemies.Length-1)/2) * _spacing, _center.y, 0);
            _enemies[i].Person.gameObject.GetComponentInChildren<EnemyNumber>().SetNumber(i+1);
        }
    }

    private void KillEnemy(int index)
    {
        KillEnemy(_enemies[index].Person);
    }

    public void KillEnemy(Person person)
    {
        _enemies = _enemies.Where(enemy => enemy.Person != person).ToArray();
        PlayerTurn.Instance.GetEnemies(this);    
        PlaceEnemies();
    }

    public Enemy[] Enemies => _enemies;
}
