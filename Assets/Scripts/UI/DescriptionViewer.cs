using UI_Carriers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

/**
 * shows text when hovering
 */
namespace DefaultNamespace.UI
{
    public class DescriptionViewer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [FormerlySerializedAs("_defaultText")] [SerializeField] private string _text;
        public string Text { get => _text; set => _text = value; }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            Description.AddDescription(gameObject, Text);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Description.RemoveDescription(gameObject);
        }
    }
}