using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Room")] 
public class Room : ScriptableObject
{
    [SerializeField] private Enemy[] _enemies;

    // public void Awake()
    // {
    //     for (int i = 0; i < _enemies.Length; i++)
    //     {
    //         _enemies[i].Init();
    //     }
    // }

    public Enemy[] Enemies
    {
        get
        {
            return _enemies;
        }
    }
}