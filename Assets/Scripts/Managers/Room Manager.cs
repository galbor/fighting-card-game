using System.Linq;
using UnityEngine;

namespace Managers
{
    public class RoomManager : Singleton<RoomManager>
    {
        [SerializeField] private Room[] _rooms;
        [SerializeField] private Transform _enemyParent;
        [SerializeField] private Vector2 _center;
        [SerializeField] private float _spacing;
        [SerializeField] private Camera _camera;
        private Enemy[] _enemies;
        private int _roomIndex = -1;

        private BasicAttackCard _prize;

        protected RoomManager()
        {

        }

        void Awake()
        {
            // SetNextRoom();
        }

        public void SetNextRoom()
        {
            _roomIndex++;
            SetRoom(_roomIndex);
        }

        private void SetRoom(int roomIndex)
        {
            _enemies = new Enemy[_rooms[roomIndex].Enemies.Length];
            for (int i = 0; i < _enemies.Length; i++)
            {
                _enemies[i] = Instantiate(_rooms[roomIndex].Enemies[i]);
                _enemies[i].Init(_enemyParent);
            }

            PlaceEnemies();
        }

        private void PlaceEnemies()
        {
            for (int i = 0; i < _enemies.Length; i++)
            {
                _enemies[i].Person.transform.position = _camera.WorldToScreenPoint(
                    new Vector3(_center.x + (i - (float)(_enemies.Length - 1) / 2) * _spacing, _center.y, 0));
                _enemies[i].Person.SetEnemyNumber(i + 1);
                _enemies[i].Person.SetEnemyNumberActive(false);
            }
        }

        private void KillEnemy(int index)
        {
            KillEnemy(_enemies[index].Person);
        }

        public bool KillEnemy(Person person)
        {
            int enemyAmt = _enemies.Length;
            _enemies = _enemies.Where(enemy => enemy.Person != person).ToArray();
            if (enemyAmt == _enemies.Length) return false;

            PlayerTurn.Instance.GetEnemies(this);
            PlaceEnemies();

            // if (_enemies.Length == 0)
            // {
            //     RoomWin();
            // }

            return true;
        }

        /**
         * Triggers RoomWin() iff all enemies are dead
         */
        public bool CheckRoomWin()
        {
            if (_enemies.Length == 0)
                RoomWin();
            return _enemies.Length == 0;
        }

        private void RoomWin()
        {
            EventManager.Instance.TriggerEvent(EventManager.EVENT__END_COMBAT, null);

            CardDraftManager.Instance.StartCardDraft();
        }

        public Enemy[] Enemies => _enemies;
    }
}