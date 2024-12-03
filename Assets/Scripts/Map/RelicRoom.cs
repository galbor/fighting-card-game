using Managers;
using Relics;
using UnityEngine;

namespace Map
{
    public class RelicRoom : MonoBehaviour
    {
        [SerializeField] private AbstractRelic _relic;

        public void Awake()
        {
            if (_relic != null) return;
            _relic = RelicManager.Instance.PopRandomUnseenRelic();
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _relic.Sprite;
        }
        
        public void ObtainRelic()
        {
            RelicManager.Instance.AddRelic(_relic);
            gameObject.SetActive(false);
        }
    }
}