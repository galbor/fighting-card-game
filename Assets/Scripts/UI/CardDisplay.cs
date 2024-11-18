using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Managers;

namespace DefaultNamespace.UI
{
    public class CardDisplay : MonoBehaviour
    {
        private BasicCard _card;
        [SerializeField] private Image _cardImage;
        [SerializeField] private Image _cardSprite;
        [SerializeField] private TMP_Text _numberText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private TMP_Text _descriptionText;

        private Vector3 _prevExtends = Vector3.zero;
        private Vector3 _extends;

        public void SetCard(BasicCard card)
        {
            if (card == null)
            {
                gameObject.SetActive(false);
            }

            this._card = card;
            _cardImage.sprite = card.Image;
            _nameText.text = card.Name;
            _descriptionText.text = card.DisplayDescription;
            _energyText.text = card.Cost.ToString();
            // SetScale();
            EnergyCostColor();
        }

        public void EnergyCostColor()
        {
            //TODO not access Instance too much?
            _energyText.color = PlayerTurn.Instance.Energy < _card.Cost ? Color.red : Color.white;
        }

        public void SetNumberActive(bool active)
        {
            _numberText.gameObject.SetActive(active);
        }

        public void SetCardNumber(int num)
        {
            _numberText.text = num.ToString();
        }

        public Vector2 GetCardSize()
        {
            return transform.localScale * _cardSprite.GetComponent<RectTransform>().sizeDelta;
        }
    }
}