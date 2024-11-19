using UnityEngine;

namespace Managers
{
    public class MapManager : Singleton<MapManager>
    {
        protected MapManager() { }
        [SerializeField] private GameObject[] _possibleMaps;
        
        private GameObject _map;
        private int _currentMap = 0;
        
        public void SetMapActive(bool active)
        {
            _map.SetActive(active);
        }

        public void SetNextMap()
        {
            if (_currentMap > 0) Destroy(_map);
            _map = Instantiate(_possibleMaps[_currentMap]);
            _currentMap++;
            SetMapActive(true);
        }
    }
}