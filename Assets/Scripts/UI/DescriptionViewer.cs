using UI_Carriers;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * shows text when hovering
 */
namespace DefaultNamespace.UI
{
    public class DescriptionViewer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string Text { get; set; }


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