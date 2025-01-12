using DefaultNamespace.UI;
using Managers;
using Relics;
using Unity.VisualScripting;
using UnityEngine;

namespace Map
{
    public class RelicRoom : MonoBehaviour
    {
        [SerializeField] private AbstractRelic _relic;
        [SerializeField] private DescriptionViewer _descriptionViewer;

        public bool Taken { get; private set; } = false;

        public void Awake()
        {
            if (_relic == null)
            {
                _relic = RelicManager.Instance.PopRandomUnseenRelic();
                transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _relic.Sprite;
            }
            _descriptionViewer.Text = _relic.Description;
        }
        
        public void ObtainRelic()
        {
            gameObject.SetActive(false);
            Taken = true;
            RelicManager.Instance.AddRelic(_relic);
        }
    }
}