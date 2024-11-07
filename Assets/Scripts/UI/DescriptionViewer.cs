using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * shows text when hovering
 */
namespace DefaultNamespace.UI
{
    public class DescriptionViewer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TMP_Text _descriptionTMP;
        
        public string Text
        {
            get => _descriptionTMP.text;
            set => _descriptionTMP.text = value;
        }

        private void OnEnable()
        {
            OnPointerExit(null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _descriptionTMP.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _descriptionTMP.gameObject.SetActive(false);
        }
    }
}