using UnityEngine;

namespace Managers
{
    public class MapManager : Singleton<MapManager>
    {
        protected MapManager() { }

        [SerializeField] private GameObject _map;

        public void SetMapActive(bool active)
        {
            _map.SetActive(active);
        }
    }
}